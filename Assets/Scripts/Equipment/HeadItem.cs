using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "ScriptableObject/HeadItem")]
public class HeadItem : Item
{
    public void Awake()
    {
        Type = ItemType.Head;
    }
}
