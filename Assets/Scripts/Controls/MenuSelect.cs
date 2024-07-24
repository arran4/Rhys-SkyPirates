using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSelect : MonoBehaviour, ISelectionResponce
{
    public GameObject CurrentSelection()
    {
        throw new System.NotImplementedException();
    }

    public void Deselect()
    {
        HexSelectManager.Instance.SwitchToDefaultState();
    }

    public void Select(GameObject Selection)
    {
        throw new System.NotImplementedException();
    }

}
