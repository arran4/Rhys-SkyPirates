using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSelect : MonoBehaviour, ISelectionResponce
{
    public Material selectedMat;
    public GameObject SelectedObject { get; private set; } = null;
    public Tile SelectedTile { get; private set; } = null;
    public Pawn SelectedContents { get; private set; } = null;

    public int CharaterNo;

    private bool isReselecting = false;

    public void Start()
    {
        CharaterNo = -1;
    }

    public GameObject CurrentSelection()
    {
        return SelectedObject;
    }

    public void Deselect()
    {
        if (SelectedObject != null)
        {
            SelectedTile.Hex.meshupdate(SelectedTile.BaseMaterial);
            SelectedTile = null;
            SelectedContents = null;
            SelectedObject = null;
        }
        if (CharaterNo == -1 && !isReselecting)
        {
            HexSelectManager.Instance.SwitchToDefaultState();
        }
    }

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
            }
            SelectedTile.Hex.meshupdate(selectedMat);
        }
        else if (Selection != null)
        {
            SelectedTile = SelectedObject.GetComponent<Tile>();
            SelectedContents = SelectedTile.Contents;
            if (SelectedContents == null || !(SelectedContents is PlayerPawns))
            {
                CharaterNo = -1;
            }
            else
            {
                int index = 0;
                foreach (PlayerPawns a in PawnManager.PawnManagerInstance.PlayerPawns)
                {
                    if (a == SelectedContents)
                    {
                        CharaterNo = index;
                    }
                    index++;
                }
            }

            // Set the reselecting flag to true before deselecting
            isReselecting = true;
            Deselect();
            // Set the reselecting flag back to false after deselecting
            isReselecting = false;

            Select(Selection);
            CharaterNo = -1;
        }
    }
}