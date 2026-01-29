using UnityEngine;
using UnityEngine.UI;

public class CardGameUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private Text resultText;
    [SerializeField] private Button rematchButton;

    private void Start()
    {
        // Subscribe to events
        if (GameStateMachine.Instance != null)
        {
            GameStateMachine.Instance.OnGameEnd += HandleGameEnd;
            GameStateMachine.Instance.OnStateChanged += HandleStateChanged;
        }

        if (rematchButton != null)
        {
            rematchButton.onClick.AddListener(OnRematchClicked);
        }

        // Ensure panel is hidden at start
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        if (GameStateMachine.Instance != null)
        {
            GameStateMachine.Instance.OnGameEnd -= HandleGameEnd;
            GameStateMachine.Instance.OnStateChanged -= HandleStateChanged;
        }

        if (rematchButton != null)
        {
            rematchButton.onClick.RemoveListener(OnRematchClicked);
        }
    }

    private void HandleGameEnd(bool playerWon)
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        if (resultText != null)
        {
            resultText.text = playerWon ? "YOU WIN!" : "YOU LOSE!";
            resultText.color = playerWon ? Color.green : Color.red;
        }
    }

    private void HandleStateChanged(GameState newState)
    {
        if (newState == GameState.GameStart && gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }

    private void OnRematchClicked()
    {
        if (GameStateMachine.Instance != null)
        {
            GameStateMachine.Instance.RestartGame();
        }
    }
}
