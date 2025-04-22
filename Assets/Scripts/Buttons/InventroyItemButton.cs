using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventroyItemButton : MonoBehaviour
{
    public Item Equip;

    public void onClick()
    {
        if (CanvasManager.CanvasInstance.positon == 0)
        {
            EventManager.EquipmentChangeTrigger(Equip.Type, Equip);
        }
        else if(CanvasManager.CanvasInstance.positon == 2)
        {
            EventManager.ShowInfoTrigger(Equip);
        }
    }
}
