using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class AmmoPickupSpawner : MonoBehaviourPunCallbacks
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

        objInstantiated.GetComponent<AmmoPickUp>().SetMasterPhotonView(GetPlayerController().GetComponent<PhotonView>());
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

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (newMasterClient.IsLocal)
        {
            SpawnPickup();
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
}
