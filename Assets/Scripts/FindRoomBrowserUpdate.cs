using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class FindRoomBrowserUpdate : MonoBehaviour
{
    public float updateIntervalSeconds;
    private Coroutine updateRoomBrowserCo;

    private void OnEnable()
    {
        updateRoomBrowserCo = StartCoroutine(UpdateRoomBrowser());
    }

    private void OnDisable()
    {
        StopCoroutine(updateRoomBrowserCo);
    }

    private IEnumerator UpdateRoomBrowser()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(updateIntervalSeconds);

            MainMenu.instance.isRoomBrowserOn = true;

            if (PhotonNetwork.InLobby)
            {
                PhotonNetwork.LeaveLobby();
            }

            if (!PhotonNetwork.InLobby)
            {
                PhotonNetwork.JoinLobby();
            }
        }
    }
}

