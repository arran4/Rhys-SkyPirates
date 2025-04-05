using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "ScriptableObject/AccesoriesItem")]

public class AccesoriesItem : Item
{
    // Start is called before the first frame update
    public void Awake()
    {
        Type = ItemType.Accessory;
    }
}
