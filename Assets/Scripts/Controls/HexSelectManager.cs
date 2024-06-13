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

    private HexSelectState currentState;
    private DefaultSelectState defaultState;
    private MoveSelectState moveSelectState;

    public void Awake()
    {
        defaultState = new DefaultSelectState();
        moveSelectState = new MoveSelectState();
        currentState = defaultState;
        currentState.EnterState(this);
    }

    void Start()
    {
        EventManager.OnTileSelect += Select;
        EventManager.OnTileDeselect += Responce.Deselect;
        EventManager.OnTileHover += Highlight.SetHighlight;
        inputActions = EventManager.EventInstance.inputActions;


    }

    void Update()
    {
        currentState.UpdateState(this);
    }

    public void Select()
    {
        Responce.Select(Highlight.ReturnHighlight());
    }

    public void SwitchToMoveSelectState()
    {
        currentState.ExitState(this);
        currentState = moveSelectState;
        currentState.EnterState(this);
    }

    public void SwitchToDefaultState()
    {
        currentState.ExitState(this);
        currentState = defaultState;
        currentState.EnterState(this);
    }

    private void OnDestroy()
    {
        EventManager.OnTileSelect -= Select;
        EventManager.OnTileDeselect -= Responce.Deselect;
        EventManager.OnTileHover -= Highlight.SetHighlight;
    }
}