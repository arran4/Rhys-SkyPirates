using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISelectionResponce
{
    public void Select(GameObject Selection);
    public void Deselect();

    public GameObject CurrentSelection();
}
