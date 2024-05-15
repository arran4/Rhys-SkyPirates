using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class HexSelect : MonoBehaviour
{
    public GameObject HighLightSelect { get; private set; } = null;
    public GameObject SelectedTile { get; private set; } = null;
    public Tile CSTile;
    public Material BaseMat, HighlightMat, selectedMat, ajacentMat;
    public BasicControls inputActions;

    // Start is called before the first frame update
    void Awake()
    {
        EventManager.OnTileSelect += Select;
        EventManager.OnTileDeselect += Deselect;
        EventManager.OnTileHover += setHighlight;
        inputActions = EventManager.EventInstance.inputActions;
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Mouse.current.position.ReadValue().x, Mouse.current.position.ReadValue().y, 0));
        RaycastHit hit;
       // bellow is clunky, refactor to use events?
        if (Physics.Raycast(ray, out hit, 100000f))
        {
            setHighlight(hit.transform.gameObject);
        }
        if (inputActions.Battle.MoveSelection.triggered)
        {
            HexMove(inputActions.Battle.MoveSelection.ReadValue<Vector2>());
        }
        if (inputActions.Battle.Select.triggered)
        {
            Select();
        }
        if (inputActions.Battle.Deselect.triggered)
        {
            Deselect();
        }
    }

    public void Select()
    {
        if (HighLightSelect != null && SelectedTile == null)
        {
            SelectedTile = HighLightSelect;
            SelectedTile.GetComponent<MeshRenderer>().material = selectedMat;
            foreach(Tile neighbour in SelectedTile.GetComponent<Tile>().Neighbours)
            {
                neighbour.gameObject.GetComponent<MeshRenderer>().material = ajacentMat;
            }
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
            foreach (Tile neighbour in SelectedTile.GetComponent<Tile>().Neighbours)
            {
                neighbour.gameObject.GetComponent<MeshRenderer>().material = BaseMat;
            }
            SelectedTile = null;
        }
    }

    private void OnDestroy()
    {
        EventManager.OnTileSelect -= Select;
        EventManager.OnTileDeselect -= Deselect;
    }

    private void setHighlight(GameObject Input)
    {
        if (HighLightSelect != Input && (Input != SelectedTile || SelectedTile == null))
        {
            if (HighLightSelect != null && HighLightSelect != SelectedTile)
            {
                HighLightSelect.GetComponent<MeshRenderer>().material = BaseMat;
            }
            HighLightSelect = Input;
            HighLightSelect.GetComponent<MeshRenderer>().material = HighlightMat;
        }
    }

    private void HexMove(Vector2 Input)
    {
        Tile check = HighLightSelect.GetComponent<Tile>().CheckNeighbours(Input);
        if (check != null)
        {
            setHighlight(check.gameObject);
        }
    }

}
