using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "ScriptableObject/WeaponItem")]

public class WeaponItem : Item
{
    public void Awake()
    {
        Type = ItemType.Weapon;
    }
}
