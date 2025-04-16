using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentChangeUpdater : MonoBehaviour
{
    public GameObject ChuzpahPanel;
    public GameObject CadishnessPanel;
    public GameObject GracePanel;
    public GameObject GritPanel;
    public GameObject SerindipityPanel;
    public GameObject SwaggerPanel;

    private Image Chuzpah;
    private Image Cadishness;
    private Image Grace;
    private Image Grit;
    private Image Serindipity;
    private Image Swagger;

    private Image[] statImages;

    public Text InfoText;

    public void Start()
    {
        EventManager.OnInfoChange += InfoChange;
        EventManager.OnInfoReset += ResetPanels;
        EventManager.OnInfoCompareChange += InfoChangeCompare;

        GetImage();
    }

    public void GetImage()
    {
        Chuzpah = ChuzpahPanel.GetComponent<Image>();
        Cadishness = CadishnessPanel.GetComponent<Image>();
        Grace = GracePanel.GetComponent<Image>();
        Grit = GritPanel.GetComponent<Image>();
        Serindipity = SerindipityPanel.GetComponent<Image>();
        Swagger = SwaggerPanel.GetComponent<Image>();

        statImages = new Image[] { Chuzpah, Cadishness, Grace, Grit, Serindipity, Swagger };
    }

    public void PanelChange(int[] Colors)
    {
        if (statImages == null || statImages.Length == 0)
            GetImage();

        for (int i = 0; i < Colors.Length && i < statImages.Length; i++)
        {
            if (Colors[i] > 0)
                statImages[i].color = Color.green;
            else if (Colors[i] < 0)
                statImages[i].color = Color.red;
        }
    }

    public void InfoChange(Item Info)
    {
        PanelChange(Info.StatChanges);
        InfoText.text = Info.Info + System.Environment.NewLine + string.Join(" ", Info.StatChanges);
    }

    public void InfoChangeCompare(Item Info, int[] StatChanges)
    {
        PanelChange(StatChanges);
        InfoText.text = Info.Info + System.Environment.NewLine + string.Join(" ", Info.StatChanges);
    }

    public void ResetPanels()
    {
        if (statImages == null || statImages.Length == 0)
            GetImage();

        foreach (var img in statImages)
        {
            img.color = Color.gray;
        }
    }

    public void OnDestroy()
    {
        EventManager.OnInfoChange -= InfoChange;
        EventManager.OnInfoReset -= ResetPanels;
        EventManager.OnInfoCompareChange -= InfoChangeCompare;
    }
}
