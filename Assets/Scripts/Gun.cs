using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
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

    public void AddStartingReserveAmmo()
    {
        currentAmmoReserve = startingReserveAmmo;
    }

    public int GetCurrentAmmo() { return currentAmmo; }

    public bool IsMagazineFull() { return currentAmmo == magazineAmmoCapacity; }

    public void Reload()
    {
        if (currentAmmoReserve <= 0)
        {
            HUDController.instance.ShowWarningText("No more ammo to Reload!", 2f, Color.red);

            return;
        }

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
    }

    public void ShowAmmoDisplay()
    {
        HUDController.instance.ammoText.text = currentAmmo + "/" + currentAmmoReserve;
    }
}
