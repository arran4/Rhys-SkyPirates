using UnityEngine;
using System.Collections.Generic;

public class CardBoard : MonoBehaviour
{
    // ========================================================================
    // EVENTS
    // ========================================================================

    public event System.Action OnBoardTransformed;
    public event System.Action<Vector2Int, Card, CardSelect> OnCardPlaced;

    // ========================================================================
    // ENUMS
    // ========================================================================

    public enum TransformationType
    {
        RotateClockwise,
        RotateCounterclockwise,
        FlipHorizontal,
        FlipVertical
    }

    // ========================================================================
    // CONFIG
    // ========================================================================

    [Header("Grid Configuration")]
    [SerializeField] private float gridCellWidth = 2.5f;
    [SerializeField] private float gridCellHeight = 3.5f;

    [Header("Slate Configuration")]
    [SerializeField] private SOCard slate;

    private const int GRID_SIZE = 3;
    private const int MAX_CARDS = 8;

    // ========================================================================
    // STATE
    // ========================================================================

    private Slate[,] board;
    private CardSelect[,] visuals;

    private Slate slateInstance;
    private Vector2Int slatePosition;

    // ========================================================================
    // INITIALIZATION
    // ========================================================================

    private void Awake()
    {
        board = new Slate[GRID_SIZE, GRID_SIZE];
        visuals = new CardSelect[GRID_SIZE, GRID_SIZE];
        PlaceSlateRandomly();
    }

    private void PlaceSlateRandomly()
    {
        slatePosition = new Vector2Int(Random.Range(0, GRID_SIZE), Random.Range(0, GRID_SIZE));
        slateInstance = new Slate(slate);
        board[slatePosition.x, slatePosition.y] = slateInstance;
    }

    // ========================================================================
    // COORDINATES
    // ========================================================================

    public Vector2Int WorldToGrid(Vector3 worldPosition)
    {
        int x = Mathf.RoundToInt(worldPosition.x / gridCellWidth) + 1;
        int y = Mathf.RoundToInt(worldPosition.y / gridCellHeight) + 1;
        return IsValidPosition(x, y) ? new Vector2Int(x, y) : new Vector2Int(-1, -1);
    }

    public Vector3 GridToWorld(Vector2Int gridPosition)
    {
        return new Vector3(
            (gridPosition.x - 1) * gridCellWidth,
            (gridPosition.y - 1) * gridCellHeight,
            0f
        );
    }

    // ========================================================================
    // PLACEMENT
    // ========================================================================

    public bool PlaceCard(Vector2Int position, Card card, CardSelect visual)
    {
        if (GameStateMachine.Instance.IsResolvingDraw)
            return false;

        if (!IsEmpty(position))
            return false;

        SetCell(position, card, visual);

        ExecuteCardEffects(card, position);
        RemoveCardFromOwnerHand(card);

        OnCardPlaced?.Invoke(position, card, visual);
        return true;
    }

    // ========================================================================
    // EFFECT ORDER
    // ========================================================================

    private void ExecuteCardEffects(Card card, Vector2Int position)
    {
        Vector2Int CardExecutePosition = position;
        if (GameRules.GameInstance.IsBeforeCapture)
        {
            ExecuteEffect(card.Effect, card.SlateTarget, position);
            TryGetCardPosition(card, out CardExecutePosition);
        }

        ExecuteCapture(CardExecutePosition, card);

        if (!GameRules.GameInstance.IsBeforeCapture)
            ExecuteEffect(card.Effect, card.SlateTarget, position);
    }

    // ========================================================================
    // BOARD ACCESS
    // ========================================================================

    public Slate GetCard(Vector2Int pos) => IsValidPosition(pos) ? board[pos.x, pos.y] : null;
    public CardSelect GetVisual(Vector2Int pos) => IsValidPosition(pos) ? visuals[pos.x, pos.y] : null;

    public bool IsValidPosition(Vector2Int p) => IsValidPosition(p.x, p.y);
    private bool IsValidPosition(int x, int y) => x >= 0 && x < GRID_SIZE && y >= 0 && y < GRID_SIZE;

    private bool IsEmpty(Vector2Int p) => IsValidPosition(p) && board[p.x, p.y] == null;

    private void SetCell(Vector2Int p, Slate s, CardSelect v)
    {
        board[p.x, p.y] = s;
        visuals[p.x, p.y] = v;
    }
    public CardSelect GetVisualiser(Vector2Int pos)
    {
        return IsValidPosition(pos) ? visuals[pos.x, pos.y] : null;
    }

    // ========================================================================
    // ADJACENCY
    // ========================================================================

    private static readonly Vector2Int[] Directions =
    {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.right,
        Vector2Int.left
    };

    public Vector2Int[] GetAdjacentPositions(Vector2Int pos)
    {
        List<Vector2Int> list = new();
        foreach (var d in Directions)
        {
            Vector2Int p = pos + d;
            if (IsValidPosition(p)) list.Add(p);
        }
        return list.ToArray();
    }

    // ========================================================================
    // CAPTURE SYSTEM (REFACTORED INTERNALLY)
    // ========================================================================

    public void ExecuteCapture(Vector2Int gridPosition, Card playedCard)
    {
        List<Vector2Int> comboSeeds = new List<Vector2Int>();

        // SAME
        if (GameRules.GameInstance.IsSame)
        {
            comboSeeds.AddRange(ResolveSameCaptures(gridPosition, playedCard));
        }

        // PLUS
        if (GameRules.GameInstance.IsPlus)
        {
            comboSeeds.AddRange(
                ResolvePlusCaptures(gridPosition, playedCard)
            );
        }
        // Standard captures (DO NOT seed combo)
        Vector2Int[] adjacentPositions = GetAdjacentPositions(gridPosition);
        foreach (Vector2Int adjPos in adjacentPositions)
        {
            TryStandardCapture(playedCard, gridPosition, adjPos);
        }

        //  Combo (ONLY from SAME / PLUS )
        if (GameRules.GameInstance.IsCombo && comboSeeds.Count > 0)
        {
            ResolveComboCaptures(comboSeeds, playedCard.Capture);
        }
    }

    private List<Vector2Int> ResolveStandardCaptures(Vector2Int playedPos, Card playedCard)
    {
        List<Vector2Int> captured = new();

        foreach (Vector2Int adj in GetAdjacentPositions(playedPos))
        {
            if (TryStandardCapture(playedCard, playedPos, adj))
                captured.Add(adj);
        }

        return captured;
    }

    private List<Vector2Int> ResolveSameCaptures(Vector2Int playedPos, Card playedCard)
    {
        List<Vector2Int> sameCaptured = new List<Vector2Int>();

        Vector2Int[] adjacents = GetAdjacentPositions(playedPos);

        // SAME requires at least 2 adjacent cards
        if (adjacents.Length < 2)
            return sameCaptured;

        for (int i = 0; i < adjacents.Length; i++)
        {
            for (int j = i + 1; j < adjacents.Length; j++)
            {
                Vector2Int posA = adjacents[i];
                Vector2Int posB = adjacents[j];

                if (!(GetCard(posA) is Card cardA) || !(GetCard(posB) is Card cardB))
                    continue;

                // At least one must be opponent-owned
                if (cardA.Capture == playedCard.Capture &&
                    cardB.Capture == playedCard.Capture)
                    continue;

                int playedRankA = GetRankForDirection(playedCard, playedPos, posA);
                int targetRankA = GetRankForDirection(cardA, posA, playedPos);

                int playedRankB = GetRankForDirection(playedCard, playedPos, posB);
                int targetRankB = GetRankForDirection(cardB, posB, playedPos);

                if (playedRankA == targetRankA &&
                    playedRankB == targetRankB)
                {
                    if (cardA.Capture != playedCard.Capture)
                    {
                        CaptureCard(posA, playedCard.Capture);
                        sameCaptured.Add(posA);
                    }

                    if (cardB.Capture != playedCard.Capture)
                    {
                        CaptureCard(posB, playedCard.Capture);
                        sameCaptured.Add(posB);
                    }
                }
            }
        }

        return sameCaptured;
    }

    private List<Vector2Int> ResolvePlusCaptures(Vector2Int playedPos, Card playedCard)
    {
        List<Vector2Int> plusSeeds = new();

        Vector2Int[] adj = GetAdjacentPositions(playedPos);

        // PLUS needs at least two adjacent cards
        if (adj.Length < 2)
            return plusSeeds;

        for (int i = 0; i < adj.Length; i++)
        {
            for (int j = i + 1; j < adj.Length; j++)
            {
                Vector2Int a = adj[i];
                Vector2Int b = adj[j];

                if (GetCard(a) is not Card cardA)
                    continue;
                if (GetCard(b) is not Card cardB)
                    continue;

                // At least one must be opponent-owned
                if (cardA.Capture == playedCard.Capture &&
                    cardB.Capture == playedCard.Capture)
                    continue;

                int sumA =
                    GetRankForDirection(playedCard, playedPos, a) +
                    GetRankForDirection(cardA, a, playedPos);

                int sumB =
                    GetRankForDirection(playedCard, playedPos, b) +
                    GetRankForDirection(cardB, b, playedPos);

                if (sumA == sumB)
                {
                    if (cardA.Capture != playedCard.Capture)
                    {
                        CaptureCard(a, playedCard.Capture);
                        plusSeeds.Add(a);
                    }

                    if (cardB.Capture != playedCard.Capture)
                    {
                        CaptureCard(b, playedCard.Capture);
                        plusSeeds.Add(b);
                    }
                }
            }
        }

        return plusSeeds;
    }


    private bool TryStandardCapture(Card playedCard, Vector2Int from, Vector2Int to)
    {
        if (GetCard(to) is not Card target)
            return false;

        if (target.Capture == playedCard.Capture)
            return false;

        int playedRank = GetRankForDirection(playedCard, from, to);
        int targetRank = GetRankForDirection(target, to, from);

        if (playedRank <= targetRank)
            return false;

        CaptureCard(to, playedCard.Capture);
        return true;
    }

    private void ResolveComboCaptures(List<Vector2Int> initial, bool capturingPlayer)
    {
        Queue<Vector2Int> queue = new(initial);
        HashSet<Vector2Int> checkedPositions = new(initial);

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();
            if (GetCard(current) is not Card source)
                continue;

            foreach (Vector2Int adj in GetAdjacentPositions(current))
            {
                if (checkedPositions.Contains(adj))
                    continue;

                if (TryComboCapture(source, current, adj, capturingPlayer))
                {
                    queue.Enqueue(adj);
                    checkedPositions.Add(adj);
                }
            }
        }
    }

    private bool TryComboCapture(Card source, Vector2Int from, Vector2Int to, bool capturingPlayer)
    {
        if (GetCard(to) is not Card target)
            return false;

        if (target.Capture == capturingPlayer)
            return false;

        int srcRank = GetRankForDirection(source, from, to);
        int tgtRank = GetRankForDirection(target, to, from);

        if (srcRank <= tgtRank)
            return false;

        CaptureCard(to, capturingPlayer);
        return true;
    }

    private int GetRankForDirection(Card card, Vector2Int from, Vector2Int to)
    {
        Vector2Int d = to - from;

        if (d == Vector2Int.up) return card.top;
        if (d == Vector2Int.down) return card.bottom;
        if (d == Vector2Int.right) return card.right;
        if (d == Vector2Int.left) return card.left;

        return 0;
    }

    private void CaptureCard(Vector2Int pos, bool newOwner)
    {
        if (GetCard(pos) is not Card card)
            return;

        card.Capture = newOwner;
        UpdateCardVisual(pos, card);
    }

    private void UpdateCardVisual(Vector2Int pos, Card card)
    {
        GetVisualiser(pos)?.GetComponent<Visualiser>()?.UpdateCard(card);
    }

    // ========================================================================
    // GAME END
    // ========================================================================

    public bool CheckGameEnd(out bool playerWon, out bool isDraw)
    {
        isDraw = false;
        playerWon = false;

        if (CountTotalCards() < MAX_CARDS)
            return false;

        int p = CountPlayerCards(true);
        int o = CountPlayerCards(false);

        if (p == o) { isDraw = true; return true; }

        playerWon = p > o;
        return true;
    }

    private int CountTotalCards()
    {
        int c = 0;
        foreach (var s in board)
            if (s is Card) c++;
        return c;
    }

    public int CountPlayerCards(bool player)
    {
        int c = 0;
        foreach (var s in board)
            if (s is Card card && card.Capture == player)
                c++;
        return c;
    }

    // ========================================================================
    // RESET / REDISTRIBUTION
    // ========================================================================

    public void ResetBoard()
    {
        ClearBoard();
        PlaceSlateRandomly();
    }

    private void ClearBoard()
    {
        for (int x = 0; x < GRID_SIZE; x++)
            for (int y = 0; y < GRID_SIZE; y++)
            {
                board[x, y] = null;
                visuals[x, y] = null;
            }
    }

    // === Redistribution logic intentionally unchanged ===

    // ========================================================================
    // TRANSFORMS
    // ========================================================================

    public void Transform(TransformationType t)
    {
        switch (t)
        {
            case TransformationType.FlipHorizontal: FlipHorizontal(); break;
            case TransformationType.FlipVertical: FlipVertical(); break;
            case TransformationType.RotateClockwise: Rotate(true); break;
            case TransformationType.RotateCounterclockwise: Rotate(false); break;
        }
        OnBoardTransformed?.Invoke();
    }

    private void FlipHorizontal()
    {
        for (int y = 0; y < GRID_SIZE; y++)
            for (int x = 0; x < GRID_SIZE / 2; x++)
                Swap(x, y, GRID_SIZE - 1 - x, y);
    }

    private void FlipVertical()
    {
        for (int x = 0; x < GRID_SIZE; x++)
            for (int y = 0; y < GRID_SIZE / 2; y++)
                Swap(x, y, x, GRID_SIZE - 1 - y);
    }

    private void Rotate(bool cw)
    {
        int n = GRID_SIZE;
        for (int layer = 0; layer < n / 2; layer++)
        {
            int first = layer;
            int last = n - 1 - layer;

            for (int i = first; i < last; i++)
            {
                int o = i - first;
                SwapCycle(first, i, i, last, last, last - o, last - o, first, cw);
            }
        }
    }

    private void SwapCycle(int ax, int ay, int bx, int by, int cx, int cy, int dx, int dy, bool cw)
    {
        if (cw)
        {
            Swap(ax, ay, dx, dy);
            Swap(ax, ay, cx, cy);
            Swap(ax, ay, bx, by);
        }
        else
        {
            Swap(ax, ay, bx, by);
            Swap(ax, ay, cx, cy);
            Swap(ax, ay, dx, dy);
        }
    }

    private void Swap(int x1, int y1, int x2, int y2)
    {
        (board[x1, y1], board[x2, y2]) = (board[x2, y2], board[x1, y1]);
        (visuals[x1, y1], visuals[x2, y2]) = (visuals[x2, y2], visuals[x1, y1]);
    }

    // ========================================================================
    // SLATE EFFECTS
    // ========================================================================

    public void ExecuteEffect(CardEffectType effect, SlateTarget target, Vector2Int playedPos)
    {
        switch (effect)
        {
            case CardEffectType.RotateClockwise: Transform(TransformationType.RotateClockwise); break;
            case CardEffectType.RotateCounterclockwise: Transform(TransformationType.RotateCounterclockwise); break;
            case CardEffectType.FlipXAxis: Transform(TransformationType.FlipHorizontal); break;
            case CardEffectType.FlipYAxis: Transform(TransformationType.FlipVertical); break;
            case CardEffectType.MoveSlateToPosition: TryMoveSlate(TargetToGrid(target)); break;
            case CardEffectType.SwapWithSlate: SwapWithSlate(playedPos); break;
        }
    }

    private Vector2Int TargetToGrid(SlateTarget t) => t switch
    {
        SlateTarget.TopLeft => new(0, 2),
        SlateTarget.TopCenter => new(1, 2),
        SlateTarget.TopRight => new(2, 2),
        SlateTarget.MiddleLeft => new(0, 1),
        SlateTarget.Center => new(1, 1),
        SlateTarget.MiddleRight => new(2, 1),
        SlateTarget.BottomLeft => new(0, 0),
        SlateTarget.BottomCenter => new(1, 0),
        SlateTarget.BottomRight => new(2, 0),
        _ => new(-1, -1)
    };

    private void TryMoveSlate(Vector2Int target)
    {
        if (!IsValidPosition(target) || board[target.x, target.y] != null)
            return;

        board[slatePosition.x, slatePosition.y] = null;
        slatePosition = target;
        board[target.x, target.y] = slateInstance;
        OnBoardTransformed?.Invoke();
    }

    private void SwapWithSlate(Vector2Int pos)
    {
        Swap(pos.x, pos.y, slatePosition.x, slatePosition.y);
        slatePosition = pos;
        OnBoardTransformed?.Invoke();
    }

    // ========================================================================
    // HAND MANAGEMENT
    // ========================================================================

    private void RemoveCardFromOwnerHand(Card card)
    {
        foreach (Hand h in FindObjectsOfType<Hand>())
            if (h.Player == card.Capture)
            {
                h.RemoveCard(card);
                return;
            }

        Debug.LogError($"Failed to remove card {card.Name} from owner hand");
    }

    public bool TryGetCardPosition(Slate card, out Vector2Int position)
    {
        for (int x = 0; x < GRID_SIZE; x++)
        {
            for (int y = 0; y < GRID_SIZE; y++)
            {
                if (board[x, y] == card)
                {
                    position = new Vector2Int(x, y);
                    return true;
                }
            }
        }

        position = default;
        return false;
    }
    public Vector2Int FindSlate()
    {
        return slateInstance != null ? slatePosition : new Vector2Int(-1, -1);
    }
    public void RedistributeCards(Hand playerHand, Hand opponentHand)
    {
        GameLayout layout = FindObjectOfType<GameLayout>();
        layout.ReclaimAllBoardVisuals();

        List<Card> allCards = new();

        for (int x = 0; x < GRID_SIZE; x++)
        {
            for (int y = 0; y < GRID_SIZE; y++)
            {
                if (board[x, y] is Card card)
                {
                    allCards.Add(card);
                    board[x, y] = null;
                    visuals[x, y] = null;
                }
            }
        }

        if (allCards.Count != MAX_CARDS)
            Debug.LogError($"Expected {MAX_CARDS} cards, got {allCards.Count}");

        playerHand.ClearHand();
        opponentHand.ClearHand();

        foreach (Card card in allCards)
        {
            Visualiser visual = layout.GetVisualForCard(card);
            if (visual == null)
                continue;

            visual.transform.SetParent(layout.transform);
            visual.transform.localPosition = Vector3.zero;

            CardSelect select = visual.GetComponent<CardSelect>();
            if (select != null)
                select.enabled = true;
        }

        ShuffleCards(allCards);

        for (int i = 0; i < allCards.Count; i++)
        {
            Card card = allCards[i];

            if (i < 4)
            {
                card.Capture = true;
                playerHand.AddCard(card);
            }
            else
            {
                card.Capture = false;
                opponentHand.AddCard(card);
            }
        }
    }
    private void ShuffleCards(List<Card> cards)
    {
        for (int i = 0; i < cards.Count; i++)
        {
            int randomIndex = Random.Range(i, cards.Count);
            (cards[i], cards[randomIndex]) = (cards[randomIndex], cards[i]);
        }
    }
    public void InitializeForTests(SOCard testSlate)
    {
        board = new Slate[GRID_SIZE, GRID_SIZE];
        visuals = new CardSelect[GRID_SIZE, GRID_SIZE];

        slateInstance = new Slate(testSlate);
        slatePosition = new Vector2Int(1, 1); // center by default
        board[slatePosition.x, slatePosition.y] = slateInstance;
    }
}
