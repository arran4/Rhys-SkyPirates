using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class HexSelect : MonoBehaviour
{
    public GameObject HighLightSelect { get; private set; } = null;
    public GameObject SelectedTile { get; private set; } = null;
    public Tile CSTile;
    public Material BaseMat, HighlightMat, selectedMat;
    
    // Start is called before the first frame update
    void Start()
    {
        EventManager.OnTileSelect += Select;
        EventManager.OnTileDeselect += Deselect;
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Mouse.current.position.ReadValue().x, Mouse.current.position.ReadValue().y, 0));
        RaycastHit hit;
       // bellow is clunky, refactor to use events?
        if (Physics.Raycast(ray, out hit, 100000f))
        {
            if (HighLightSelect != hit.transform.gameObject && (hit.transform.gameObject != SelectedTile || SelectedTile == null))
            {
                if(HighLightSelect != null && HighLightSelect != SelectedTile)
                {
                    HighLightSelect.GetComponent<MeshRenderer>().material = BaseMat;
                }
                HighLightSelect = hit.transform.gameObject;
                HighLightSelect.GetComponent<MeshRenderer>().material = HighlightMat;
            }
           // for clarity in testing, remove later.
            Debug.DrawLine(ray.origin, ray.direction * 100000, Color.red, 5);
            Debug.Log(hit.transform.name);
            Debug.Log("hit");
        }
    }

    public void Select()
    {
        if (HighLightSelect != null && SelectedTile == null)
        {
            SelectedTile = HighLightSelect;
            SelectedTile.GetComponent<MeshRenderer>().material = selectedMat;
        }
        else if(HighLightSelect != null)
        {
            Deselect();
            Select();
        }
    }

    public void Deselect()
    {
        if (SelectedTile != null)
        {
            SelectedTile.GetComponent<MeshRenderer>().material = BaseMat;
            SelectedTile = null;
        }
    }

    private void OnDestroy()
    {
        EventManager.OnTileSelect -= Select;
        EventManager.OnTileDeselect -= Deselect;
    }

}
