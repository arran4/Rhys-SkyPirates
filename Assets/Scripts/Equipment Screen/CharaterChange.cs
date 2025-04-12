using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharaterChange : MonoBehaviour
{
    public Text Chuzpah;
    public Text Cadishness;
    public Text Grace;
    public Text Grit;
    public Text Serendipity;
    public Text Swagger;

    public Text Equipment;

    public Button Head;
    public Button Body;
    public Button Weapon;
    public Button Feet;
    public Button Accessory;


    // Start is called before the first frame update
    void Start()
    {
        EventManager.OnCharaterChange += UpdateCanvas;
    }

    public void UpdateCanvas(Pawn OnScreeen)
    {
        Chuzpah.text = (OnScreeen.Stats.Chutzpah + OnScreeen.Equiped.chuzpah).ToString();
        Cadishness.text = (OnScreeen.Stats.Cadishness + OnScreeen.Equiped.cadishness).ToString();
        Grace.text = (OnScreeen.Stats.Grace + OnScreeen.Equiped.grace).ToString();
        Grit.text = (OnScreeen.Stats.Grit + OnScreeen.Equiped.grit).ToString();
        Serendipity.text = (OnScreeen.Stats.Serendipity + OnScreeen.Equiped.serindipity).ToString();
        Swagger.text = (OnScreeen.Stats.Swagger + OnScreeen.Equiped.swagger).ToString();

        Equipment.text = "";

        foreach(Item x in OnScreeen.Equiped.Equipment)
        {
            Equipment.text += x.Name + System.Environment.NewLine;

            switch(x.Type)
            {
                case ItemType.Head:
                    Head.GetComponentInChildren<Text>().text = x.Name;
                    Head.GetComponentInChildren<ItemButton>().CurrentEquip = x;
                    break;
                case ItemType.Body:
                    Body.GetComponentInChildren<Text>().text = x.Name;
                    Body.GetComponentInChildren<ItemButton>().CurrentEquip = x;
                    break;
                case ItemType.Weapon:
                    Weapon.GetComponentInChildren<Text>().text = x.Name;
                    Weapon.GetComponentInChildren<ItemButton>().CurrentEquip = x;
                    break;
                case ItemType.Feet:
                    Feet.GetComponentInChildren<Text>().text = x.Name;
                    Feet.GetComponentInChildren<ItemButton>().CurrentEquip = x;
                    break;
                case ItemType.Accessory:
                    Accessory.GetComponentInChildren<Text>().text = x.Name;
                    Accessory.GetComponentInChildren<ItemButton>().CurrentEquip = x;
                    break;
            }
        }

    }

    public void OnDestroy()
    {
        EventManager.OnCharaterChange -= UpdateCanvas;
        
    }
}
