using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexSelection : MonoBehaviour, ISelectionResponce
{
    public Material selectedMat;
    public GameObject SelectedTile { get; private set; } = null;


    public void Update()
    {
        if (SelectedTile != null)
        {
            SelectedTile.GetComponent<MeshRenderer>().material = selectedMat;
        }
    }
    public void Select(GameObject Selection)
    {
        if (Selection != null && SelectedTile == null)
        {
            SelectedTile = Selection;
            SelectedTile.GetComponent<MeshRenderer>().material = selectedMat;
        }
        else if (Selection != null)
        {
            Deselect();
            Select(Selection);
        }
    }

    public void Deselect()
    {
        if (SelectedTile != null)
        {
            SelectedTile.GetComponent<MeshRenderer>().material = SelectedTile.GetComponent<Tile>().BaseMat;
            SelectedTile = null;
        }
    }

    public GameObject CurrentSelection()
    {
        return SelectedTile;
    }

}
