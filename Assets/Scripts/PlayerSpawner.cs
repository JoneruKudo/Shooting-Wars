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
    private float deathAnimationTime = 3f;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if (PhotonNetwork.IsConnected)
        {
            SpawnPlayer();
        }
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
        yield return new WaitForSecondsRealtime(deathAnimationTime);

        PhotonNetwork.Destroy(player);

        yield return new WaitForSecondsRealtime(timeToRespawn);

        SpawnPlayer();
    }
}
