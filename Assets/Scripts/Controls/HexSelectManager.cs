using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class HexSelectManager : MonoBehaviour
{
    public GameObject HighLightSelect { get; private set; } = null;
    public Material HighlightMat;
    public BasicControls inputActions;
    public ISelectionResponce Responce;
    public IHighlightResponce Highlight;

    // Start is called before the first frame update
    void Awake()
    {
        Responce = GetComponent<ISelectionResponce>();
        Highlight = GetComponent<IHighlightResponce>();
        EventManager.OnTileSelect += Responce.Select;
        EventManager.OnTileDeselect += Responce.Deselect;
        EventManager.OnTileHover += Highlight.SetHighlight;
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
            Highlight.SetHighlight(hit.transform.gameObject);
        }
        if (inputActions.Battle.MoveSelection.triggered)
        {
            Highlight.MoveHighlight(inputActions.Battle.MoveSelection.ReadValue<Vector2>());
        }
        if (inputActions.Battle.Select.triggered)
        {
            Responce.Select(Highlight.ReturnHighlight());
        }
        if (inputActions.Battle.Deselect.triggered)
        {
            Responce.Deselect();
        }
    }

    private void OnDestroy()
    {
        EventManager.OnTileSelect -= Responce.Select;
        EventManager.OnTileDeselect -= Responce.Deselect;
        EventManager.OnTileHover -= Highlight.SetHighlight;
    }
}
