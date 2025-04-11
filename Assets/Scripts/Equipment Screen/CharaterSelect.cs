using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharaterSelect : MonoBehaviour
{
    public Transform OnScreen;
    public Transform Storage;
    public List<PlayerPawns> PlayerPawnsList;
    public int PawnOnScreen = 0;
    private BasicControls inputActions;
    private bool SceneLoad = false;
    // Start is called before the first frame update
    void Start()
    {
        SceneManager.sceneUnloaded += OnSceneLoaded;
        inputActions = EventManager.EventInstance.inputActions;
        PlayerPawnsList = new List<PlayerPawns>();
        foreach (PlayerPawns x in PawnManager.PawnManagerInstance.PlayerPawns)
        {
            x.gameObject.transform.position = Storage.position;
            PlayerPawnsList.Add(x);
        }

        PlayerPawnsList[0].gameObject.transform.position = OnScreen.position;
        PawnOnScreen = 0;
        EventManager.OnEquipmentChange += UpdateEquipement;
        EventManager.CharaterChangeTrigger(PlayerPawnsList[PawnOnScreen]);
    }


    // Update is called once per frame
    void Update()
    {
        if(SceneLoad == false)
        {
            EventManager.CharaterChangeTrigger(PlayerPawnsList[PawnOnScreen]);
            SceneLoad = true;
        }
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
            EventManager.CharaterChangeTrigger(PlayerPawnsList[PawnOnScreen]);
        }
    }
    public void UpdateEquipement(ItemType TypeToGChange, Item ToChange)
    {
        PlayerPawnsList[PawnOnScreen].Equiped.UpdateEquipment(TypeToGChange, ToChange);
        PlayerList.ListInstance.AllPlayerPawns[PawnOnScreen].GetComponent<PlayerPawns>().Equiped.UpdateEquipment(TypeToGChange, ToChange);
        EventManager.CharaterChangeTrigger(PlayerPawnsList[PawnOnScreen]);
    }

    void OnSceneLoaded(Scene scene)
    {
        SceneLoad = false;
    }



    public void OnDestroy()
    {
        EventManager.OnEquipmentChange -= UpdateEquipement;
        SceneManager.sceneUnloaded -= OnSceneLoaded;
    }
}
