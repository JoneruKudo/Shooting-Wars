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

    public void SpawnPickup()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        int ammoToSpawn = Random.Range(0, ammoPickups.Length);

        objInstantiated = PhotonNetwork.Instantiate(
            ammoPickups[ammoToSpawn].gameObject.name, 
            transform.position, 
            Quaternion.identity);

        objInstantiated.GetComponent<AmmoPickUp>().SetMasterPlayerController(HUDController.instance.GetPlayerController());
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
