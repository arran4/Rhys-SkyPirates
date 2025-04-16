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

    void Start()
    {
        inputActions = EventManager.EventInstance.inputActions;
        PlayerPawnsList = new List<PlayerPawns>();

        foreach (var pawn in PawnManager.PawnManagerInstance.PlayerPawns)
        {
            MovePawnTo(pawn, Storage.position);
            PlayerPawnsList.Add(pawn);
        }

        PawnOnScreen = 0;
        MovePawnTo(PlayerPawnsList[PawnOnScreen], OnScreen.position);

        EventManager.OnEquipmentChange += UpdateEquipement;
        EventManager.CharaterChangeTrigger(PlayerPawnsList[PawnOnScreen]);
    }

    void Update()
    {
        if (!SceneLoad)
        {
            EventManager.CharaterChangeTrigger(PlayerPawnsList[PawnOnScreen]);
            SceneLoad = true;
        }

        CharaterSwitch();
    }

    public void CharaterSwitch()
    {
        if (inputActions.Menu.SwitchCharater.triggered)
        {
            MovePawnTo(PlayerPawnsList[PawnOnScreen], Storage.position);

            PawnOnScreen += (int)inputActions.Menu.SwitchCharater.ReadValue<float>();

            if (PawnOnScreen < 0)
                PawnOnScreen = PlayerPawnsList.Count - 1;
            else if (PawnOnScreen >= PlayerPawnsList.Count)
                PawnOnScreen = 0;

            MovePawnTo(PlayerPawnsList[PawnOnScreen], OnScreen.position);
            EventManager.CharaterChangeTrigger(PlayerPawnsList[PawnOnScreen]);
        }
    }

    public void UpdateEquipement(ItemType TypeToGChange, Item ToChange)
    {
        PlayerPawns currentPawn = PlayerPawnsList[PawnOnScreen];
        currentPawn.Equiped.UpdateEquipment(TypeToGChange, ToChange);

        PlayerList.ListInstance.AllPlayerPawns[PawnOnScreen]
            .GetComponent<PlayerPawns>().Equiped.UpdateEquipment(TypeToGChange, ToChange);

        EventManager.CharaterChangeTrigger(currentPawn);
    }

    public void OnEnable()
    {
        SceneLoad = false;
    }

    public void OnDestroy()
    {
        EventManager.OnEquipmentChange -= UpdateEquipement;
    }

    private void MovePawnTo(PlayerPawns pawn, Vector3 position)
    {
        pawn.gameObject.transform.position = position;
    }
}
