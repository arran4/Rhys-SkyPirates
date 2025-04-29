using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Commits the selected tile edit
public class BuildSelect : MonoBehaviour, ISelectionResponce
{
    public Material selectedEditMaterial; // Material to apply on select

    private TileDataSO EditTile;

    private GameObject selectedObject = null;
    private Tile selectedTile = null;

    public void Start()
    {
        EventManager.OnTileChange += ChangeTile;
    }

    public void Select(GameObject selection)
    {
        if (selection != null)
        {
            selectedObject = selection;
            selectedTile = selectedObject.GetComponent<Tile>();

            if (selectedTile != null)
            {
                selectedTile.Hex.meshupdate(selectedEditMaterial);
                selectedTile.BaseMaterial = selectedEditMaterial; // Make change permanent
            }
        }
    }

    public void Deselect()
    {
        selectedObject = null;
        selectedTile = null;
    }

    public GameObject CurrentSelection()
    {
        return selectedObject;
    }

    public void ChangeTile(TileDataSO tile)
    {
        EditTile = tile;
    }

    public void OnDestroy()
    {
        EventManager.OnTileChange -= ChangeTile;
    }
}
