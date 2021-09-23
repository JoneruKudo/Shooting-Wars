using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Photon.Pun;

public class ShootButton : bl_MobileButton
{
    PlayerController playerCon = null;

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);

        if (playerCon == null)
        {
            playerCon = HUDController.instance.GetPlayerController();
        }

        playerCon.StartShooting();
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);

        if (playerCon == null)
        {
            playerCon = HUDController.instance.GetPlayerController();
        }

        playerCon.StopShooting();
    }
}
