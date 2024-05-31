using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//Manager for the selection and highlight of hexes. Informs the current IHighlightResponce and ISelectionResponce when to
//act with given inputs.
public class HexSelectManager : MonoBehaviour
{
    public GameObject HighLightSelect { get; private set; } = null;
    public BasicControls inputActions;
    public ISelectionResponce Responce;
    public IHighlightResponce Highlight;

    void Awake()
    {
        Responce = GetComponent<ISelectionResponce>();
        Highlight = GetComponent<IHighlightResponce>();
        EventManager.OnTileSelect += Select;
        EventManager.OnTileDeselect += Responce.Deselect;
        EventManager.OnTileHover += Highlight.SetHighlight;
        inputActions = EventManager.EventInstance.inputActions;
    }

    void Update()
    {
        //If possible need to move most of this to a player controls script. This should honestly be assigning events to the highlight and select scripts.
        //It may also call specific methods based on the type of highlighter or selector.
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
        {
            Highlight.SetHighlight(hit.transform.gameObject);
        }
        if (inputActions.Battle.MoveSelection.triggered)
        {
            Highlight.MoveHighlight(inputActions.Battle.MoveSelection.ReadValue<Vector2>());
        }
        if (inputActions.Battle.Select.triggered)
        {
            Select();
        }
        if (inputActions.Battle.Deselect.triggered)
        {
            Responce.Deselect();
        }
    }

    private void Select()
    {
        Responce.Select(Highlight.ReturnHighlight());
    }

    private void OnDestroy()
    {
        EventManager.OnTileSelect -= Select;
        EventManager.OnTileDeselect -= Responce.Deselect;
        EventManager.OnTileHover -= Highlight.SetHighlight;
    }
}
