using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class MainMenu : MonoBehaviourPunCallbacks
{
    public static MainMenu instance;
    private void Awake()
    {
        instance = this;
    }

    public GameObject testButton;

    public GameObject mainMenuScreen;
    public GameObject loadingScreen;
    public TMP_Text loadingText;

    public GameObject createRoomScreen;
    public TMP_InputField roomNameInputField;

    public GameObject lobbyScreen;
    public TMP_Text lobbyRoomNameText;
    public TMP_Text[] playerNames;


    private void Start()
    {
        PhotonNetwork.NickName = "junnelPogi";

        CloseAllScreen();

        loadingScreen.SetActive(true);

        loadingText.text = "Connecting to Network...";

        PhotonNetwork.ConnectUsingSettings();

#if UNITY_EDITOR
        testButton.SetActive(true);
#endif

    }

    public void CloseAllScreen()
    {
        mainMenuScreen.SetActive(false);
        loadingScreen.SetActive(false);
        createRoomScreen.SetActive(false);
        lobbyScreen.SetActive(false);
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();

        loadingText.text = "Joining Lobby...";
    }

    public override void OnJoinedLobby()
    {
        CloseAllScreen();
        mainMenuScreen.SetActive(true);
    }

    public void CreateRoomScreen()
    {
        CloseAllScreen();
        createRoomScreen.SetActive(true);
    }

    public void ReturnToMainMenu()
    {
        CloseAllScreen();
        mainMenuScreen.SetActive(true);
    }

    public void CreateRoom()
    {
        if (string.IsNullOrEmpty(roomNameInputField.text)) return;

        PhotonNetwork.CreateRoom(roomNameInputField.text);
    }

    public void LeaveRoom()
    {
        if (!PhotonNetwork.InRoom) return;

        PhotonNetwork.LeaveRoom();
    }

    public override void OnCreatedRoom()
    {
        CloseAllScreen();

        lobbyScreen.SetActive(true);

        lobbyRoomNameText.text = "Room Name : " + PhotonNetwork.CurrentRoom.Name;

        var playerList = PhotonNetwork.PlayerList;

        int playerIndex = 0;

        foreach(var player in playerList)
        {
            playerNames[playerIndex].text = player.NickName;
            playerNames[playerIndex].fontStyle = FontStyles.Bold;
            playerNames[playerIndex].color = new Color(255f, 255f, 255, 255f);

            playerIndex++;

            if(playerIndex >= playerList.Length)
            {
                playerIndex = playerList.Length - 1;
            }
        }
    }

    public override void OnLeftRoom()
    {
        ReturnToMainMenu();
    }

    public void StartGame()
    {
        Debug.Log("Game starting...");
    }

    public void TestRoom()
    {
        PhotonNetwork.CreateRoom("TestRoom");
    }
}
