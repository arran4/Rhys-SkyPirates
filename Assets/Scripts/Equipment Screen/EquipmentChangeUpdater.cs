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

    public Text InfoText;

    public void Start()
    {
        EventManager.OnInfoChange += InfoChange;
        EventManager.OnInfoReset += ResetPanels;
    }

    public void InfoChange(Item Info)
    {
        string statchange = "";
        int number = 0;
        foreach(int x in Info.StatChanges)
        {
            statchange += x.ToString() + " ";
            switch (number)
            {
                case 0:
                    if (x > 0)
                    {
                        ChuzpahPanel.GetComponent<Image>().color = Color.green;
                    }
                    else if (x < 0)
                    {
                        ChuzpahPanel.GetComponent<Image>().color = Color.red;
                    }
                    break;
                case 1:
                    if (x > 0)
                    {
                        CadishnessPanel.GetComponent<Image>().color = Color.green;
                    }
                    else if (x < 0)
                    {
                        CadishnessPanel.GetComponent<Image>().color = Color.red;
                    }
                    break;
                case 2:
                    if (x > 0)
                    {
                        GracePanel.GetComponent<Image>().color = Color.green;
                    }
                    else if (x < 0)
                    {
                        GracePanel.GetComponent<Image>().color = Color.red;
                    }
                    break;
                case 3:
                    if (x > 0)
                    {
                        GritPanel.GetComponent<Image>().color = Color.green;
                    }
                    else if (x < 0)
                    {
                        GritPanel.GetComponent<Image>().color = Color.red;
                    }
                    break;
                case 4:
                    if (x > 0)
                    {
                        SerindipityPanel.GetComponent<Image>().color = Color.green;
                    }
                    else if (x < 0)
                    {
                        SerindipityPanel.GetComponent<Image>().color = Color.red;
                    }
                    break;
                case 5:
                    if (x > 0)
                    {
                        SwaggerPanel.GetComponent<Image>().color = Color.green;
                    }
                    else if (x < 0)
                    {
                        SwaggerPanel.GetComponent<Image>().color = Color.red;
                    }
                    break;
            }
            number++;
        }
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
    }
}
