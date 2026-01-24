using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class TempSave : MonoBehaviour
{
    public Map ToSave;
    public InputField input;

    public void Press()
    {
        if (!string.IsNullOrWhiteSpace(input.text))
        {
            string safeFileName = Path.GetFileName(input.text);
            if (!string.IsNullOrWhiteSpace(safeFileName))
            {
                SaveLoadManager.SaveLoadInstance.SaveMapToJson(ToSave, Application.persistentDataPath + "/" + safeFileName + ".json");
            }
        }
    }
}
