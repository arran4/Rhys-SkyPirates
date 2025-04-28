using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempSave : MonoBehaviour
{
    public Map ToSave;

    public void Press()
    {
        
        SaveLoadManager.SaveLoadInstance.SaveMapToJson(ToSave, Application.persistentDataPath + "/" + "save.json");
    }
}
