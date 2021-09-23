using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Photon.Pun;

public class JumpButton : bl_MobileButton
{
    PlayerController playerCon = null;

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);

        if (playerCon == null)
        {
            playerCon = HUDController.instance.GetPlayerController();
        }

        playerCon.Jump();
    }
}
