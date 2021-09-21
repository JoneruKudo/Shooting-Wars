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

    public GameObject mainMenuScreen;
    public GameObject loadingScreen;
    public TMP_Text loadingText;

    public GameObject testButton;

    private void Start()
    {
        CloseAllScreen();

        loadingScreen.SetActive(true);

        loadingText.text = "Connecting to Server...";

        PhotonNetwork.ConnectUsingSettings();

#if UNITY_EDITOR
        testButton.SetActive(true);
#endif

    }

    public void CloseAllScreen()
    {
        mainMenuScreen.SetActive(false);
        loadingScreen.SetActive(false);
    }

    public override void OnConnectedToMaster()
    {
        CloseAllScreen();
        mainMenuScreen.SetActive(true);
    }

    public void CreateRoom()
    {

    }
}
