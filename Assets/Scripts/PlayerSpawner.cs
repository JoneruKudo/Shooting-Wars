using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerSpawner : MonoBehaviour
{
    public Transform[] spawnPoints;
    public GameObject playerPrefab;
    int spawnIndex;

    public static PlayerSpawner instance;
    private GameObject player;

    public float timeToRespawn;

    bool isDead = false;
    float deathTimer;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if (PhotonNetwork.IsConnected)
        {
            SpawnPlayer();

            AmmoPickupSpawner[] spawners = FindObjectsOfType<AmmoPickupSpawner>();

            foreach (AmmoPickupSpawner spawner in spawners)
            {
                spawner.SpawnPickup();
            }
        }

        deathTimer = timeToRespawn + 1f;
    }

    private void SpawnPlayer()
    {
        spawnIndex = Random.Range(0, spawnPoints.Length);

        player = PhotonNetwork.Instantiate(
            playerPrefab.name, 
            spawnPoints[spawnIndex].position, 
            spawnPoints[spawnIndex].rotation);
    }

    public void PlayerDie()
    {
        StartCoroutine(DieCo());
    }

    private IEnumerator DieCo()
    {
        MatchManager.instance.UpdatePlayerInfoSend(PhotonNetwork.LocalPlayer.NickName, 1, 1);

        HUDController.instance.respawningPanel.SetActive(true);

        HUDController.instance.touchBlockerPanel.SetActive(true);

        isDead = true;

        yield return new WaitForSecondsRealtime(timeToRespawn);

        player.GetComponent<PhotonView>().RPC("RpcDisablePlayerBodyOverNetwork", RpcTarget.All);

        yield return new WaitForSecondsRealtime(1f);

        if (!MatchManager.instance.isMatchEnded)
        {
            RespawnPlayer();
        }
    }

    private void Update()
    {
        if (isDead)
        {
            deathTimer -= Time.deltaTime;

            if (deathTimer <= 0)
            {
                deathTimer = 0;
            }

            HUDController.instance.respawningText.text = "Respawning in " + (int)deathTimer + "...";
        }
    }

    private void RespawnPlayer()
    {
        isDead = false;

        deathTimer = timeToRespawn + 1f;

        HUDController.instance.respawningPanel.SetActive(false);

        HUDController.instance.touchBlockerPanel.SetActive(false);

        spawnIndex = Random.Range(0, spawnPoints.Length);
        
        player.GetComponent<PhotonView>().RPC("RpcRespawn", 
            RpcTarget.All, 
            spawnPoints[spawnIndex].position, 
            spawnPoints[spawnIndex].rotation);
    }
}
