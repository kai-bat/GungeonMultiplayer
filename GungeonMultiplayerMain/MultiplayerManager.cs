using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Photon;
using UnityEngine;
using GungeonMultiplayerMain.Logging;
using GungeonMultiplayerMain.UI;

namespace GungeonMultiplayerMain
{
    public class MultiplayerManager : PunBehaviour
    {
        public static bool isOnline = false;
        public static PhotonView localPlayerView;
        public static PhotonView otherPlayerView;

        public static string localPlayerName;

        public static bool otherPlayerHasJoined = false;

        public void Awake()
        {
            DontDestroyOnLoad(gameObject);
            gameObject.AddComponent<PhotonView>();
            photonView.viewID = 1;

            gameObject.AddComponent<MultiplayerUI>();

            if (PlayerPrefs.HasKey("PlayerName"))
            {
                localPlayerName = PlayerPrefs.GetString("PlayerName");
            }
        }

        public static IEnumerator SetUpLocalPlayer(PlayerController player)
        {
            PhotonNetwork.ConnectUsingSettings("default");

            yield return new WaitUntil(() => PhotonNetwork.connected);
            isOnline = true;
            if (isOnline)
            {
                localPlayerView = player.gameObject.AddComponent<PhotonView>();
                if (PhotonNetwork.isMasterClient)
                {
                    localPlayerView.viewID = PhotonNetwork.AllocateViewID();
                }
            }
        }

        public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
        {
            ConsoleLogger.Log("Player join: " + newPlayer.NickName);
            GameManager.Instance.CurrentGameType = GameManager.GameType.COOP_2_PLAYER;

            GameObject coopPlayer = Instantiate(GameManager.CoopPlayerPrefabForNewGame);
            otherPlayerView = coopPlayer.AddComponent<PhotonView>();

            otherPlayerView.viewID = PhotonNetwork.AllocateViewID();

            photonView.RPC("OnConnectRPC", newPlayer, otherPlayerView.viewID);

            coopPlayer.GetComponentInChildren<PlayerController>().enabled = false;
        }

        [PunRPC]
        public void OnConnectRPC(int id)
        {
            localPlayerView.viewID = id;
        }
    }
}
