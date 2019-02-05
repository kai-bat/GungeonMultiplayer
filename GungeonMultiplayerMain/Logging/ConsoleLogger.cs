using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GungeonMultiplayerMain.Logging
{
    public static class ConsoleLogger
    {
        public static void Log(string text)
        {
            Console.WriteLine($"[GungeonOnline] {text}");
        }
    }
}
