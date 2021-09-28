using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AmmoPickUp : MonoBehaviour
{
    public int ammoAmount;
    public AmmoType ammoType;

    PhotonView masterPhotonView;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.GetComponent<PhotonView>().RPC("RPCAddAmmo", RpcTarget.All, ammoType, ammoAmount);

            //PlayerController playerCon = other.GetComponent<PlayerController>();

            // playerCon.RPCAddAmmo(ammoType, ammoAmount);

            other.GetComponent<PhotonView>().RPC("RPCDestroyPickup", RpcTarget.All);
        }
    }

    public void SetMasterPhotonView(PhotonView photonView)
    {
        masterPhotonView = photonView;
    }
}

public enum AmmoType
{
    M4Rifle,
    MachineGun,
    Sniper
}
