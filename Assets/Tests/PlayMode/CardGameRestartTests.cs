using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class CardGameRestartTests
{
    private GameObject gameObj;
    private GameStateMachine gameStateMachine;
    private CardBoard cardBoard;
    private Hand playerHand;
    private Hand opponentHand;
    private GameLayout gameLayout;

    [SetUp]
    public void Setup()
    {
        gameObj = new GameObject("CardGame");

        // Setup components
        gameStateMachine = gameObj.AddComponent<GameStateMachine>();
        cardBoard = gameObj.AddComponent<CardBoard>();
        playerHand = gameObj.AddComponent<Hand>();
        opponentHand = gameObj.AddComponent<Hand>();
        gameLayout = gameObj.AddComponent<GameLayout>();

        // Link references using reflection
        SetPrivateField(gameStateMachine, "cardBoard", cardBoard);
        SetPrivateField(gameStateMachine, "playerHand", playerHand);
        SetPrivateField(gameStateMachine, "opponentHand", opponentHand);
        SetPrivateField(gameStateMachine, "gameLayout", gameLayout);

        SetPrivateField(playerHand, "cardBoard", cardBoard);
        SetPrivateField(playerHand, "gameLayout", gameLayout);
        playerHand.Player = true;

        SetPrivateField(opponentHand, "cardBoard", cardBoard);
        SetPrivateField(opponentHand, "gameLayout", gameLayout);
        opponentHand.Player = false;

        SetPrivateField(gameLayout, "cardBoard", cardBoard);
        SetPrivateField(gameLayout, "playerHand", playerHand);
        SetPrivateField(gameLayout, "enemyHand", opponentHand);

        // Need a SnapProvider for GameLayout
        var snapProvider = gameObj.AddComponent<GridSnapProvider>();
        SetPrivateField(gameLayout, "snapProvider", snapProvider);

        // Mock Slate SO for CardBoard
        var slateSO = ScriptableObject.CreateInstance<SOCard>();
        SetPrivateField(cardBoard, "slate", slateSO);

        // Initialize Board for tests (creates array, places slate)
        cardBoard.InitializeForTests(slateSO);
    }

    [TearDown]
    public void Teardown()
    {
        Object.Destroy(gameObj);
    }

    [UnityTest]
    public IEnumerator RestartGame_ResetsStateAndBoard()
    {
        // Arrange: Simulate Game Over state
        gameStateMachine.ChangeState(GameState.GameOver);

        yield return null;

        // Act: Restart Game
        gameStateMachine.RestartGame();

        yield return null;

        // Assert: State should be GameStart
        Assert.AreEqual(GameState.GameStart, gameStateMachine.GetCurrentState());

        // Assert: Board should be reset (only 1 item - the slate)
        int occupiedCount = 0;
        for(int x=0; x<3; x++)
        {
            for(int y=0; y<3; y++)
            {
                if (cardBoard.GetCard(new Vector2Int(x,y)) != null)
                    occupiedCount++;
            }
        }

        Assert.AreEqual(1, occupiedCount, "Board should contain exactly 1 slate after restart");
    }

    private void SetPrivateField(object target, string fieldName, object value)
    {
        var field = target.GetType().GetField(fieldName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        if (field != null)
        {
            field.SetValue(target, value);
        }
        else
        {
            Debug.LogError($"Field {fieldName} not found on {target.GetType()}");
        }
    }
}
