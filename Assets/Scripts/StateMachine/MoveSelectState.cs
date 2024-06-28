using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MoveSelectState : HexSelectState
{
    private MoveSelect moveSelect;
    private MovementHighlight moveHighlight;
    private List<Tile> movementRange;
    private Pawn pawn;

    public override void EnterState(HexSelectManager manager)
    {
        moveSelect = manager.GetComponent<MoveSelect>();
        moveHighlight = manager.GetComponent<MovementHighlight>();
        GameObject selectedObject = manager.Responce.CurrentSelection();
        Tile hex = selectedObject.GetComponent<Tile>();
        pawn = hex.Contents;

        moveSelect.SelectedCharater = pawn;
        movementRange = manager.HighlightFinder.HexReachable(hex, pawn.Stats.Movement);
        moveSelect.Selections.Add(hex);
        moveSelect.Area = movementRange;
        moveHighlight.Area = movementRange;
        moveHighlight.Starthighlight(selectedObject);
        manager.Responce = moveSelect;
        manager.Highlight = moveHighlight;

        foreach (Tile tile in movementRange)
        {
            tile.Hex.meshupdate(moveSelect.HighlightMat);
        }
    }

    public override void UpdateState(HexSelectManager manager)
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
        {
            manager.Highlight.SetHighlight(hit.transform.gameObject);
        }
        if (manager.InputActions.Battle.MoveSelection.triggered)
        {
            manager.Highlight.MoveHighlight(manager.InputActions.Battle.MoveSelection.ReadValue<Vector2>());
        }
        if (manager.InputActions.Battle.Select.triggered)
        {
            manager.Select();
        }
        if (manager.InputActions.Battle.Deselect.triggered)
        {
            manager.Responce.Deselect();
        }
    }

    public override void ExitState(HexSelectManager manager)
    {
        foreach (Tile tile in movementRange)
        {
            tile.Hex.meshupdate(tile.BaseMaterial);
        }
        moveHighlight.CleanUp();
        moveSelect.CleanUP();
        movementRange.Clear();
        pawn = null;
    }
}