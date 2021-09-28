using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AmmoPickUp : MonoBehaviour
{
    public int ammoAmount;
    public AmmoType ammoType;

    PlayerController masterPlayerCon;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            PlayerController playerCon = other.GetComponent<PlayerController>();

            playerCon.AddAmmo(ammoType, ammoAmount);

            masterPlayerCon.DestroyPickup();
        }
    }

    public void SetMasterPlayerController(PlayerController playerCon)
    {
        masterPlayerCon = playerCon;
    }
}

public enum AmmoType
{
    M4Rifle,
    MachineGun,
    Sniper
}
