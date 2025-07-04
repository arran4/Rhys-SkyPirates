using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
public class SceneChange : MonoBehaviour
{
    private BasicControls inputActions;
    // Start is called before the first frame update
    void Start()
    {
        inputActions = EventManager.EventInstance.inputActions;
        string name = SceneManager.GetActiveScene().name;
        if (name == "CharacterScene")
        {
            inputActions.Battle.Disable();
            inputActions.Menu.Enable();


        }
        if (name == "BattleScene")
        {
            inputActions.Battle.Enable();
            inputActions.Menu.Disable();

        }
    }

    // Update is called once per frame
    void Update()
    {
        if(inputActions.Battle.SceneSwitch.triggered || inputActions.Menu.SceneSwitch.triggered)
        {
            string name = SceneManager.GetActiveScene().name;
            if(name == "BattleScene")
            {
                inputActions.Battle.Disable();
                inputActions.Menu.Enable();
                SceneManager.LoadScene("CharacterScene");
                
            }
            if (name == "CharacterScene")
            {
                inputActions.Battle.Enable();
                inputActions.Menu.Disable();
                SceneManager.LoadScene("BattleScene");
            }
        }
    }
}
