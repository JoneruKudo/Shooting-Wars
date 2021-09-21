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
    public TMP_InputField createRoomNameInputField;

    public GameObject lobbyScreen;
    public TMP_Text lobbyRoomNameText;
    public TMP_Text[] playerNames;

    public GameObject findRoomScreen;
    public TMP_InputField findRoomNameInputField;

    public GameObject errorScreen;
    public TMP_Text errorText;


    private void Start()
    {
        PhotonNetwork.NickName = "Player " + Random.Range(1, 1000);

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
        findRoomScreen.SetActive(false);
        errorScreen.SetActive(false);
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
        if (string.IsNullOrEmpty(createRoomNameInputField.text)) return;

        PhotonNetwork.CreateRoom(createRoomNameInputField.text);

        CloseAllScreen();

        loadingScreen.SetActive(true);

        loadingText.text = "Creating a room...";
    }

    public void LeaveRoom()
    {
        if (!PhotonNetwork.InRoom) return;

        PhotonNetwork.LeaveRoom();

        CloseAllScreen();

        loadingScreen.SetActive(true);

        loadingText.text = "Leaving room...";
    }

    public override void OnCreatedRoom()
    {
        CloseAllScreen();

        lobbyScreen.SetActive(true);

        lobbyRoomNameText.text = "Room Name : " + PhotonNetwork.CurrentRoom.Name;

        UpdatePlayersNameInLobby();
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
        PhotonNetwork.CreateRoom("test");
    }

    public void JoinRoom()
    {
        if (string.IsNullOrEmpty(findRoomNameInputField.text)) return;

        PhotonNetwork.JoinRoom(findRoomNameInputField.text);
    }

    public override void OnJoinedRoom()
    {
        CloseAllScreen();

        lobbyScreen.SetActive(true);

        lobbyRoomNameText.text = "Room Name : " + PhotonNetwork.CurrentRoom.Name;

        UpdatePlayersNameInLobby();
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        UpdatePlayersNameInLobby();
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        UpdatePlayersNameInLobby();
    }

    private void UpdatePlayersNameInLobby()
    {
        var playerList = PhotonNetwork.PlayerList;

        int playerIndex = 0;

        foreach (var player in playerList)
        {
            if (player.IsLocal)
            {
                playerNames[playerIndex].color = new Color(0f, 255f, 17f, 255f);
            }
            else
            {
                playerNames[playerIndex].color = new Color(255f, 255f, 255, 255f);
            }

            playerNames[playerIndex].text = player.NickName;
            playerNames[playerIndex].fontStyle = FontStyles.Bold;

            playerIndex++;

            if (playerIndex >= playerList.Length)
            {
                playerIndex = playerList.Length - 1;
            }
        }

        for (int i = playerIndex + 1; i < playerNames.Length; i++)
        {
            playerNames[i].text = "player slot " + (i + 1);
            playerNames[i].fontStyle = FontStyles.Italic;
            playerNames[i].color = new Color(255f, 255f, 255, 60f);
        }

    }

    public void FindRoom()
    {
        CloseAllScreen();

        findRoomScreen.SetActive(true);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        CloseAllScreen();

        errorScreen.SetActive(true);

        errorText.text = "Failed To Join A Room: " + message;
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        CloseAllScreen();

        errorScreen.SetActive(true);

        errorText.text = "Failed To Create A Room : " + message;
    }

    public void CloseErrorScreen()
    {
        ReturnToMainMenu();
    }

}
