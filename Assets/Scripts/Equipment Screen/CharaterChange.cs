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
        }

    }

    public void OnDestroy()
    {
        EventManager.OnCharaterChange -= UpdateCanvas;
    }
}
