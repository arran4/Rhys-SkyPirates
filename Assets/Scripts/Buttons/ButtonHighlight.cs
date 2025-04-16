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
        HandleHoverOrSelect(eventData.pointerEnter);
    }

    public override void OnSelect(BaseEventData eventData)
    {
        EventManager.InfoResetTrigger();
        HandleHoverOrSelect(eventData.selectedObject);
    }

    private void HandleHoverOrSelect(GameObject obj)
    {
        if (obj == null) return;

        var itemButton = obj.GetComponent<ItemButton>();
        if (itemButton != null && itemButton.CurrentEquip != null)
        {
            EventManager.InfoChangeTrigger(itemButton.CurrentEquip);
            return;
        }

        var inventoryButton = obj.GetComponent<InventroyItemButton>();
        if (inventoryButton != null && inventoryButton.Equip != null)
        {
            EventManager.InfoCompareTrigger(inventoryButton.Equip);
        }
    }
}
