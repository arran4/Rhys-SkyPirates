using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class DefaultSelectState : HexSelectState
{
    private HexSelection HexState;
    public override void EnterState(HexSelectManager manager)
    {
        HexState = manager.GetComponent<HexSelection>();
        manager.Responce = HexState;
        manager.Highlight = manager.GetComponent<HexHighlight>();
        manager.Responce.Deselect();
    }

    public override void UpdateState(HexSelectManager manager)
    {
        // Default update logic from HexSelectManager
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
        foreach (Tile tile in ((HexSelection)manager.Responce).movementRangeEnemy)
        {
            tile.Hex.meshupdate(HexState.selectedMat);
        }
    }

    public override void ExitState(HexSelectManager manager)
    {
        // Clean up if necessary
    }
}
