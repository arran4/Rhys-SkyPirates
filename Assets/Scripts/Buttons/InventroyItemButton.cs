using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventroyItemButton : MonoBehaviour
{
    public Item Equip;

    public void onClick()
    {
        EventManager.EquipmentChangeTrigger(Equip.Type, Equip);
    }
}
