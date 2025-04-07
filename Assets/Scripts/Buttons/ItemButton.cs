using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemButton : MonoBehaviour
{
    public ScrollRect ItemDysplay;
    public ItemType SearchItem;

    public void Start()
    {
        
    }

    public void buttonPress()
    {
        ItemDysplay.gameObject.SetActive(true);
        EventManager.ItemSelectTrigger(SearchItem);
        Debug.Log("Test");
    }


}
