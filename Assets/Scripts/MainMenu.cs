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
    public GameObject roomNameEmptyError;

    public GameObject lobbyScreen;
    public TMP_Text lobbyRoomNameText;
    public TMP_Text playerCountText;
    public TMP_Text maxKillsText;
    public TMP_Text matchDurationText;

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

    public GameObject settingsScreen;
    public Slider musicSlider;
    public Slider sfxSlider;
    public Slider cameraSlider;
    public TMP_InputField newPlayerNameInputField;

    public GameObject setPlayerNameScreen;
    public TMP_InputField setPlayerNameInputField;
    public GameObject playerNameEmptyError;

    private void Start()
    {
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
        settingsScreen.SetActive(false);
        setPlayerNameScreen.SetActive(false);
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();

        loadingText.text = "Joining Lobby...";

        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnJoinedLobby()
    {
        if (string.IsNullOrEmpty(GameSession.instance.GetPlayerName()))
        {
            CloseAllScreen();

            setPlayerNameScreen.SetActive(true);
        }
        else
        {
            PhotonNetwork.NickName = GameSession.instance.GetPlayerName();

            ReturnToMainMenu();
        }
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
        if (string.IsNullOrEmpty(createRoomNameInputField.text))
        {
            roomNameEmptyError.SetActive(true);
            return;
        }

        roomNameEmptyError.SetActive(false);

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

        maxKillsText.text = "Max Kills : " + GameSession.instance.maxKill.ToString();

        matchDurationText.text = "Match Duration : " + GameSession.instance.matchTimeDuration.ToString() + " Mins";

        startGameButton.GetComponent<Button>().interactable = false;

        UpdatePlayersNameInLobby();

        if (isTestingMode && isQuickStart)
        {
            isQuickStart = false;
            StartGame();
        }
    }

    public void StartGame()
    {
        PhotonNetwork.CurrentRoom.IsVisible = false;

        PhotonNetwork.LoadLevel(mapNameToLoad);
    }

    public void QuickStartGame()
    {
        PhotonNetwork.CreateRoom("test");

        GameSession.instance.matchTimeDuration = 5f;

        isQuickStart = true;
    }

    public void QuickCreateRoom()
    {
        RoomOptions roomOptions = new RoomOptions();

        roomOptions.MaxPlayers = 10;

        PhotonNetwork.CreateRoom("test", roomOptions);

        GameSession.instance.matchTimeDuration = 5f;
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

        maxKillsText.text = "Max Kills : " + GameSession.instance.maxKill.ToString();

        matchDurationText.text = "Match Duration : " + GameSession.instance.matchTimeDuration.ToString() + " Mins";

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

        CheckPlayersCountInCurrentRoom();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdatePlayersNameInLobby();

        CheckPlayersCountInCurrentRoom();
    }

    private void CheckPlayersCountInCurrentRoom()
    {
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount > 1)
            {
                startGameButton.GetComponent<Button>().interactable = true;
            }
            else
            {
                startGameButton.GetComponent<Button>().interactable = false;
            }
        }
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

        playerCountText.text = "No. of Players : " + playerList.Length;

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

    public override void OnRoomListUpdate(List<RoomInfo> roomList) // this is called only on join and left lobby
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

    public void SetPlayerName()
    {
        if (string.IsNullOrEmpty(setPlayerNameInputField.text))
        {
            playerNameEmptyError.SetActive(true);
            return;
        }

        GameSession.instance.SetPlayerName(setPlayerNameInputField.text);

        PhotonNetwork.NickName = GameSession.instance.GetPlayerName();

        ReturnToMainMenu();
    }

    public void OpenSettings()
    {
        CloseAllScreen();

        settingsScreen.SetActive(true);

        newPlayerNameInputField.placeholder.GetComponent<TMP_Text>().text = GameSession.instance.GetPlayerName();
    }

    public void ApplySettings()
    {
        if (!string.IsNullOrEmpty(newPlayerNameInputField.text))
        {
            GameSession.instance.SetPlayerName(newPlayerNameInputField.text);

            PhotonNetwork.NickName = GameSession.instance.GetPlayerName();
        }

        ReturnToMainMenu();
    }

    public void DropDownMatchDuration(int value)
    {
        switch (value)
        {
            case 0:
                GameSession.instance.matchTimeDuration = 5f;
                break;

            case 1:
                GameSession.instance.matchTimeDuration = 10f;
                break;

            case 2:
                GameSession.instance.matchTimeDuration = 15f;
                break;
        }
    }

    public void DropDownMaxKill(int value)
    {
        switch (value)
        {
            case 0:
                GameSession.instance.maxKill = 10;
                break;

            case 1:
                GameSession.instance.maxKill = 20;
                break;

            case 2:
                GameSession.instance.maxKill = 30;
                break;
        }
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
