using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class AmmoPickUp : MonoBehaviourPun
{
    public int ammoAmount;
    public AmmoType ammoType;
    public int spawnerIndex;

    PlayerController playerCon;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (playerCon == null)
            {
                playerCon = GetPlayerController();
            }
           
            if (playerCon == null)
            {
                return;
            }
            
            other.GetComponent<PhotonView>().RPC("RPCAddAmmo", RpcTarget.All, ammoType, ammoAmount);

            playerCon.GetComponent<PhotonView>().RPC("RPCDestroyPickup", RpcTarget.All, spawnerIndex);  
        }
    }

    public PlayerController GetPlayerController()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in players)
        {
            if (player.GetComponent<PhotonView>().Owner.IsMasterClient)
            {
                return player.GetComponent<PlayerController>();
            }
        }
        return null;
    }
    
    [PunRPC]
    public void RPCSetSpawnerIndex(int newIndex)
    {
        spawnerIndex = newIndex;
    }
}

public enum AmmoType
{
    M4Rifle,
    MachineGun,
    Sniper
}
