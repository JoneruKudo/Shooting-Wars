using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ShootButton : bl_MobileButton
{
    private PlayerController playerCon;

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);

        if (playerCon == null)
        {
            playerCon = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        }

        playerCon.StartShooting();
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);

        if (playerCon == null)
        {
            playerCon = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        }

        playerCon.StopShooting();
    }
}
