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

    public TMP_Text healthText;
    public TMP_Text ammoText;
    public TMP_Text warningText;
    public Image reloadingFillBarImage;
    public GameObject reloadingFillBarObject;
    private float originalReloadingImageWidth;
    private float originalReloadingImageHeight;

    public GameObject respawningPanel;
    public TMP_Text respawningText;

    public GameObject playerControllerUI;

    public GameObject leaderBoardPanel;
    public GameObject playerInfoOnLeaderboard;
    private List<PlayerInfoLeaderboard> leaderboards = new List<PlayerInfoLeaderboard>();

    Coroutine warnCor;

    private void Start()
    {
        originalReloadingImageWidth = reloadingFillBarImage.rectTransform.sizeDelta.x;
        originalReloadingImageHeight = reloadingFillBarImage.rectTransform.sizeDelta.y;

        reloadingFillBarObject.SetActive(false);

        respawningPanel.SetActive(false);
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

        List<PlayerInfo> arrangeList = new List<PlayerInfo>();

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
    }

    public void HideLeaderBoard()
    {
        leaderBoardPanel.SetActive(false);
    }

    public void BackToMainMenu()
    {
        PhotonNetwork.AutomaticallySyncScene = false;

        PhotonNetwork.DestroyAll(true);

        PhotonNetwork.LeaveRoom();

        PhotonNetwork.LeaveLobby();

        SceneManager.LoadScene(0);
    }
}
