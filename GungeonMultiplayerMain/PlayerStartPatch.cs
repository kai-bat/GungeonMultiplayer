using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Harmony;

namespace GungeonMultiplayerMain
{
    [HarmonyPatch(typeof(PlayerController), "Start")]
    public static class PlayerStartPatch
    {
        public static void Postfix(PlayerController __instance)
        {
            MultiplayerManager.SetUpLocalPlayer(__instance);
        }
    }
}
