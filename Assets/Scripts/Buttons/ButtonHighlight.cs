using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class ButtonHighlight : MonoBehaviour, IPointerEnterHandler, ISelectHandler
{

    public void OnPointerEnter(PointerEventData eventData)
    {
        var button = eventData.pointerEnter.GetComponentInParent<ItemButton>();
        if (button != null)
        {
            Debug.Log(button.CurrentEquip.name);
            return;
        }
        var button2 = eventData.pointerEnter.GetComponentInParent<InventroyItemButton>();
        if (button2 != null)
        {
            Debug.Log(button2.Equip.name);
            return;
        }

    }

    public void OnSelect(BaseEventData eventData)
    {
        var button = eventData.selectedObject.GetComponent<ItemButton>();
        if (button != null)
        {
            if (button.CurrentEquip != null)
            {
                Debug.Log(button.CurrentEquip.name);
            }
            return;
        }
        var button2 = eventData.selectedObject.GetComponent<InventroyItemButton>();
        if (button2 != null)
        {
            if (button2.Equip != null)
            {
                Debug.Log(button2.Equip.name);
            }
            return;
        }
    }
}