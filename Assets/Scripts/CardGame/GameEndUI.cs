using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameEndUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private Button rematchButton;

    private void Start()
    {
        // Ensure panel is hidden at start
        if (panel != null)
            panel.SetActive(false);

        // Subscribe to GameStateMachine events
        if (GameStateMachine.Instance != null)
        {
            GameStateMachine.Instance.OnGameEnd += HandleGameEnd;
        }
        else
        {
            Debug.LogWarning("GameStateMachine Instance not found!");
        }

        // Setup button listener
        if (rematchButton != null)
        {
            rematchButton.onClick.AddListener(OnRematchClicked);
        }
    }

    private void OnDestroy()
    {
        if (GameStateMachine.Instance != null)
        {
            GameStateMachine.Instance.OnGameEnd -= HandleGameEnd;
        }
    }

    private void HandleGameEnd(bool playerWon)
    {
        if (panel != null)
        {
            panel.SetActive(true);
        }

        if (resultText != null)
        {
            resultText.text = playerWon ? "YOU WIN!" : "YOU LOSE!";
            // Optional: Change color based on win/loss
            resultText.color = playerWon ? Color.green : Color.red;
        }
    }

    private void OnRematchClicked()
    {
        if (GameStateMachine.Instance != null)
        {
            GameStateMachine.Instance.RestartGame();
            if (panel != null)
            {
                panel.SetActive(false);
            }
        }
    }
}
