using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harmony;
using UnityEngine;
using GungeonMultiplayerMain.Logging;

namespace GungeonMultiplayerMain
{
    public class MultiplayerPatcher
    {
        public static void Patch ()
        {
            ConsoleLogger.Log("Loading multiplayer");
            GameObject multiplayerManager = new GameObject("Multiplayer");
            multiplayerManager.AddComponent<MultiplayerManager>();
        }
    }
}
