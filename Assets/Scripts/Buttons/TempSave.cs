using System.Collections;
using System.Collections.Generic;
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
            // SECURITY: Sanitize user input to prevent path traversal
            string safeName = PathSanitizer.SanitizeFileName(input.text);
            string fullPath = Path.Combine(Application.persistentDataPath, safeName + ".json");

            SaveLoadManager.SaveLoadInstance.SaveMapToJson(ToSave, fullPath);
        }
    }
}
