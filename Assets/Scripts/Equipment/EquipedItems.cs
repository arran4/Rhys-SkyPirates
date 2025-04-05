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

    public List<Item> Equipment;
    // Start is called before the first frame update
    void Start()
    {
        Equipment.Add(Head);
        Equipment.Add(Body);
        Equipment.Add(Weapon);
        Equipment.Add(Feet);
        Equipment.Add(Accessorie);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
