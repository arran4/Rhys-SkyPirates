using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "ScriptableObject/FeetItem")]

public class FeetItem : Item
{
    public void Awake()
    {
        Type = ItemType.Feet;
    }
}
