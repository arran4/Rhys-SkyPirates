using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CanvasManager : MonoBehaviour
{
    public static CanvasManager CanvasInstance { get; private set; }
    public List<Canvas> Menues;
    public BasicControls inputActions;
    public int positon;
    public Transform CameraPosInventory;
    public Transform CameraPosEquipment;
    // Start is called before the first frame update
    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (CanvasInstance != null && CanvasInstance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            CanvasInstance = this;
        }
    }

    public void Start()
    {
        positon = 0;
        inputActions = EventManager.EventInstance.inputActions;
        foreach (Canvas x in gameObject.GetComponentsInChildren<Canvas>())
        {
            if (!Menues.Contains(x))
            {
                Menues.Add(x);
                x.gameObject.SetActive(false);
            }
        }
        Menues[positon].gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if(inputActions.Menu.MenuSwap.triggered)
        {
            Menues[positon].gameObject.SetActive(false);
            positon += (int)inputActions.Menu.MenuSwap.ReadValue<float>();
            if(positon >= Menues.Count)
            {
                positon = 0;
            }
            else if (positon < 0)
            {
                positon = Menues.Count - 1;
            }
            if(positon == 0)
            {
                Camera.main.transform.position = CameraPosEquipment.position;
            }
            else if(positon == 2)
            {
                Camera.main.transform.position = CameraPosInventory.position;
            }
            Menues[positon].gameObject.SetActive(true);
        }
    }
}
