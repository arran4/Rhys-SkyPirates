using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class ButtonHighlight : Selectable, IPointerEnterHandler, ISelectHandler
{
    public override void OnPointerExit(PointerEventData eventData)
    {
        EventManager.InfoResetTrigger();
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        var button = eventData.pointerEnter.GetComponentInParent<ItemButton>();
        if (button != null)
        {
            EventManager.InfoChangeTrigger(button.CurrentEquip);
            return;
        }
        var button2 = eventData.pointerEnter.GetComponentInParent<InventroyItemButton>();
        if (button2 != null)
        {
            EventManager.InfoChangeTrigger(button2.Equip);
            return;
        }

    }

    public override void OnSelect(BaseEventData eventData)
    {
        EventManager.InfoResetTrigger();
        var button = eventData.selectedObject.GetComponent<ItemButton>();
        if (button != null)
        {
            if (button.CurrentEquip != null)
            {
                EventManager.InfoChangeTrigger(button.CurrentEquip);
            }
            return;
        }
        var button2 = eventData.selectedObject.GetComponent<InventroyItemButton>();
        if (button2 != null)
        {
            if (button2.Equip != null)
            {
                EventManager.InfoChangeTrigger(button2.Equip);
            }
            return;
        }
    }
}