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
}
