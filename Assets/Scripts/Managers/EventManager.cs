using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class used for storing any events. All events should be here if not realocate here and use from the instance of the manager.
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

        //Have to orginise all controls into one script and have that initalize this.
        //For nowthis will give the entire project access without allocating multiple times.
        inputActions = new BasicControls();
        inputActions.Battle.Enable();
    }
}
