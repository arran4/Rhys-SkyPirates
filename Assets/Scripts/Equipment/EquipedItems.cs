using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipedItems : MonoBehaviour
{
    public HeadItem Head;
    public WeaponItem Weapon;
    public BodyItem Body;
    public AccesoriesItem Accessorie;
    public FeetItem Feet;

    public List<Item> Equipment = new List<Item>(5);
    // Start is called before the first frame update
    void Start()
    {
        populateEquipment();
    }

    public void populateEquipment()
    {
        if (Equipment.Count < 5)
        {
            Equipment.Add(Head);
            Equipment.Add(Body);
            Equipment.Add(Weapon);
            Equipment.Add(Feet);
            Equipment.Add(Accessorie);
        }
    }

    public void UpdateEquipment(ItemType WhatToCahnge, Item newItem)
    {
        switch (WhatToCahnge)
        {
            case ItemType.Head:
                Equipment.Remove(Head);
                Head = (HeadItem)newItem;
                Equipment.Add(Head);
                break;
            case ItemType.Body:
                Equipment.Remove(Body);
                Body = (BodyItem)newItem;
                Equipment.Add(Body);
                break;
            case ItemType.Weapon:
                Equipment.Remove(Weapon);
                Weapon = (WeaponItem)newItem;
                Equipment.Add(Weapon);
                break;
            case ItemType.Feet:
                Equipment.Remove(Feet);
                Feet = (FeetItem)newItem;
                Equipment.Add(Feet);
                break;
            case ItemType.Accessory:
                Equipment.Remove(Accessorie);
                Accessorie = (AccesoriesItem)newItem;
                Equipment.Add(Accessorie);
                break;
        }
    }
}
