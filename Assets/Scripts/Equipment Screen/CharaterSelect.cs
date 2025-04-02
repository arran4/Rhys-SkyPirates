using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharaterSelect : MonoBehaviour
{

    public Transform OnScreen;
    public Transform Storage;
    public List<PlayerPawns> PlayerPawnsList;
    public int PawnOnScreen = 0;
    private BasicControls inputActions;
    // Start is called before the first frame update
    void Start()
    {
        inputActions = EventManager.EventInstance.inputActions;
        PlayerPawnsList = new List<PlayerPawns>();
        foreach (PlayerPawns x in PawnManager.PawnManagerInstance.PlayerPawns)
        {
            x.gameObject.transform.position = Storage.position;
            PlayerPawnsList.Add(x);
        }

        PlayerPawnsList[0].gameObject.transform.position = OnScreen.position;
        PawnOnScreen = 0;
    }

    // Update is called once per frame
    void Update()
    {
        CharaterSwitch();
    }

    public void CharaterSwitch()
    {
        if(inputActions.Menu.SwitchCharater.triggered)
        {

            PlayerPawnsList[PawnOnScreen].gameObject.transform.position = Storage.position;
            PawnOnScreen += (int)inputActions.Menu.SwitchCharater.ReadValue<float>();

            if(PawnOnScreen < 0)
            {
                PawnOnScreen = PlayerPawnsList.Count - 1;
            }
            else if (PawnOnScreen > PlayerPawnsList.Count - 1)
            {
                PawnOnScreen = 0;
            }

            PlayerPawnsList[PawnOnScreen].gameObject.transform.position = OnScreen.position;
        }
    }
}
