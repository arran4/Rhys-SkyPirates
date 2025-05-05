using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public class TempLoad : MonoBehaviour
{
    public string Load;

    public Map ToLoad;


    public void Press()
    {
        for (int x = 0; x < ToLoad.PlayArea._size_X; x++)
        {
            for (int y = 0; y < ToLoad.PlayArea._size_Y; y++)
            {
                Destroy(ToLoad.PlayArea.get_Tile(x, y).gameObject);
            }
        }
        SaveLoadManager.SaveLoadInstance.LoadMapFromJson(ToLoad, Application.persistentDataPath + "/" + Load);
    }
}
