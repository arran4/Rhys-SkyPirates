using UnityEngine;
using System.Collections.Generic;

public class GameLayout : MonoBehaviour
{
    [Header("Hand References")]
    [SerializeField]
    private Hand playerHand;

    [SerializeField]
    private Hand enemyHand;

    [Header("Board Reference")]
    [SerializeField]
    private CardBoard cardBoard;

    [Header("Layout Settings")]
    [SerializeField]
    private float handCardSpacing = 1.5f;

    [SerializeField]
    private float handYOffset = 4f;

    [Header("Visualiser Prefab")]
    [SerializeField]
    private Visualiser slateVisualPrefab;

    [Header("Center Marker")]
    [SerializeField]
    private GameObject centerMarkerPrefab;

    private GameObject centerMarker;

    [SerializeField]
    private GridSnapProvider snapProvider;

    private readonly Dictionary<Slate, Visualiser> cardVisuals
    = new Dictionary<Slate, Visualiser>();


    public void Start()
    {
        if (cardBoard != null)
        {
            cardBoard.OnBoardTransformed += UpdateGridVisuals;
            cardBoard.OnCardPlaced += OnCardPlaced; // SUBSCRIBE TO CARD PLACEMENT
        }

        Invoke(nameof(PositionHands), 0.2f);

        // Spawn center marker
        if (centerMarkerPrefab != null)
        {
            Vector3 centerPosition = new Vector3(0, 0, 1); // Center of 3x3 grid
            centerMarker = Instantiate(centerMarkerPrefab, centerPosition, Quaternion.identity, transform);
        }
        SpawnSlateVisual();
    }

    private void OnDestroy()
    {
        if (cardBoard != null)
        {
            cardBoard.OnBoardTransformed -= UpdateGridVisuals;
            cardBoard.OnCardPlaced -= OnCardPlaced;
        }
    }

    // NEW: Handle card placement - move the visualizer immediately
    private void OnCardPlaced(Vector2Int position, Card card, CardSelect visualiser)
    {
        if (GameStateMachine.Instance != null &&
    GameStateMachine.Instance.IsResolvingDraw)
        {
            return;
        }
        if (visualiser == null)
        {
            Debug.LogError("OnCardPlaced received null visualiser!");
            return;
        }

        // CRITICAL: Check if the card is still at this position on the board
        // (it might have been collected during a draw redistribution)
        Slate currentOccupant = cardBoard.GetCard(position);
        if (currentOccupant != card)
        {
            Debug.LogWarning($"OnCardPlaced fired for {card.Name} at {position}, but card is no longer there. Skipping visual move.");
            return;
        }

        // Move the visualizer to the board position
        Vector3 worldPos = snapProvider.GetWorldPosition(position);
        visualiser.transform.position = worldPos;
        visualiser.transform.SetParent(transform); // Parent to GameLayout (the board)

        Debug.Log($"Moved visualizer for {card.Name} to position {position} at {worldPos}");
    }

    public void UpdateGridVisuals()
    {
        foreach (var kvp in cardVisuals)
        {
            Slate card = kvp.Key;
            Visualiser visual = kvp.Value;

            // Ask the board where this card is
            if (!cardBoard.TryGetCardPosition(card, out Vector2Int gridPos))
                continue;

            // Ask the snap provider where that grid position is visually
            Vector3 worldPos = snapProvider.GetWorldPosition(gridPos);

            visual.transform.position = worldPos;
        }
        Debug.Log($"UpdateGridVisuals called. Registered visuals: {cardVisuals.Count}");
    }

    public void PositionHands()
    {
        if (playerHand != null)
        {
            PositionHand(playerHand, -handYOffset);
        }

        if (enemyHand != null)
        {
            PositionHand(enemyHand, handYOffset);
        }
    }

    public void PositionHand(Hand hand, float yPosition)
    {
        List<Visualiser> visualisers = hand.GetVisualisers();
        int cardCount = visualisers.Count;

        float totalWidth = (cardCount - 1) * handCardSpacing;
        float startX = -totalWidth / 2f;

        for (int i = 0; i < cardCount; i++)
        {
            Vector3 cardPosition = new Vector3(
                startX + (i * handCardSpacing),
                yPosition,
                0
            );

            visualisers[i].transform.position = cardPosition;
            visualisers[i].transform.SetParent(hand.transform);
        }
    }

    public void RefreshHandPositions()
    {
        PositionHands();
    }

    public bool TryPlaceCardAtIndex(int snapIndex, Card card, CardSelect visualiser)
    {
        Vector2Int gridPos = SnapIndexToGrid(snapIndex);
        return cardBoard.PlaceCard(gridPos, card, visualiser);
    }

    private Vector2Int SnapIndexToGrid(int index)
    {
        // Visual top-left = index 0
        return index switch
        {
            0 => new Vector2Int(0, 2),
            1 => new Vector2Int(1, 2),
            2 => new Vector2Int(2, 2),

            3 => new Vector2Int(0, 1),
            4 => new Vector2Int(1, 1),
            5 => new Vector2Int(2, 1),

            6 => new Vector2Int(0, 0),
            7 => new Vector2Int(1, 0),
            8 => new Vector2Int(2, 0),

            _ => new Vector2Int(-1, -1)
        };
    }

    public void RegisterVisual(Slate card, Visualiser visual)
    {
        if (!cardVisuals.ContainsKey(card))
        {
            cardVisuals.Add(card, visual);
        }
    }

    public void SpawnSlateVisual()
    {
        Vector2Int slatePos = cardBoard.FindSlate();
        Slate slate = cardBoard.GetCard(slatePos);

        if (slate == null)
        {
            Debug.LogError("Slate not found on board");
            return;
        }

        Visualiser visual = Instantiate(slateVisualPrefab);
        visual.Setup(slate);
        visual.GetComponent<CardSelect>().enabled = false;

        visual.transform.SetParent(transform);

        RegisterVisual(slate, visual);
        UpdateGridVisuals();
    }

    public Visualiser GetVisualForCard(Slate card)
    {
        if (cardVisuals.TryGetValue(card, out Visualiser visual))
        {
            return visual;
        }
        return null;
    }
    public void ReclaimAllBoardVisuals()
    {
        foreach (var kvp in cardVisuals)
        {
            Visualiser visual = kvp.Value;
            if (visual == null)
                continue;

            visual.transform.SetParent(transform);
        }
    }

    public void ClearAllVisuals()
    {
        foreach (var visual in cardVisuals.Values)
        {
            if (visual != null)
                Destroy(visual.gameObject);
        }
        cardVisuals.Clear();
    }

    public void ResetLayout()
    {
        ClearAllVisuals();
        SpawnSlateVisual();
    }
}
