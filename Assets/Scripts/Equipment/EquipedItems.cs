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
    public int chuzpah;
    public int cadishness;
    public int grace;
    public int grit;
    public int serindipity;
    public int swagger;

    public List<Item> Equipment = new List<Item>(5);
    // Start is called before the first frame update
    void Start()
    {
        EventManager.OnInfoCompare += Compare;
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

        chuzpah = Head.StatChanges[0] + Body.StatChanges[0] + Weapon.StatChanges[0] + Feet.StatChanges[0] + Accessorie.StatChanges[0];
        cadishness = Head.StatChanges[1] + Body.StatChanges[1] + Weapon.StatChanges[1] + Feet.StatChanges[1] + Accessorie.StatChanges[1];
        grace = Head.StatChanges[2] + Body.StatChanges[2] + Weapon.StatChanges[2] + Feet.StatChanges[2] + Accessorie.StatChanges[2];
        grit = Head.StatChanges[3] + Body.StatChanges[3] + Weapon.StatChanges[3] + Feet.StatChanges[3] + Accessorie.StatChanges[3];
        serindipity = Head.StatChanges[4] + Body.StatChanges[4] + Weapon.StatChanges[4] + Feet.StatChanges[4] + Accessorie.StatChanges[4];
        swagger = Head.StatChanges[5] + Body.StatChanges[5] + Weapon.StatChanges[5] + Feet.StatChanges[5] + Accessorie.StatChanges[5];

    }

    public void Compare(Item toCompare)
    {
        Item inventoryItem = new Item();
        if(toCompare.Type == Head.Type)
        {
            inventoryItem = Head;
        }
        else if (toCompare.Type == Body.Type)
        {
            inventoryItem = Body;
        }
        if (toCompare.Type == Weapon.Type)
        {
            inventoryItem = Weapon;
        }
        if (toCompare.Type == Feet.Type)
        {
            inventoryItem = Feet;
        }
        if (toCompare.Type == Accessorie.Type)
        {
            inventoryItem = Accessorie;
        }

        int[] compareStats = new int[6];

        for(int x = 0; x < compareStats.Length; x++)
        {
            compareStats[x] = 0;
        }

        int count = 0;
        foreach (int x in toCompare.StatChanges)
        {
            switch (count)
            {
                case 0:
                    if (x > inventoryItem.StatChanges[0])
                    {
                        compareStats[0] = 1;
                    }
                    else if (x < inventoryItem.StatChanges[0])
                    {
                        compareStats[0] = -1;
                    }
                    break;
                case 1:
                    if (x > inventoryItem.StatChanges[1])
                    {
                        compareStats[1] = 1;
                    }
                    else if (x < inventoryItem.StatChanges[1])
                    {
                        compareStats[1] = -1;
                    }
                    break;
                case 2:
                    if (x > inventoryItem.StatChanges[2])
                    {
                        compareStats[2] = 1;
                    }
                    else if (x < inventoryItem.StatChanges[2])
                    {
                        compareStats[2] = -1;
                    }
                    break;
                case 3:
                    if (x > inventoryItem.StatChanges[3])
                    {
                        compareStats[3] = 1;
                    }
                    else if (x < inventoryItem.StatChanges[3])
                    {
                        compareStats[3] = -1;
                    }
                    break;
                case 4:
                    if (x > inventoryItem.StatChanges[4])
                    {
                        compareStats[4] = 1;
                    }
                    else if (x < inventoryItem.StatChanges[4])
                    {
                        compareStats[4] = -1;
                    }
                    break;
                case 5:
                    if (x > inventoryItem.StatChanges[5])
                    {
                        compareStats[5] = 1;
                    }
                    else if (x < inventoryItem.StatChanges[5])
                    {
                        compareStats[5] = -1;
                    }
                    break;
            }
            count++;
        }

        EventManager.InfoCompareChangeTrigger(toCompare, compareStats);
    }

    public void OnDestroy()
    {
        EventManager.OnInfoCompare -= Compare;
    }
}
