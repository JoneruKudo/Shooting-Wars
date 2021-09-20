using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSwitcher : bl_SlotSwitcher
{
    public static WeaponSwitcher instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        UpdateSlotSwitcherInfo(0);
    }

    public void UpdateSlotSwitcherInfo(int slotIndex)
    {
        WeaponNameText.text = slotsData[slotIndex].ItemName;
        PreviewImage.sprite = slotsData[slotIndex].Icon;
    }

}
