using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager EventInstance { get; private set; }
    public BasicControls inputActions { get; private set; }

    public static event TileHover OnTileHover;
    public delegate void TileHover(GameObject Selected);
    public static event TileSelect OnTileSelect;
    public delegate void TileSelect();
    public static event TileDeselect OnTileDeselect;
    public delegate void TileDeselect();

    public static void TileSelectTrigger()
    {
        OnTileSelect?.Invoke();
    }
    public static void TileDeselectTrigger()
    {
        OnTileDeselect?.Invoke();
    }
    public static void TileHoverTrigger(GameObject Selected)
    {
        OnTileHover?.Invoke(Selected);
        Debug.Log("Hover Triggered");
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

        inputActions = new BasicControls();

        inputActions.Battle.Enable();
    }
}
