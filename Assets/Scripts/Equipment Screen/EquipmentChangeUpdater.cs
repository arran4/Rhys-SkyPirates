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

    public Text InfoText;

    public void Start()
    {
        EventManager.OnInfoChange += InfoChange;
        EventManager.OnInfoReset += ResetPanels;
        //EventManager.OnInfoCompare += InfoCompare;
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
    }

    public void PanelChange(int[] Colors)
    {
        GetImage();
        int count = 0;
        foreach (int x in Colors)
        {
            switch (count)
            {
                case 0:
                    if (x > 0)
                    {
                        Chuzpah.color = Color.green;
                    }
                    else if (x < 0)
                    {
                        Chuzpah.color = Color.red;
                    }
                    break;
                case 1:
                    if (x > 0)
                    {
                        Cadishness.color = Color.green;
                    }
                    else if (x < 0)
                    {
                        Cadishness.color = Color.red;
                    }
                    break;
                case 2:
                    if (x > 0)
                    {
                        Grace.color = Color.green;
                    }
                    else if (x < 0)
                    {
                        Grace.color = Color.red;
                    }
                    break;
                case 3:
                    if (x > 0)
                    {
                        Grit.color = Color.green;
                    }
                    else if (x < 0)
                    {
                        Grit.color = Color.red;
                    }
                    break;
                case 4:
                    if (x > 0)
                    {
                        Serindipity.color = Color.green;
                    }
                    else if (x < 0)
                    {
                        Serindipity.color = Color.red;
                    }
                    break;
                case 5:
                    if (x > 0)
                    {
                        Swagger.color = Color.green;
                    }
                    else if (x < 0)
                    {
                        Swagger.color = Color.red;
                    }
                    break;
            }
            count++;
        }
    }
    
    public void InfoChange(Item Info)
    {
        string statchange = "";
        int number = 0;
        foreach(int x in Info.StatChanges)
        {
            statchange += x.ToString() + " ";
            number++;
        }
        PanelChange(Info.StatChanges);
        InfoText.text = Info.Info + System.Environment.NewLine + statchange;
    }

    public void InfoChangeCompare(Item Info, int[] StatChanges)
    {
        string statchange = "";
        int number = 0;
        foreach (int x in Info.StatChanges)
        {
            statchange += x.ToString() + " ";
            number++;
        }
        PanelChange(StatChanges);
        InfoText.text = Info.Info + System.Environment.NewLine + statchange;
    }

    public void ResetPanels()
    {
        ChuzpahPanel.GetComponent<Image>().color = Color.gray ;
        CadishnessPanel.GetComponent<Image>().color = Color.gray;
        GracePanel.GetComponent<Image>().color = Color.gray;
        GritPanel.GetComponent<Image>().color = Color.gray;
        SerindipityPanel.GetComponent<Image>().color = Color.gray;
        SwaggerPanel.GetComponent<Image>().color = Color.gray;
    }
    public void OnDestroy()
    {
        EventManager.OnInfoChange -= InfoChange;
        EventManager.OnInfoReset -= ResetPanels;
       // EventManager.OnInfoCompare -= InfoCompare;
        EventManager.OnInfoCompareChange -= InfoChangeCompare;
    }
}
