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
        GameObject hoveredObject = eventData.pointerEnter;

        var button = hoveredObject.GetComponentInParent<ItemButton>();
        if (button != null)
        {
            if (button.CurrentEquip != null)
            {
                EventManager.InfoChangeTrigger(button.CurrentEquip);
            }
            else
            {
                Debug.LogWarning("[PointerEnter] ItemButton found but CurrentEquip is null.");
            }
            return;
        }

        var button2 = hoveredObject.GetComponentInParent<InventroyItemButton>();
        if (button2 != null)
        {
            if (button2.Equip != null)
            {
                EventManager.InfoCompareTrigger(button2.Equip);
            }
            else
            {
                Debug.LogWarning("[PointerEnter] InventroyItemButton found but Equip is null.");
            }
            return;
        }

        Debug.LogWarning("[PointerEnter] No valid button found on pointer enter.");
    }

    public override void OnSelect(BaseEventData eventData)
    {
        GameObject selectedObject = eventData.selectedObject;

        EventManager.InfoResetTrigger();

        var button = selectedObject.GetComponent<ItemButton>();
        if (button != null)
        {
            if (button.CurrentEquip != null)
            {
                EventManager.InfoChangeTrigger(button.CurrentEquip);
            }
            else
            {
                Debug.LogWarning("[Select] ItemButton found but CurrentEquip is null.");
            }
            return;
        }

        var button2 = selectedObject.GetComponent<InventroyItemButton>();
        if (button2 != null)
        {
            if (button2.Equip != null)
            {
                EventManager.InfoCompareTrigger(button2.Equip);
            }
            else
            {
                Debug.LogWarning("[Select] InventroyItemButton found but Equip is null.");
            }
            return;
        }

        Debug.LogWarning("[Select] No valid button component found.");
    }
}
