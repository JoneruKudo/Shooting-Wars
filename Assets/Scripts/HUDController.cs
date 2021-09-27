using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;

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

    Coroutine warnCor;

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
}
