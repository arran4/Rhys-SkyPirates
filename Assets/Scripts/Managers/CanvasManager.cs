using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CanvasManager : MonoBehaviour
{
    public List<Canvas> Menues;
    public BasicControls inputActions;
    public int positon;
    // Start is called before the first frame update
    public void Start()
    {
        positon = 0;
        inputActions = EventManager.EventInstance.inputActions;
        foreach (Canvas x in gameObject.GetComponentsInChildren<Canvas>())
        {
            Menues.Add(x);
            x.gameObject.SetActive(false);
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
            Menues[positon].gameObject.SetActive(true);
        }
    }
}
