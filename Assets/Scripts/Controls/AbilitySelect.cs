using System.Collections.Generic;
using UnityEngine;

public class AbilitySelect : MonoBehaviour, ISelectionResponce
{
    public Material HighlightMat;
    public ActiveAbility ActiveAbility;
    public Tile SourceTile;
    public List<Tile> Area = new List<Tile>();

    public GameObject SelectedObject { get; private set; } = null;

    public GameObject CurrentSelection()
    {
        return SelectedObject;
    }

    public void Select(GameObject selection)
    {
        Tile tile = selection.GetComponent<Tile>();

        if (tile != null && Area.Contains(tile))
        {
            SelectedObject = selection;

            // Selection complete. Trigger ability resolution here or pass to next system.
            Debug.Log("Ability selected at tile: " + tile.name);

            if (ActiveAbility != null && ActiveAbility.Actions != null)
            {
                Board board = tile.transform.GetComponentInParent<Map>()?.PlayArea;
                if (board == null)
                {
                    Map map = FindObjectOfType<Map>();
                    if (map != null) board = map.PlayArea;
                }

                if (board != null)
                {
                    foreach (var action in ActiveAbility.Actions)
                    {
                        if (action.MoveEffect == Effect.Push || action.MoveEffect == Effect.Pull || action.MoveEffect == Effect.Slide)
                        {
                            Pawn targetPawn = tile.Contents;
                            if (targetPawn != null && SourceTile != null)
                            {
                                // Using Size as the push/pull distance
                                ActionResolver.ApplyForcedMovement(targetPawn, SourceTile, action.MoveEffect, action.Size, board);
                            }
                        }
                    }
                }
            }

            TurnManager.Instance.currentTurn.ActionTaken = true;
            EventManager.TriggerActionExicuted(false);

            // Return to previous state after selection
            HexSelectManager.Instance.ReturnToPreviousState();
        }
    }

    public void Deselect()
    {
        HexSelectManager.Instance.ReturnToPreviousState();
    }

    public void CleanUp()
    {
        Area.Clear();
        SelectedObject = null;
    }
}
