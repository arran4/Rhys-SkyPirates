using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHighlightResponce
{
    public void SetHighlight(GameObject Highlight);
    public void MoveHighlight(Vector2 Input);

    public GameObject ReturnHighlight();
}
