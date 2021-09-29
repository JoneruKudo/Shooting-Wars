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
    PhotonView playerPhotonView;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            GetComponent<Collider>().enabled = false;

            if (playerCon == null)
            {
                playerCon = GetPlayerController();
                playerPhotonView = playerCon.GetComponent<PhotonView>();
            }
           
            if (playerPhotonView == null)
            {
                GetComponent<Collider>().enabled = true;
                return;
            }
            
            other.GetComponent<PhotonView>().RPC("RPCAddAmmo", RpcTarget.All, ammoType, ammoAmount);

            playerPhotonView.RPC("RPCDestroyPickup", RpcTarget.All, spawnerIndex);  
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
