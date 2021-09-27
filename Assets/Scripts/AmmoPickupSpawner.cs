using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class AmmoPickupSpawner : MonoBehaviour
{
    public AmmoPickUp[] ammoPickups;
    public float spawnTime;

    GameObject objInstantiated;

    public static AmmoPickupSpawner instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if (PhotonNetwork.IsConnected)
        {
            SpawnPickup();
        }
    }

    private void SpawnPickup()
    {
        int ammoToSpawn = Random.Range(0, ammoPickups.Length);

        objInstantiated = PhotonNetwork.Instantiate(
            ammoPickups[ammoToSpawn].gameObject.name, 
            transform.position, 
            Quaternion.identity);
    }

    public void DestroyPickup()
    {
        StartCoroutine(DestroyCo());
    }

    private IEnumerator DestroyCo()
    {
        PhotonNetwork.Destroy(objInstantiated);

        yield return new WaitForSecondsRealtime(spawnTime);

        SpawnPickup();
    }
}
