using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodFloorButton : MonoBehaviour
{
    public TileDataSO tile;

    public void SetChange()
    {
        EventManager.TriggerTileChange(tile);
    }
}
