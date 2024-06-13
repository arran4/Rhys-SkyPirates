using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MoveSelectState : HexSelectState
{
    private MoveSelect moveSelect;

    public override void EnterState(HexSelectManager manager)
    {
        moveSelect = new MoveSelect();
        manager.Responce = moveSelect;
    }

    public override void UpdateState(HexSelectManager manager)
    {
        // Movement selection update logic
        // Similar logic but using moveSelect and moveHighlight instead
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
        {
            manager.Highlight.SetHighlight(hit.transform.gameObject);
        }
        if (manager.inputActions.Battle.MoveSelection.triggered)
        {
            manager.Highlight.MoveHighlight(manager.inputActions.Battle.MoveSelection.ReadValue<Vector2>());
        }
        if (manager.inputActions.Battle.Select.triggered)
        {
            manager.Select();
        }
        if (manager.inputActions.Battle.Deselect.triggered)
        {
            manager.Responce.Deselect();
        }
    }

    public override void ExitState(HexSelectManager manager)
    {
        // Clean up if necessary
    }
}