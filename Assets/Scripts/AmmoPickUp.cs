using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPickUp : MonoBehaviour
{
    public int ammoAmount;
    public AmmoType ammoType;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.GetComponent<PlayerController>().AddAmmo(ammoType, ammoAmount);

            AmmoPickupSpawner.instance.DestroyPickup();
        }
    }
}

public enum AmmoType
{
    M4Rifle,
    MachineGun,
    Sniper
}
