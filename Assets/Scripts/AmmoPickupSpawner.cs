using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class AmmoPickupSpawner : MonoBehaviourPunCallbacks
{
    public AmmoPickUp[] ammoPickups;
    public float spawnTime;
    public int spawnerIndex;

    GameObject objInstantiated;

    public void SpawnPickup()
    {
        if (!PhotonNetwork.LocalPlayer.IsMasterClient) return;

        int ammoToSpawn = Random.Range(0, ammoPickups.Length);

        objInstantiated = PhotonNetwork.Instantiate(
            ammoPickups[ammoToSpawn].gameObject.name, 
            transform.position, 
            Quaternion.identity);

        objInstantiated.GetComponent<PhotonView>().RPC("RPCSetSpawnerIndex", RpcTarget.All, spawnerIndex);
    }

    public void DestroyPickup()
    {
        if (!PhotonNetwork.LocalPlayer.IsMasterClient) return;

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

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (!PhotonNetwork.LocalPlayer.IsMasterClient) return;

        objInstantiated.GetComponent<PhotonView>().RPC("RPCSetSpawnerIndex", RpcTarget.All, spawnerIndex);
    }


}
