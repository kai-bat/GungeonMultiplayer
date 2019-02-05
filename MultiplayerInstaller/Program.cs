using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System.IO;

namespace MultiplayerInstaller
{
    class Program
    {
        static string modAssembly;
        static string gameAssembly;
        static string backupAssembly;
        static DefaultAssemblyResolver resolver;

        static void Main(string[] args)
        {
            Console.WriteLine("Patching EtG Game!...");
            gameAssembly = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\Assembly-CSharp.dll"));
            modAssembly = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"GungeonMultiplayerMain.dll"));
            backupAssembly = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\Assembly-CSharp-multibackup.dll"));
            resolver = new DefaultAssemblyResolver();
            resolver.AddSearchDirectory(Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\")));

            Console.WriteLine("Game assembly file at: "+gameAssembly);

            Console.WriteLine("Please input install, uninstall or exit: ");
            string input = Console.ReadLine();

            input = input.ToLower();
            try
            {
                switch (input)
                {
                    case "install":
                        Install();
                        break;
                    case "uninstall":
                        Uninstall();
                        break;

                }
            }
            catch(Exception e)
            {
                Console.WriteLine("Installer Failed: " + e.Message + "\n" + e.StackTrace);
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        static void Uninstall()
        {
            if (!IsInstalled())
            {
                Console.WriteLine("Could not uninstall, no patch found!");
                return;
            }

            bool succeeded = true;

            try
            {
                if (File.Exists(backupAssembly))
                {
                    File.Delete(backupAssembly);
                }
                File.Move(gameAssembly, backupAssembly);

                using (AssemblyDefinition game = AssemblyDefinition.ReadAssembly(backupAssembly, new ReaderParameters { ReadWrite = true, AssemblyResolver = resolver }))
                {
                    AssemblyDefinition mod = AssemblyDefinition.ReadAssembly(modAssembly);
                    MethodDefinition patchMethod = mod.MainModule.GetType("GungeonMultiplayerMain.MultiplayerPatcher").Methods.First(x => x.Name == "Patch");

                    TypeDefinition type = game.MainModule.GetType("GameManager");
                    MethodDefinition method = type.Methods.First(x => x.Name == "Awake");

                    Instruction inst = null;

                    foreach (Instruction instruction in method.Body.Instructions)
                    {
                        if (instruction.OpCode == OpCodes.Call && instruction.ToString() == "System.Void GungeonMultiplayerMain.MultiplayerPatcher.Patch()")
                        {
                            inst = instruction;
                        }
                    }

                    if (inst != null)
                    {
                        method.Body.GetILProcessor().Remove(inst);
                    }
                    else
                    {
                        throw new Exception("Uh Oh! This should not have happened, I guess we failed to detect no patch was present? Aborting...");
                    }

                    game.Write(gameAssembly);
                }
                File.Delete(backupAssembly);
                Console.WriteLine("Success!");
            }
            catch (Exception e)
            {
                Console.WriteLine("Uninstall failed: " + e.Message + "\n" + e.StackTrace);
                succeeded = false;
            }

            if(succeeded)
            {
                Console.WriteLine("Uninstalled mod successfully!");
            }
        }

        static void Install()
        {
            if(IsInstalled())
            {
                Console.WriteLine("Could not install, patch already detected");
                return;
            }

            bool succeeded = true;

            try
            {
                if (File.Exists(backupAssembly))
                {
                    File.Delete(backupAssembly);
                }
                File.Move(gameAssembly, backupAssembly);

                using (AssemblyDefinition game = AssemblyDefinition.ReadAssembly(backupAssembly, new ReaderParameters { ReadWrite = true, AssemblyResolver = resolver }))
                {
                    AssemblyDefinition mod = AssemblyDefinition.ReadAssembly(modAssembly);
                    MethodDefinition patchMethod = mod.MainModule.GetType("GungeonMultiplayerMain.MultiplayerPatcher").Methods.First(x => x.Name == "Patch");

                    TypeDefinition type = game.MainModule.GetType("GameManager");
                    MethodDefinition method = type.Methods.First(x => x.Name == "Awake");

                    method.Body.GetILProcessor().InsertBefore(method.Body.Instructions[0], Instruction.Create(OpCodes.Call, method.Module.ImportReference(patchMethod)));

                    game.Write(gameAssembly);
                }
                File.Delete(backupAssembly);
            }
            catch (Exception e)
            {
                Console.WriteLine("Patching failed: " + e.Message + "\n" + e.StackTrace);
                succeeded = false;
            }

            if(succeeded)
            {
                Console.WriteLine("Installed Mod Successfully!");
            }
        }

        static bool IsInstalled()
        {
            using (AssemblyDefinition game = AssemblyDefinition.ReadAssembly(gameAssembly, new ReaderParameters { ReadWrite = true, AssemblyResolver = resolver }))
            {
                MethodDefinition method = game.MainModule.GetType("GameManager").Methods.First(x => x.Name == "Awake");
                foreach(Instruction instruction in method.Body.Instructions)
                {
                    if(instruction.OpCode == OpCodes.Call && instruction.Operand.ToString() == "System.Void GungeonMultiplayerMain.MultiplayerPatcher::Patch()")
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
