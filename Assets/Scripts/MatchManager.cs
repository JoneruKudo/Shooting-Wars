using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class MatchManager : MonoBehaviourPunCallbacks, IOnEventCallback
{
    public static MatchManager instance;

    private void Awake()
    {
        instance = this;
    }

    public List<PlayerInfo> allPlayers = new List<PlayerInfo>();
    private int localPlayerIndex;

    public enum EventCode : byte
    {
        NewPlayer,
        PlayerList,
        UpdatePlayerInfo
    }


    private void Start()
    {
        if (!PhotonNetwork.IsConnected && SceneManager.GetActiveScene().buildIndex != 0)
        {
            SceneManager.LoadScene(0);
        }
        else
        {
            NewPlayerSend();
        }
    }

    public override void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    public override void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code < 200)
        {
            EventCode eventCode = (EventCode)photonEvent.Code;
            object[] data = (object[])photonEvent.CustomData;

            switch (eventCode)
            {
                case EventCode.NewPlayer:
                    NewPlayerReceive(data);
                    break;

                case EventCode.PlayerList:
                    PlayerListReceive(data);
                    break;

                case EventCode.UpdatePlayerInfo:
                    UpdatePlayerInfoReceive(data);
                    break;
            }
        }
    }

    public void NewPlayerSend()
    {
        object[] package = new object[] { PhotonNetwork.NickName, PhotonNetwork.LocalPlayer.ActorNumber, 0, 0 };

        PhotonNetwork.RaiseEvent(
            (byte)EventCode.NewPlayer,
            package,
            new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient },
            new SendOptions { Reliability = true }
            );
    }

    public void NewPlayerReceive(object[] dataReceived)
    {
        PlayerInfo newPlayer = new PlayerInfo(
            (string)dataReceived[0], 
            (int)dataReceived[1], 
            (int)dataReceived[2], 
            (int)dataReceived[3]
            );

        allPlayers.Add(newPlayer);

        PlayerListSend();
    }

    public void PlayerListSend()
    {
        object[] package = new object[allPlayers.Count];

        for (int i = 0; i < allPlayers.Count; i++)
        {
            object[] piece = new object[4];

            piece[0] = allPlayers[i].name;
            piece[1] = allPlayers[i].actorNumber;
            piece[2] = allPlayers[i].kills;
            piece[3] = allPlayers[i].deaths;

            package[i] = piece;
        }

        PhotonNetwork.RaiseEvent(
            (byte)EventCode.PlayerList,
            package,
            new RaiseEventOptions { Receivers = ReceiverGroup.All },
            new SendOptions { Reliability = true }
            );
    }

    public void PlayerListReceive(object[] dataReceived)
    {
        allPlayers.Clear();

        for (int i = 0; i < dataReceived.Length; i++)
        {
            object[] piece = (object[])dataReceived[i];

            PlayerInfo playerInfo = new PlayerInfo(
                (string)piece[0],
                (int)piece[1],
                (int)piece[2],
                (int)piece[3]
                );

            allPlayers.Add(playerInfo);

            if (playerInfo.actorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
            {
                localPlayerIndex = playerInfo.actorNumber;
            }
        }

        HUDController.instance.UpdatePlayerLeaderboard();
    }

    public void UpdatePlayerInfoSend(string playerName, int statType, int amount)
    {
        object[] package = new object[] { playerName, statType, amount };

        PhotonNetwork.RaiseEvent(
            (byte)EventCode.UpdatePlayerInfo,
            package,
            new RaiseEventOptions { Receivers = ReceiverGroup.All},
            new SendOptions { Reliability = true}
            );
    }

    public void UpdatePlayerInfoReceive(object[] dataReceived)
    {
        string playerName = (string)dataReceived[0];
        int statType = (int)dataReceived[1];
        int amount = (int)dataReceived[2];

        foreach (PlayerInfo playerInfo in allPlayers)
        {
            if (playerInfo.name == playerName)
            {
                switch (statType)
                {
                    case 0: // statType = 0 for kills
                        playerInfo.kills += amount;
                        break;
                    case 1: // statType = 1 for deaths
                        playerInfo.deaths += amount;
                        break;
                }

                break;
            }
        }

        HUDController.instance.UpdatePlayerLeaderboard();
    }
}

public class PlayerInfo
{
    public string name;
    public int actorNumber, kills, deaths;

    public PlayerInfo(string _name, int _actorNumber, int _kills, int _deaths)
    {
        name = _name;
        actorNumber = _actorNumber;
        kills = _kills;
        deaths = _deaths;
    }
}
