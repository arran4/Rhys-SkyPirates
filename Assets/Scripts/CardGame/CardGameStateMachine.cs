using UnityEngine;
using System;

// ============================================================================
// STATE DEFINITIONS
// ============================================================================

public enum GameState
{
    GameStart,
    PlayerTurn,
    OpponentTurn,
    ProcessingCapture,
    GameOver,
    Draw
}

// ============================================================================
// GAME STATE MACHINE
// ============================================================================

public class GameStateMachine : MonoBehaviour
{
    public static GameStateMachine Instance { get; private set; }

    [Header("References")]
    [SerializeField] private CardBoard cardBoard;
    [SerializeField] private Hand playerHand;
    [SerializeField] private Hand opponentHand;
    [SerializeField] private GameLayout gameLayout;

    [Header("State")]
    [SerializeField] private GameState currentState;
    [SerializeField] private bool isPlayerTurn;
    private bool hasStarted = false;
    public bool IsResolvingDraw { get; private set; }
    private bool hasSwapped = false;


    // Events for UI and other systems to listen to
    public event Action<GameState> OnStateChanged;
    public event Action<bool> OnTurnChanged; // true = player turn, false = opponent turn
    public event Action<bool> OnGameEnd; // true = player won, false = opponent won

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        // Subscribe to card placement events
        if (cardBoard != null)
        {
            cardBoard.OnCardPlaced += OnCardPlacedHandler;
        }

        // Start the game
        ChangeState(GameState.GameStart);
    }

    private void OnDestroy()
    {
        if (cardBoard != null)
        {
            cardBoard.OnCardPlaced -= OnCardPlacedHandler;
        }
    }

    // ========================================================================
    // STATE MANAGEMENT
    // ========================================================================

    public void ChangeState(GameState newState)
    {
        if (IsResolvingDraw && newState != GameState.PlayerTurn && newState != GameState.OpponentTurn)
            return;
        if (currentState == newState)
            return;

        ExitState(currentState);
        currentState = newState;
        EnterState(newState);

        OnStateChanged?.Invoke(newState);
        Debug.Log($"State changed to: {newState}");
    }

    private void EnterState(GameState state)
    {
        switch (state)
        {
            case GameState.GameStart:
                StartGame();
                break;

            case GameState.PlayerTurn:
                isPlayerTurn = true;
                OnTurnChanged?.Invoke(true);
                EnablePlayerInput(true);
                break;

            case GameState.OpponentTurn:
                isPlayerTurn = false;
                OnTurnChanged?.Invoke(false);
                EnablePlayerInput(false);
                // TODO: Trigger AI opponent logic here
                break;

            case GameState.ProcessingCapture:
                EnablePlayerInput(false);
                break;

            case GameState.Draw:
                HandleDraw();
                break;

            case GameState.GameOver:
                EnablePlayerInput(false);
                break;
        }
    }

    private void ExitState(GameState state)
    {
        // Clean up when leaving a state if needed
    }

    // ========================================================================
    // GAME FLOW
    // ========================================================================

    private void StartGame()
    {
        if (hasStarted)
            return;

        hasStarted = true;
        if (GameRules.GameInstance != null && GameRules.GameInstance.IsSwap && !hasSwapped)
        {
            ExecuteSwap();
            hasSwapped = true;
        }

        isPlayerTurn = UnityEngine.Random.value > 0.5f;

        Debug.Log($"Game started! {(isPlayerTurn ? "Player" : "Opponent")} goes first");

        ChangeState(isPlayerTurn ? GameState.PlayerTurn : GameState.OpponentTurn);

    }


    private void OnCardPlacedHandler(Vector2Int position, Card card, CardSelect visualiser)
    {
        // Only process if we're in a turn state
        if (currentState != GameState.PlayerTurn && currentState != GameState.OpponentTurn)
        {
            Debug.LogWarning("Card placed outside of turn state!");
            return;
        }

        // Verify it's the correct player's turn
        if (card.Capture != isPlayerTurn)
        {
            Debug.LogWarning($"Wrong player tried to place a card! Expected: {isPlayerTurn}, Got: {card.Capture}");
            return;
        }

        // Move to processing state
        ChangeState(GameState.ProcessingCapture);

        // Process capture logic
        ProcessCapture(position, card);
    }

    // ========================================================================
    // PUBLIC API FOR VALIDATION
    // ========================================================================

    public bool CanPlaceCard(Card card)
    {
        // Can only place cards during a turn state
        if (currentState != GameState.PlayerTurn && currentState != GameState.OpponentTurn)
            return false;

        // Card ownership must match whose turn it is
        return card.Capture == isPlayerTurn;
    }

    private void ProcessCapture(Vector2Int position, Card card)
    {
        FinalizeTurn();
    }

    private void SwitchTurn()
    {
        isPlayerTurn = !isPlayerTurn;
        ChangeState(isPlayerTurn ? GameState.PlayerTurn : GameState.OpponentTurn);
    }

    private void HandleDraw()
    {
        Debug.Log("Draw! Redistributing cards and restarting...");

        IsResolvingDraw = true;

        cardBoard.RedistributeCards(playerHand, opponentHand);

        IsResolvingDraw = false;

        isPlayerTurn = UnityEngine.Random.value > 0.5f;
        ChangeState(isPlayerTurn ? GameState.PlayerTurn : GameState.OpponentTurn);
    }

    private void HandleGameEnd(bool playerWon)
    {
        Debug.Log($"Game Over! {(playerWon ? "Player" : "Opponent")} wins!");

        OnGameEnd?.Invoke(playerWon);
        ChangeState(GameState.GameOver);

        // TODO: Show win/loss UI
        // TODO: Offer rematch option
    }

    // ========================================================================
    // UTILITY
    // ========================================================================

    private void EnablePlayerInput(bool enabled)
    {
        // Enable/disable card selection for the player
        if (playerHand != null)
        {
            foreach (var visualiser in playerHand.GetVisualisers())
            {
                CardSelect cardSelect = visualiser.GetComponent<CardSelect>();
                if (cardSelect != null)
                {
                    cardSelect.enabled = enabled;
                }
            }
        }
    }

    public bool IsPlayerTurn()
    {
        return isPlayerTurn && currentState == GameState.PlayerTurn;
    }

    public GameState GetCurrentState()
    {
        return currentState;
    }

    // ========================================================================
    // PUBLIC API
    // ========================================================================

    public void RestartGame()
    {
        if (gameLayout == null)
            gameLayout = FindObjectOfType<GameLayout>();

        gameLayout.ClearLayout();
        cardBoard.ResetBoard();
        gameLayout.SpawnSlateVisual();

        playerHand.ResetHand();
        opponentHand.ResetHand();

        hasStarted = false;
        hasSwapped = false;
        ChangeState(GameState.GameStart);
    }
    private void FinalizeTurn()
    {
        bool playerWon;
        bool isDraw;

        if (cardBoard.CheckGameEnd(out playerWon, out isDraw))
        {
            if (isDraw)
            {
                ChangeState(GameState.Draw);
            }
            else
            {
                HandleGameEnd(playerWon);
            }

            return;
        }

        SwitchTurn();
    }
    private void ExecuteSwap()
    {
        if (playerHand == null || opponentHand == null)
            return;

        if (playerHand.PlayerHand.Count == 0 || opponentHand.PlayerHand.Count == 0)
            return;

        // Pick random cards
        int pIndex = UnityEngine.Random.Range(0, playerHand.PlayerHand.Count);
        int oIndex = UnityEngine.Random.Range(0, opponentHand.PlayerHand.Count);

        Card playerCard = playerHand.PlayerHand[pIndex];
        Card opponentCard = opponentHand.PlayerHand[oIndex];

        // Remove from hands (DO NOT destroy visuals)
        playerHand.RemoveCard(playerCard);
        opponentHand.RemoveCard(opponentCard);

        // Swap ownership
        playerCard.Capture = false;
        opponentCard.Capture = true;

        // Add back to opposite hands (visuals reused)
        playerHand.AddCard(opponentCard);
        opponentHand.AddCard(playerCard);

        Debug.Log($"Swap executed: Player ↔ Opponent" + opponentCard.Name + playerCard.Name);
    }

}
