using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AmmoPickUp : MonoBehaviour
{
    public int ammoAmount;
    public AmmoType ammoType;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.GetComponent<PhotonView>().RPC("RPCAddAmmo", RpcTarget.All, ammoType, ammoAmount);

            GetPlayerController().GetComponent<PhotonView>().RPC("RPCDestroyPickup", RpcTarget.All);
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
}

public enum AmmoType
{
    M4Rifle,
    MachineGun,
    Sniper
}
