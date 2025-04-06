using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemButton : MonoBehaviour
{
    public Canvas ItemDysplay;

    public void buttonPress()
    {
        ItemDysplay.gameObject.SetActive(true);
        Debug.Log("Test");
    }
}
