using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GungeonMultiplayerMain.UI
{
    class MultiplayerUI : MonoBehaviour
    {
        bool windowIsOpen = false;
        Rect windowRect = new Rect(Screen.width - 320, 20, 300, 200);

        void OnGUI()
        {
            if(Input.GetKeyDown(KeyCode.O))
            {
                windowIsOpen = !windowIsOpen;
            }

            if(windowIsOpen)
            {
                windowRect = GUILayout.Window(0, windowRect, WindowFunction, "Gungeon Online");
            }
            else
            {
                if(GUILayout.Button("Open Window (O)"))
                {
                    windowIsOpen = true;
                }
            }
        }

        void WindowFunction(int windowId)
        {
            if(GUILayout.Button("Close Window (O)"))
            {
                windowIsOpen = false;
            }
        }
    }
}
