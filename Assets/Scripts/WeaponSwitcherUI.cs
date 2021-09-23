using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSwitcherUI : bl_SlotSwitcher
{
    public static WeaponSwitcherUI instance;

    private void Awake()
    {
        instance = this;
    }

    public void UpdateSlotSwitcherInfo(int slotIndex)
    {
        WeaponNameText.text = slotsData[slotIndex].ItemName;
        PreviewImage.sprite = slotsData[slotIndex].Icon;
    }

}
