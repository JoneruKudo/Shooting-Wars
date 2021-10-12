using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class HUDController : MonoBehaviour
{
    public static HUDController instance;

    private void Awake()
    {
        instance = this;
    }

    public GameObject crossHairImage;

    public GameObject tutorialCanvas;

    public GameObject mobileControlPanel;
    public TMP_Text healthText;
    public TMP_Text ammoText;
    public Image reloadingFillBarImage;
    public GameObject reloadingFillBarObject;
    private float originalReloadingImageWidth;
    private float originalReloadingImageHeight;

    public GameObject warningPanel;
    public TMP_Text warningText;

    public GameObject respawningPanel;
    public TMP_Text respawningText;

    public GameObject playerControllerUI;

    public GameObject leaderBoardPanel;
    public GameObject playerInfoOnLeaderboard;
    private List<PlayerInfoLeaderboard> leaderboards = new List<PlayerInfoLeaderboard>();
    public GameObject leaderBoardButton;

    public GameObject matchTimerPanel;
    public TMP_Text matchTimerText;

    public List<PlayerInfo> arrangeList = new List<PlayerInfo>();

    public GameObject endMatchPanel;
    public TMP_Text winText;
    public TMP_Text loseText;

    public GameObject loadingPanel;
    public TMP_Text loadingText;

    public GameObject optionsPanel;
    public GameObject settingsPanel;
    public Slider musicSlider;
    public Slider sfxSlider;
    public Slider cameraSlider;

    Coroutine warnCor;

    private void Start()
    {
        originalReloadingImageWidth = reloadingFillBarImage.rectTransform.sizeDelta.x;
        originalReloadingImageHeight = reloadingFillBarImage.rectTransform.sizeDelta.y;

        CloseAllPanels();

        reloadingFillBarObject.SetActive(false);

        crossHairImage.SetActive(true);
        mobileControlPanel.SetActive(true);
        warningPanel.SetActive(true);
        matchTimerPanel.SetActive(true);
        leaderBoardButton.SetActive(true);
    }

    private void CloseAllPanels()
    {
        crossHairImage.SetActive(false);
        mobileControlPanel.SetActive(false);
        warningPanel.SetActive(false);
        respawningPanel.SetActive(false);
        leaderBoardPanel.SetActive(false);
        matchTimerPanel.SetActive(false);
        endMatchPanel.SetActive(false);
        leaderBoardButton.SetActive(false);
        loadingPanel.SetActive(false);
        optionsPanel.SetActive(false);
        settingsPanel.SetActive(false);
    }

    public void UpdatePlayerLeaderboard()
    {
        if (leaderboards.Count > 0)
        {
            foreach (PlayerInfoLeaderboard leaderboard in leaderboards)
            {
                Destroy(leaderboard.gameObject);
            }

            leaderboards.Clear();
        }

        List<PlayerInfo> playerList = MatchManager.instance.allPlayers;

        arrangeList.Clear();

        while (arrangeList.Count < playerList.Count)
        {
            int highestKill = -1;
            PlayerInfo selectedPlayer = playerList[0];

            foreach (PlayerInfo playerInfo in playerList)
            {
                if (!arrangeList.Contains(playerInfo))
                {
                    if (playerInfo.kills > highestKill)
                    {
                        highestKill = playerInfo.kills;
                        selectedPlayer = playerInfo;
                    }
                }
            }

            arrangeList.Add(selectedPlayer);

            if (highestKill >= GameSession.instance.maxKill)
            {
                MatchManager.instance.isMaxKillReached = true;
            }
        }

        for (int i = 0; i < arrangeList.Count; i++)
        {
            GameObject playerInfoInstance = Instantiate(playerInfoOnLeaderboard, playerInfoOnLeaderboard.transform.parent);

            PlayerInfoLeaderboard playerInfoLeaderboard = playerInfoInstance.GetComponent<PlayerInfoLeaderboard>();

            if (PhotonNetwork.LocalPlayer.NickName == arrangeList[i].name )
            {
                playerInfoLeaderboard.playerNameText.color = Color.cyan;
                playerInfoLeaderboard.killsText.color = Color.cyan;
                playerInfoLeaderboard.deathsText.color = Color.cyan;
            }

            playerInfoLeaderboard.playerNameText.text = arrangeList[i].name;

            playerInfoLeaderboard.killsText.text = (arrangeList[i].kills).ToString();

            playerInfoLeaderboard.deathsText.text = (arrangeList[i].deaths).ToString();

            playerInfoInstance.SetActive(true);

            leaderboards.Add(playerInfoInstance.GetComponent<PlayerInfoLeaderboard>());
        }
    }

    public PlayerController GetPlayerController()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in players)
        {
            if (player.GetComponent<PhotonView>().IsMine)
            {
                return player.GetComponent<PlayerController>();
            }
        }
        return null;
    }

    public void ShowWarningText(string message, float showingDuration, Color textColor)
    {
        if (warnCor != null)
        {
            StopCoroutine(warnCor);
        }

        warnCor = StartCoroutine(WarningCor(message, showingDuration, textColor));
    }

    private IEnumerator WarningCor(string message, float waitingTime, Color textColor)
    {
        warningText.text = message;

        warningText.color = textColor;

        yield return new WaitForSecondsRealtime(waitingTime);

        warningText.text = "";
    }

    public void ShowReloadingFillBar(float reloadingTime)
    {
        StartCoroutine(ReloadingBarCo(reloadingTime));
    }

    private IEnumerator ReloadingBarCo(float reloadingTime)
    {
        reloadingFillBarObject.SetActive(true);

        float timeToReload = reloadingFillBarImage.rectTransform.sizeDelta.x / reloadingTime;

        float xWidth = originalReloadingImageWidth;

        while (true)
        {
            reloadingFillBarImage.rectTransform.sizeDelta = new Vector2(xWidth, originalReloadingImageHeight);

            if (reloadingFillBarImage.rectTransform.sizeDelta.x <= 0)
            {
                reloadingFillBarImage.rectTransform.sizeDelta = new Vector2(originalReloadingImageWidth, 
                                                                            originalReloadingImageHeight);

                reloadingFillBarObject.SetActive(false);

                break;
            }

            xWidth -= Time.deltaTime * timeToReload;

            yield return null;
        }
    }

    public void ShowLeaderBoard()
    {
        leaderBoardPanel.SetActive(true);
        leaderBoardButton.SetActive(false);
    }

    public void HideLeaderBoard()
    {
        leaderBoardPanel.SetActive(false);
        leaderBoardButton.SetActive(true);
    }

    public void MatchEndHandler(string playerName)
    {
        CloseAllPanels();

        leaderBoardPanel.SetActive(true);
        endMatchPanel.SetActive(true);

        if (playerName == PhotonNetwork.LocalPlayer.NickName)
        {
            winText.gameObject.SetActive(true);
        }
        else
        {
            loseText.gameObject.SetActive(true);
        }
    }

    public void OpenSettings()
    {
        optionsPanel.SetActive(false);

        settingsPanel.SetActive(true);

        musicSlider.value = GameSession.instance.GetMusicVolume();

        sfxSlider.value = GameSession.instance.GetSFXVolume();

        cameraSlider.value = GameSession.instance.GetCameraSensitivity();
    }

    public void ApplySettings()
    {
        GameSession.instance.ApplySettings();

        settingsPanel.SetActive(false);
    }

    public void CancelSettings()
    {
        GameSession.instance.CancelSettings();

        settingsPanel.SetActive(false);

        optionsPanel.SetActive(true);
    }

    public void OpenOptionMenu()
    {
        optionsPanel.SetActive(true);

        HideLeaderBoard();
    }

    public void CloseOptionMenu()
    {
        optionsPanel.SetActive(false);
    }

    public void OpenTutorialCanvas()
    {
        tutorialCanvas.SetActive(true);

        matchTimerPanel.SetActive(false);

        leaderBoardButton.SetActive(false);
    }

    public void CloseTutorialCanvas()
    {
        tutorialCanvas.SetActive(false);

        matchTimerPanel.SetActive(true);

        leaderBoardButton.SetActive(true);
    }

    public void SetMusicVolumeOnAudioMixer(float amount)
    {
        GameSession.instance.audioMixer.SetFloat("Music", amount);
    }

    public void SetSFXVolumeOnAudioMixer(float amount)
    {
        GameSession.instance.audioMixer.SetFloat("SFX", amount);
    }

    public void SetCameraSensitivityOnMobileInputSettings(float amount)
    {
        GameSession.instance.SetCameraSensitivityOnMobileInputSettings(amount);
    }

    public void BackToMainMenu()
    {
        CloseAllPanels();

        loadingPanel.SetActive(true);

        loadingText.text = "Back to Main Menu...";

        PhotonNetwork.AutomaticallySyncScene = false;

        PhotonNetwork.DestroyAll(true);

        PhotonNetwork.LeaveRoom();

        PhotonNetwork.LeaveLobby();

        SceneManager.LoadScene("Main Menu");
    }
}
