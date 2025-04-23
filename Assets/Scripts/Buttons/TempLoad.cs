using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempLoad : MonoBehaviour
{
    public Map ToSave;

    public void Press()
    {
        ToSave.LoadMapFromJson(Application.persistentDataPath + "/" + "save.json");
    }
}
