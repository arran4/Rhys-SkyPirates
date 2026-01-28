using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class SimpleAITests
{
    private GameObject gameGameObject;
    private GameStateMachine gameStateMachine;
    private GameObject aiGameObject;
    private SimpleAI simpleAI;

    [SetUp]
    public void Setup()
    {
        // Setup GameStateMachine
        gameGameObject = new GameObject("GameStateMachine");
        gameStateMachine = gameGameObject.AddComponent<GameStateMachine>();

        // Setup SimpleAI
        aiGameObject = new GameObject("SimpleAI");
        simpleAI = aiGameObject.AddComponent<SimpleAI>();
    }

    [TearDown]
    public void Teardown()
    {
        if (gameGameObject != null)
            Object.Destroy(gameGameObject);
        if (aiGameObject != null)
            Object.Destroy(aiGameObject);
    }

    [UnityTest]
    public IEnumerator AI_Responds_To_OpponentTurn_Event()
    {
        // Wait for Start methods to run
        yield return null;

        // We expect the AI to start making a move when the turn changes to OpponentTurn.
        // The MakeMove coroutine logs "========== AI MAKING MOVE ==========" at the start.
        UnityEngine.TestTools.LogAssert.Expect(LogType.Log, "========== AI MAKING MOVE ==========");

        // We also expect it to fail later due to missing dependencies (Hand, Board),
        // so we might need to ignore those errors to pass the test,
        // or just verify the initial trigger.
        // For this test, we accept that it might throw errors after the log,
        // but we are only verifying the trigger mechanism.

        // Trigger the state change
        // Note: We need to ensure the state machine accepts the transition.
        // Initial state is usually GameStart.
        gameStateMachine.ChangeState(GameState.OpponentTurn);

        yield return null; // Wait for frame
    }
}
