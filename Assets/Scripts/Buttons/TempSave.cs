using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempSave : MonoBehaviour
{
    public Map ToSave;

    public void Press()
    {
        ToSave.SaveMapToJson(Application.persistentDataPath + "/" + "save.json");
    }
}
