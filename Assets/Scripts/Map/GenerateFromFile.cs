using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateFromFile : MonoBehaviour , IGenerate
{
    public string FileName;
    public Board Generate(Map Data)
    {
        SaveLoadManager.SaveLoadInstance.LoadMapFromJson(Data, Application.dataPath + "/" + FileName);
        return Data.PlayArea;
    }
}
