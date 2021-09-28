using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;

public class MainMenu : MonoBehaviourPunCallbacks
{
    public static MainMenu instance;
    private void Awake()
    {
        instance = this;
    }

    public bool isTestingMode = false;
    public GameObject testButtons;
    private bool isQuickStart = false;

    public GameObject mainMenuScreen;
    public GameObject loadingScreen;
    public TMP_Text loadingText;

    public GameObject createRoomScreen;
    public TMP_InputField createRoomNameInputField;

    public GameObject lobbyScreen;
    public TMP_Text lobbyRoomNameText;

    public GameObject findRoomScreen;
    public GameObject roomFoundObject;
    public GameObject joinButton;
    private string selectedButtonRoomName;
    private List<RoomButton> roomButtons = new List<RoomButton>();

    public GameObject startGameButton;

    public GameObject errorScreen;
    public TMP_Text errorText;

    public string mapNameToLoad;

    public TMP_Text[] playerNames;

    private void Start()
    {
        if (PhotonNetwork.NickName.Length <= 0)
        {
            PhotonNetwork.NickName = "Player " + Random.Range(1, 1000);
        }

        CloseAllScreen();

        mainMenuScreen.SetActive(true);

        if (!PhotonNetwork.IsConnected)
        {
            CloseAllScreen();

            loadingScreen.SetActive(true);

            loadingText.text = "Connecting to Network...";

            PhotonNetwork.ConnectUsingSettings();
        }

        testButtons.SetActive(isTestingMode ? true : false);
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

        PhotonNetwork.AutomaticallySyncScene = true;
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

        PhotonNetwork.LeaveLobby();

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

        if (isTestingMode && isQuickStart)
        {
            isQuickStart = false;
            StartGame();
        }
    }

    public override void OnLeftRoom()
    {
        PhotonNetwork.JoinLobby();

        ReturnToMainMenu();
    }

    public void StartGame()
    {
        PhotonNetwork.LoadLevel(mapNameToLoad);
    }

    public void QuickStartGame()
    {
        PhotonNetwork.CreateRoom("test");

        isQuickStart = true;
    }

    public void QuickCreateRoom()
    {
        PhotonNetwork.CreateRoom("test");
    }

    public void QuickJoin()
    {
        PhotonNetwork.JoinRoom("test");
    }

    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(selectedButtonRoomName);
    }

    public override void OnJoinedRoom()
    {
        CloseAllScreen();

        lobbyScreen.SetActive(true);

        lobbyRoomNameText.text = "Room Name : " + PhotonNetwork.CurrentRoom.Name;

        if (PhotonNetwork.IsMasterClient)
        {
            startGameButton.SetActive(true);
        }
        else
        {
            startGameButton.SetActive(false);
        }

        UpdatePlayersNameInLobby();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdatePlayersNameInLobby();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
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

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            startGameButton.SetActive(true);
        }
        else
        {
            startGameButton.SetActive(false);
        }
    }

    public void FindRoom()
    {
        CloseAllScreen();

        findRoomScreen.SetActive(true);

        joinButton.GetComponent<Button>().interactable = false;
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

    public void SelectRoom(RoomButton roomButton)
    {
        selectedButtonRoomName = roomButton.GetComponentInChildren<TMP_Text>().text;

        if (string.IsNullOrEmpty(selectedButtonRoomName)) return;

        joinButton.GetComponent<Button>().interactable = true;
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {  
       foreach(RoomButton roomButton in roomButtons)
        {
            Destroy(roomButton.gameObject);
        }

        roomButtons.Clear();

        for (int i = 0; i < roomList.Count; i++)
        {
            if (!roomList[i].RemovedFromList)
            {
                GameObject roomFoundInstance = Instantiate(roomFoundObject, roomFoundObject.transform.parent);
                roomFoundInstance.GetComponentInChildren<TMP_Text>().text = roomList[i].Name;
                roomFoundInstance.SetActive(true);
                roomButtons.Add(roomFoundInstance.GetComponent<RoomButton>());
            }
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
