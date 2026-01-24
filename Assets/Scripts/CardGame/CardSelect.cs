using UnityEngine;
using UnityEngine.InputSystem;

public class CardSelect : MonoBehaviour
{
    private static CardSelect currentlyHeldCard = null;
    private bool isHeld = false;
    private Vector3 offset;
    private Camera mainCamera;
    private SpriteRenderer spriteRenderer;
    private int originalSortingOrder;
    private Vector3 originalPosition;
    private Transform originalParent;

    [Header("Grid Snapping")]
    [SerializeField] private float snapDistance = 0.6f;
    [SerializeField] private GridSnapProvider snapProvider;

    [Header("References")]
    [SerializeField] private CardBoard cardBoard;
    [SerializeField] private GameLayout gameLayout;

    private float gridCellWidth;
    private float gridCellHeight;

    // Reference to the card data this visualiser represents
    private Card cardData;

    void Start()
    {
        mainCamera = Camera.main;
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalSortingOrder = spriteRenderer.sortingOrder;

        // Find references if not set
        if (cardBoard == null)
        {
            cardBoard = FindObjectOfType<CardBoard>();
        }

        if (gameLayout == null)
        {
            gameLayout = FindObjectOfType<GameLayout>();
        }
        if(snapProvider == null)
        {
            snapProvider = FindObjectOfType<GridSnapProvider>();
        }

        // Calculate grid cell size from this card's own size
        CalculateGridCellSize();
    }

    // Method to set the card data this visualiser represents
    public void SetCardData(Card card)
    {
        cardData = card;
    }

    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (!isHeld &&
                currentlyHeldCard == null &&
                GetComponent<Collider2D>().OverlapPoint(GetMouseWorldPos()))
            {
                // CHECK IF WE CAN PICK UP THIS CARD
                if (GameStateMachine.Instance != null && !GameStateMachine.Instance.CanPlaceCard(cardData))
                {
                    Debug.LogWarning("Not your turn!");
                    return;
                }

                isHeld = true;
                currentlyHeldCard = this;
                originalPosition = transform.position;
                originalParent = transform.parent;
                offset = transform.position - GetMouseWorldPos();
                spriteRenderer.sortingOrder = 100;
            }
            else if (isHeld)
            {
                isHeld = false;
                currentlyHeldCard = null;
                spriteRenderer.sortingOrder = originalSortingOrder;
                TrySnapOrReturn();
            }
        }

        if (isHeld)
        {
            transform.position = GetMouseWorldPos() + offset;
        }
    }

    private void CalculateGridCellSize()
    {
        // Use the card's own renderer bounds to determine grid spacing
        Bounds bounds = spriteRenderer.bounds;
        gridCellWidth = bounds.size.x;
        gridCellHeight = bounds.size.y;

    }

    private void TrySnapOrReturn()
    {
        float distance;
        int snapIndex = snapProvider.GetClosestSnapIndex(
            transform.position,
            out distance
        );

        if (snapIndex < 0 || distance > snapDistance)
        {
            ReturnToOriginalPosition();
            return;
        }

        // CHECK WITH STATE MACHINE FIRST
        if (GameStateMachine.Instance != null && !GameStateMachine.Instance.CanPlaceCard(cardData))
        {
            Debug.LogWarning("Cannot place card - not your turn!");
            ReturnToOriginalPosition();
            return;
        }

        Vector3 snapPos = snapProvider.GetSnapWorldPosition(snapIndex);

        // Ask GameLayout / Board to attempt placement
        bool placed = gameLayout.TryPlaceCardAtIndex(
            snapIndex,
            cardData,
            this
        );

        if (placed)
        {
            enabled = false;
            gameLayout.RefreshHandPositions();
        }
        else
        {
            ReturnToOriginalPosition();
        }
    }

    private void ReturnToOriginalPosition()
    {
        transform.position = originalPosition;
        transform.SetParent(originalParent);
        Debug.Log($"Returned to original position, closest was too far or invalid");
    }

    private Vector3 GetClosestGridPosition(Vector3 worldPos, out float closestDistance)
    {
        Vector3 closest = Vector3.zero;
        closestDistance = float.MaxValue;

        // Check all 9 grid positions
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                Vector3 gridPos = new Vector3(
                    x * gridCellWidth,
                    y * gridCellHeight,
                    0
                );

                float dist = Vector3.Distance(worldPos, gridPos);
                if (dist < closestDistance)
                {
                    closestDistance = dist;
                    closest = gridPos;
                }
            }
        }

        return closest;
    }

    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePos = Mouse.current.position.ReadValue();
        mousePos.z = mainCamera.WorldToScreenPoint(transform.position).z;
        Vector3 pos = mainCamera.ScreenToWorldPoint(mousePos);
        pos.z = 0;
        return pos;
    }


}
