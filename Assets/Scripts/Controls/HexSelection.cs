using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexSelection : MonoBehaviour, ISelectionResponce
{
    public Material selectedMat;
    public GameObject SelectedObject { get; private set; } = null;
    public Tile SelectedTile { get; private set; } = null;
    public Pawn SelectedContents { get; private set; } = null;

    public List<Tile> movementRangeEnemy;


    public void Update()
    {
        //better implementaiton needed to fix highlight overwrite
        if (SelectedTile != null)
        {
            SelectedTile.Hex.meshupdate(selectedMat);
        }
    }
    //Basic Tile selection using recusion to swap selections
    public void Select(GameObject Selection)
    {
        if (Selection != null && SelectedObject == null)
        {
            SelectedObject = Selection;
            SelectedTile = SelectedObject.GetComponent<Tile>();
            SelectedContents = SelectedTile.Contents;
            if (SelectedContents != null)
            {
                EventManager.TriggerPawnSelect(SelectedContents);
                if (SelectedContents is PlayerPawns)
                {
                    HexSelectManager.Instance.SwitchToActionSelectState();
                }
                else
                {
                    movementRangeEnemy = HexSelectManager.Instance.HighlightFinder.AreaRing(SelectedContents.Position, SelectedContents.Stats.Movement);
                    foreach(Tile x in movementRangeEnemy)
                    {
                        if(x.Data.MovementCost == 0)
                        {
                            movementRangeEnemy.Remove(x);
                        }
                    }
                }
            }
            SelectedTile.Hex.meshupdate(selectedMat);
        }
        else if (Selection != null)
        {
            Deselect();
            Select(Selection);
        }
    }

    //Deselects current tile
    public void Deselect()
    {
        if (SelectedObject != null)
        {
            if (movementRangeEnemy != null)
            {
                foreach (Tile tile in movementRangeEnemy)
                {
                    tile.Hex.meshupdate(tile.BaseMaterial);
                }
                movementRangeEnemy.Clear();
            }
            SelectedTile.Hex.meshupdate(SelectedTile.BaseMaterial);
            SelectedTile = null;
            SelectedContents = null;
            SelectedObject = null;
        }
    }

    public GameObject CurrentSelection()
    {
        return SelectedObject;
    }

}
