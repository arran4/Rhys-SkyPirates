using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateFromFile : MonoBehaviour , IGenerate
{
    public string FileName;
    public Board Generate(Map Data)
    {
        Data.PlayArea = SaveLoadManager.LoadBoardFromJson(Application.dataPath + "/" + FileName, Data, Data.gameObject.transform);
        return Data.PlayArea;
    }
}
