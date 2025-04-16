using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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

    private Dictionary<ItemType, Button> equipmentButtons;

    void Start()
    {
        EventSystem.current.firstSelectedGameObject = Head.gameObject;
        EventManager.OnCharaterChange += UpdateCanvas;

        // Setup dictionary for easier lookup
        equipmentButtons = new Dictionary<ItemType, Button>
        {
            { ItemType.Head, Head },
            { ItemType.Body, Body },
            { ItemType.Weapon, Weapon },
            { ItemType.Feet, Feet },
            { ItemType.Accessory, Accessory }
        };
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

        foreach (var item in OnScreeen.Equiped.Equipment)
        {
            Equipment.text += item.Name + System.Environment.NewLine;

            if (equipmentButtons.TryGetValue(item.Type, out Button button))
            {
                var textComp = button.GetComponentInChildren<Text>();
                var itemButton = button.GetComponentInChildren<ItemButton>();

                if (textComp != null) textComp.text = item.Name;
                if (itemButton != null) itemButton.CurrentEquip = item;
            }
        }
    }

    public void OnDestroy()
    {
        EventManager.OnCharaterChange -= UpdateCanvas;
    }
}
