using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MoveSelectState : HexSelectState
{
    private MoveSelect moveSelect;
    private List<Tile> movementRange;

    public override void EnterState(HexSelectManager manager)
    {
        GameObject Game = manager.Responce.CurrentSelection();
        Tile Hex = Game.GetComponent<Tile>();
        Pawn Center = Hex.Contents;

        movementRange = manager.HighlightFinder.HexReachable(Hex, Center.Stats.Movement);
        moveSelect = manager.GetComponent<MoveSelect>();
        manager.Responce = moveSelect;
        
        foreach (Tile tile in movementRange)
        {
            tile.Hex.meshupdate(moveSelect.HighlightMat);
        }
    }

    public override void UpdateState(HexSelectManager manager)
    {
        // Movement selection update logic
        // Similar logic but using moveSelect
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
        foreach (Tile tile in movementRange)
        {
            tile.Hex.meshupdate(moveSelect.HighlightMat);
        }
    }

    public override void ExitState(HexSelectManager manager)
    {
        foreach (Tile tile in movementRange)
        {
            tile.Hex.meshupdate(tile.BaseMaterial);
        }
        movementRange.Clear();
    }
}