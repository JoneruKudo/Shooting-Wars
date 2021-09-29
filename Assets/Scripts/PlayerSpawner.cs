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

        deathTimer = deathAnimationTime + timeToRespawn;
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
        isDead = true;

        yield return new WaitForSecondsRealtime(deathAnimationTime);

        //PhotonNetwork.Destroy(player);

        player.GetComponent<PhotonView>().RPC("RpcDisablePlayerBodyOverNetwork", RpcTarget.All);

        yield return new WaitForSecondsRealtime(timeToRespawn);

        //SpawnPlayer();

        RespawnPlayer();
    }

    private void Update()
    {
        if (isDead)
        {
            deathTimer -= Time.deltaTime;

            Debug.Log((int)deathTimer);
        }
    }

    private void RespawnPlayer()
    {
        isDead = false;

        deathTimer = deathAnimationTime + timeToRespawn;

        spawnIndex = Random.Range(0, spawnPoints.Length);
        
        player.GetComponent<PhotonView>().RPC("RpcRespawn", 
            RpcTarget.All, 
            spawnPoints[spawnIndex].position, 
            spawnPoints[spawnIndex].rotation);
    }
}
