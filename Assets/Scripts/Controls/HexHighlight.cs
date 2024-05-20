using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexHighlight : MonoBehaviour, IHighlightResponce
{
    public GameObject HighLightSelect { get; private set; } = null;
    public Material HighlightMat;

    public void SetHighlight(GameObject Input)
    {
        if (HighLightSelect != Input)
        {
            if (HighLightSelect != null)
            {
                HighLightSelect.GetComponent<MeshRenderer>().material = HighLightSelect.GetComponent<Tile>().BaseMat;
            }
            HighLightSelect = Input;
            HighLightSelect.GetComponent<MeshRenderer>().material = HighlightMat;
        }
    }

    public void MoveHighlight(Vector2 Input)
    {
        Tile check = HighLightSelect.GetComponent<Tile>().CheckNeighbours(Input);
        if (check != null)
        {
            SetHighlight(check.gameObject);
        }
    }

    public GameObject ReturnHighlight()
    {
        return HighLightSelect;
    }

}
