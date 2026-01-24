using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SimpleAI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CardBoard cardBoard;
    [SerializeField] private Hand opponentHand;
    [SerializeField] private GameLayout gameLayout;

    [Header("AI Settings")]
    [SerializeField] private float thinkingDelay = 1f; // Delay before AI makes a move

    private Coroutine currentMoveCoroutine;

    private void Start()
    {

        // Find references if not set
        if (cardBoard == null)
            cardBoard = FindObjectOfType<CardBoard>();

        if (gameLayout == null)
            gameLayout = FindObjectOfType<GameLayout>();
    }


    private void Update()
    {
        if (GameStateMachine.Instance == null)
            return;

        if (GameStateMachine.Instance.GetCurrentState() != GameState.OpponentTurn)
            return;

        if (currentMoveCoroutine == null)
            currentMoveCoroutine = StartCoroutine(MakeMove());
    }

    private IEnumerator MakeMove()
    {
        Debug.Log("========== AI MAKING MOVE ==========");

        // Double-check we're actually in opponent turn state
        if (GameStateMachine.Instance.GetCurrentState() != GameState.OpponentTurn)
        {
            Debug.LogWarning($"AI tried to move but state is: {GameStateMachine.Instance.GetCurrentState()}");
            currentMoveCoroutine = null;
            yield break;
        }

        // Wait a bit so it doesn't feel instant
        yield return new WaitForSeconds(thinkingDelay);

        // Check AGAIN after the delay - state might have changed
        if (GameStateMachine.Instance.GetCurrentState() != GameState.OpponentTurn)
        {
            Debug.LogWarning($"AI cancelled move - state changed during delay to: {GameStateMachine.Instance.GetCurrentState()}");
            currentMoveCoroutine = null;
            yield break;
        }

        // Get all available cards
        List<Card> availableCards = opponentHand.PlayerHand;

        Debug.Log($"AI Hand Count: {availableCards.Count}");
        Debug.Log($"AI Visualizers Count: {opponentHand.GetVisualisers().Count}");

        // Check what's actually in the hand
        for (int i = 0; i < availableCards.Count; i++)
        {
            Card card = availableCards[i];
            bool isActuallyCard = card is Card;
            string cardType = isActuallyCard ? "Card" : "NOT A CARD (SLATE?)";
            Debug.Log($"AI Hand[{i}]: {card.Name} - Type: {cardType}, Owner: {(isActuallyCard ? (card.Capture ? "Player" : "Opponent").ToString() : "N/A")}");
        }

        if (availableCards.Count == 0)
        {
            Debug.LogWarning("AI has no cards to play!");
            yield break;
        }

        // Pick a random card
        Card selectedCard = availableCards[Random.Range(0, availableCards.Count)];
        Debug.Log($"AI selected card: {selectedCard.Name}");

        // Find all empty positions on the board
        List<Vector2Int> emptyPositions = GetEmptyPositions();

        Debug.Log($"Empty positions found: {emptyPositions.Count}");
        foreach (var pos in emptyPositions)
        {
            Debug.Log($"  Empty at: {pos}");
        }

        if (emptyPositions.Count == 0)
        {
            Debug.LogWarning("No empty positions on board!");
            yield break;
        }

        // Pick a random empty position
        Vector2Int selectedPosition = emptyPositions[Random.Range(0, emptyPositions.Count)];
        Debug.Log($"AI selected position: {selectedPosition}");

        // Double-check the position is actually empty
        Slate occupant = cardBoard.GetCard(selectedPosition);
        if (occupant != null)
        {
            Debug.LogError($"ERROR: Position {selectedPosition} is NOT empty! Occupied by: {occupant.Name} (Type: {(occupant is Card ? "Card" : "Slate")})");
            currentMoveCoroutine = null;
            yield break;
        }

        // Get the visualiser for this card
        Visualiser cardVisual = GetVisualizerForCard(selectedCard);

        if (cardVisual == null)
        {
            Debug.LogError("Could not find visualiser for AI card!");
            currentMoveCoroutine = null;
            yield break;
        }

        CardSelect cardSelect = cardVisual.GetComponent<CardSelect>();

        if (cardSelect == null)
        {
            Debug.LogError("Card visualiser missing CardSelect component!");
            currentMoveCoroutine = null;
            yield break;
        }

        // FINAL CHECK: Are we still in OpponentTurn right before placing?
        if (GameStateMachine.Instance.GetCurrentState() != GameState.OpponentTurn)
        {
            Debug.LogWarning($"AI cancelled move - state is no longer OpponentTurn: {GameStateMachine.Instance.GetCurrentState()}");
            currentMoveCoroutine = null;
            yield break;
        }

        // FINAL CHECK: Use the state machine's validation
        if (!GameStateMachine.Instance.CanPlaceCard(selectedCard))
        {
            Debug.LogWarning($"AI cannot place card - failed CanPlaceCard validation");
            currentMoveCoroutine = null;
            yield break;
        }
        if (GameStateMachine.Instance.IsResolvingDraw)
        {
            currentMoveCoroutine = null;
            yield break;
        }

        // Place the card
        int snapIndex = GridPositionToSnapIndex(selectedPosition);
        Debug.Log($"Attempting to place at snap index: {snapIndex}");

        bool placed = gameLayout.TryPlaceCardAtIndex(
            snapIndex,
            selectedCard,
            cardSelect
        );

        if (placed)
        {
            Debug.Log($"✓ AI successfully placed {selectedCard.Name} at {selectedPosition}");
            cardSelect.enabled = false;
            gameLayout.RefreshHandPositions();
        }
        else
        {
            Debug.LogError($"✗ AI failed to place card at {selectedPosition}!");
        }

        currentMoveCoroutine = null;
    }

    private List<Vector2Int> GetEmptyPositions()
    {
        List<Vector2Int> emptyPositions = new List<Vector2Int>();

        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                Vector2Int pos = new Vector2Int(x, y);
                if (cardBoard.GetCard(pos) == null)
                {
                    emptyPositions.Add(pos);
                }
            }
        }

        return emptyPositions;
    }

    private Visualiser GetVisualizerForCard(Card card)
    {
        // Get all visualisers from the opponent's hand
        List<Visualiser> handVisuals = opponentHand.GetVisualisers();

        foreach (Visualiser visual in handVisuals)
        {
            if (visual.GetCard() == card)
            {
                return visual;
            }
        }

        return null;
    }

    private int GridPositionToSnapIndex(Vector2Int gridPos)
    {
        // Convert grid position (0-2, 0-2) to snap index (0-8)
        // Matches the mapping in GridSnapProvider
        return gridPos switch
        {
            { x: 0, y: 2 } => 0,
            { x: 1, y: 2 } => 1,
            { x: 2, y: 2 } => 2,
            { x: 0, y: 1 } => 3,
            { x: 1, y: 1 } => 4,
            { x: 2, y: 1 } => 5,
            { x: 0, y: 0 } => 6,
            { x: 1, y: 0 } => 7,
            { x: 2, y: 0 } => 8,
            _ => -1
        };
    }
}
