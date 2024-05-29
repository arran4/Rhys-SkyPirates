using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Base highlight implementation.
public class HexHighlight : MonoBehaviour, IHighlightResponce
{
    private GameObject HighLightSelect = null;
    private Tile HighlightTile = null;
    public Material HighlightMat;

    //Sets the Highlight
    public void SetHighlight(GameObject Input)
    {
        if (HighLightSelect != Input)
        {
            if (HighLightSelect != null)
            {
                HighlightTile.Hex.meshupdate(HighlightTile.BaseMaterial);
            }
            HighLightSelect = Input;
            HighlightTile = HighLightSelect.GetComponent<Tile>();
            if(HighlightTile == null)
            {
                HighlightTile = HighLightSelect.GetComponent<Pawn>().Position;
            }
            HighlightTile.Hex.meshupdate(HighlightMat);
        }
    }

    //Finds the Tile in the direction relitive to the camera and moves the highlight 1 space.
    public void MoveHighlight(Vector2 Input)
    {
        Tile check = HighlightTile.CheckNeighbours(Input);
        if (check != null)
        {
            SetHighlight(check.gameObject);
        }
    }

    //Returns the current highlight
    public GameObject ReturnHighlight()
    {
        return HighLightSelect;
    }

}
