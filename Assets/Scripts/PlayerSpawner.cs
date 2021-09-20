using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public Transform[] spawnPoints;
    public GameObject playerPrefab;
    int spawnIndex;

    public static PlayerSpawner instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        SpawnPlayer();        
    }

    private void SpawnPlayer()
    {
        spawnIndex = Random.Range(0, spawnPoints.Length);

        GameObject playerInstance = Instantiate(
            playerPrefab,
            spawnPoints[spawnIndex].position,
            spawnPoints[spawnIndex].rotation);
    }


}
