using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public AmmoType ammoType;
    public int damage;
    public float weaponRange;
    public float timeBetweenShots;
    public float weaponFieldOfView;
    public float movespeedFactorWhenAiming;
    public float touchpadFactorWhenAiming;
    public int magazineAmmoCapacity;
    public int maxReserveAmmo;
    public int startingReserveAmmo;
    public float reloadingTime;

    private int currentAmmo = 0;
    private int currentAmmoReserve;

    public int GetCurrentAmmo() { return currentAmmo; }

    public int GetCurrentAmmoReserve() { return currentAmmoReserve; }

    public bool IsMagazineFull() { return currentAmmo == magazineAmmoCapacity; }

    public void LoadAmmo()
    {
        currentAmmoReserve = startingReserveAmmo;
        currentAmmo = magazineAmmoCapacity;
    }

    public void Reload()
    {
        int ammoToReload = magazineAmmoCapacity - currentAmmo;

        if (ammoToReload < currentAmmoReserve)
        {
            currentAmmo += ammoToReload;
            currentAmmoReserve -= ammoToReload;
        }
        else
        {
            ammoToReload = currentAmmoReserve;
            currentAmmo += ammoToReload;
            currentAmmoReserve -= ammoToReload;
        }

    }

    public void ReduceCurrentAmmo()
    {
        currentAmmo--;
    }

    public void AddAmmoReserve(int amount)
    {
        currentAmmoReserve += amount;

        if (currentAmmoReserve >= maxReserveAmmo)
        {
            currentAmmoReserve = maxReserveAmmo;
        }
    }

    public void ShowAmmoDisplay()
    {
        HUDController.instance.ammoText.text = currentAmmo + "/" + currentAmmoReserve;
    }
}
