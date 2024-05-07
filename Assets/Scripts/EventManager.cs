using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager EventInstance { get; private set; }

    public static event TileHover OnTileHover;
    public delegate void TileHover(Tile Selected);

    public static void TileHoverTrigger(Tile Selected)
    {
        OnTileHover?.Invoke(Selected);
    }

    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (EventInstance != null && EventInstance != this)
        {
            Destroy(this);
        }
        else
        {
            EventInstance = this;
        }
    }
}
