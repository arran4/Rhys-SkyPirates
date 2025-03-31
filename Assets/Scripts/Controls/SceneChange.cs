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
    }

    // Update is called once per frame
    void Update()
    {
        if(inputActions.Battle.SceneSwitch.triggered)
        {
            string name = SceneManager.GetActiveScene().name;
            if(name == "BattleScene")
            {
                SceneManager.LoadScene("CharaterScene");
            }
            if (name == "CharaterScene")
            {
                SceneManager.LoadScene("BattleScene");
            }
        }
    }
}
