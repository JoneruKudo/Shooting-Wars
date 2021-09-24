using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AimButton : bl_MobileButton
{
    PlayerController playerCon = null;

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);

        if (playerCon == null)
        {
            playerCon = HUDController.instance.GetPlayerController();
        }

        if (!playerCon.isAiming)
        {
            playerCon.Aim();
        }
        else
        {
            playerCon.ReleaseAim();
        }
    }
}
