using NUnit.Framework;
using UnityEngine;

public class CardBoardPlusComboTests
{
    private CardBoard board;
    private GameRules Game;

    [SetUp]
    public void Setup()
    {
        GameObject go = new GameObject("Board");
        board = go.AddComponent<CardBoard>();
        Game = go.AddComponent<GameRules>();
        Game.InitializeForTests();
        Game.IsCombo = true;
        Game.IsPlus = true;
        GameRules.GameInstance = Game;

        SOCard testSlate = ScriptableObject.CreateInstance<SOCard>();

        board.InitializeForTests(testSlate);

       

        // Enable PLUS, disable others
        Game.IsPlus = true;
        Game.IsSame = false;
        Game.IsCombo = false;
        Game.IsBeforeCapture = false;
    }

    [Test]
    public void PlusRule_CapturesBothAdjacentCards()
    {
        // Played card (player)
        Card played = MakeCard(true, top: 5, bottom: 5, left: 5, right: 5);

        // Adjacent cards (opponent)
        Card left = MakeCard(false, right: 3);
        Card right = MakeCard(false, left: 3);

        Vector2Int center = new(1, 1);
        Vector2Int leftPos = new(0, 1);
        Vector2Int rightPos = new(2, 1);

        // Place cards manually
        SetCard(center, played);
        SetCard(leftPos, left);
        SetCard(rightPos, right);

        // Act
        board.ExecuteCapture(center, played);

        // Assert
        Assert.IsTrue(left.Capture, "Left card should be captured by PLUS");
        Assert.IsTrue(right.Capture, "Right card should be captured by PLUS");
    }
    [Test]
    public void PlusRule_TriggersComboCapture()
    {
        GameRules.GameInstance.IsCombo = true;

        Card played = MakeCard(true, top: 5, bottom: 5, left: 5, right: 5);

        Card left = MakeCard(false, right: 3, top: 6);
        Card right = MakeCard(false, left: 3);

        // This should be captured by combo
        Card comboTarget = MakeCard(false, bottom: 1);

        Vector2Int center = new(1, 1);
        Vector2Int leftPos = new(0, 1);
        Vector2Int rightPos = new(2, 1);
        Vector2Int aboveLeft = new(0, 2);

        SetCard(center, played);
        SetCard(leftPos, left);
        SetCard(rightPos, right);
        SetCard(aboveLeft, comboTarget);

        board.ExecuteCapture(center, played);

        Assert.IsTrue(left.Capture, "Left card should flip from PLUS");
        Assert.IsTrue(right.Capture, "Right card should flip from PLUS");
        Assert.IsTrue(comboTarget.Capture, "Combo target should be captured");
    }
    private Card MakeCard(
        bool owner,
        int top = 0,
        int bottom = 0,
        int left = 0,
        int right = 0
    )
    {
        SOCard data = ScriptableObject.CreateInstance<SOCard>();

        data.Top = top;
        data.Bottom = bottom;
        data.Left = left;
        data.Right = right;

        data.Effect = CardEffectType.None;
        data.SlateTarget = SlateTarget.Center;

        return new Card(data, owner);
    }


    private void SetCard(Vector2Int pos, Card card)
    {
        Assert.IsNotNull(board, "board is null");

        var boardField = typeof(CardBoard).GetField(
            "board",
            System.Reflection.BindingFlags.Instance |
            System.Reflection.BindingFlags.NonPublic
        );

        Assert.IsNotNull(boardField, "CardBoard.board field not found via reflection");

        var grid = boardField.GetValue(board) as Slate[,];

        Assert.IsNotNull(grid, "CardBoard.board is null — InitializeForTests was not called");

        grid[pos.x, pos.y] = card;
    }


}

