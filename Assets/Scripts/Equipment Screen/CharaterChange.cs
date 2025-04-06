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
        Chuzpah.text = OnScreeen.Stats.Chutzpah.ToString();
        Cadishness.text = OnScreeen.Stats.Cadishness.ToString();
        Grace.text = OnScreeen.Stats.Grace.ToString();
        Grit.text = OnScreeen.Stats.Grit.ToString();
        Serendipity.text = OnScreeen.Stats.Serendipity.ToString();
        Swagger.text = OnScreeen.Stats.Swagger.ToString();

        Equipment.text = "";

        foreach(Item x in OnScreeen.Equiped.Equipment)
        {
            Equipment.text += x.Name + System.Environment.NewLine;

            switch(x.Type)
            {
                case ItemType.Head:
                    Head.GetComponentInChildren<Text>().text = x.Name;
                    break;
                case ItemType.Body:
                    Body.GetComponentInChildren<Text>().text = x.Name;
                    break;
                case ItemType.Weapon:
                    Weapon.GetComponentInChildren<Text>().text = x.Name;
                    break;
                case ItemType.Feet:
                    Feet.GetComponentInChildren<Text>().text = x.Name;
                    break;
                case ItemType.Accessory:
                    Accessory.GetComponentInChildren<Text>().text = x.Name;
                    break;
            }
        }

    }

    public void OnDestroy()
    {
        EventManager.OnCharaterChange -= UpdateCanvas;
    }
}
