using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//Manager for the selection and highlight of hexes. Informs the current IHighlightResponce and ISelectionResponce when to
//act with given inputs.
[RequireComponent(typeof(RangeFinder))]
public class HexSelectManager : MonoBehaviour
{
    public static HexSelectManager HexSelectManagerInstance { get; private set; }
    public GameObject HighLightSelect { get; private set; } = null;
    public BasicControls inputActions;
    public ISelectionResponce Responce;
    public IHighlightResponce Highlight;
    public RangeFinder HighlightFinder;

    private HexSelectState currentState;
    private DefaultSelectState defaultState;
    private MoveSelectState moveSelectState;

    public void Awake()
    {
        if (HexSelectManagerInstance != null && HexSelectManagerInstance != this)
        {
            Destroy(this);
        }
        else
        {
            HexSelectManagerInstance = this;
        }
        defaultState = new DefaultSelectState();
        moveSelectState = new MoveSelectState();
        currentState = defaultState;
        currentState.EnterState(this);
    }

    void Start()
    {
        EventManager.OnTileSelect += Select;
        EventManager.OnTileDeselect += Deselcet;
        EventManager.OnTileHover += SetHighlight;
        inputActions = EventManager.EventInstance.inputActions;
        HighlightFinder = GetComponent<RangeFinder>();
    }

    void Update()
    {
        currentState.UpdateState(this);
    }

    public void Select()
    {
        Responce.Select(Highlight.ReturnHighlight());
    }
    
    public void Deselcet()
    {
        Responce.Deselect();
    }

    public void SetHighlight(GameObject ToHighlight)
    {
        Highlight.SetHighlight(ToHighlight);
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
        EventManager.OnTileDeselect -= Deselcet;
        EventManager.OnTileHover -= SetHighlight;
    }
}