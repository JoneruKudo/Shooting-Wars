using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WeaponSwitchButtonLeft : bl_MobileButton
{
    private PlayerController playerCon;

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);

        if (playerCon == null)
        {
            playerCon = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        }

        playerCon.SwitchWeaponOnLeft();
    }
}
