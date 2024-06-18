using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSelect : MonoBehaviour, ISelectionResponce
{
    public Material HighlightMat;
    public GameObject SelectedObject { get; private set; } = null;
    public GameObject CurrentSelection()
    {
        return SelectedObject;
    }

    public void Deselect()
    {
        HexSelectManager.HexSelectManagerInstance.SwitchToDefaultState();
    }

    public void Select(GameObject Selection)
    {
        throw new System.NotImplementedException();
    }

 
}
