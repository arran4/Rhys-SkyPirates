using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class GameEndUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject panel;
    [SerializeField] private CanvasGroup panelCanvasGroup;
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private Button rematchButton;

    [Header("Animation Settings")]
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private float textScaleDuration = 0.5f;
    [SerializeField] private Vector3 textFinalScale = Vector3.one;
    [SerializeField] private Vector3 textInitialScale = Vector3.zero;

    private void Start()
    {
        // Ensure panel is hidden at start
        if (panel != null)
        {
            panel.SetActive(false);
            if (panelCanvasGroup != null)
            {
                panelCanvasGroup.alpha = 0f;
            }
        }

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

            // Set Text
            if (resultText != null)
            {
                resultText.text = playerWon ? "VICTORY!" : "DEFEAT";
                resultText.color = playerWon ? new Color(0.2f, 0.8f, 0.2f) : new Color(0.8f, 0.2f, 0.2f); // Vibrant Green / Red
                resultText.transform.localScale = textInitialScale;
            }

            // Start Animations
            StartCoroutine(AnimateUI());
        }
    }

    private IEnumerator AnimateUI()
    {
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);

            // Fade in panel
            if (panelCanvasGroup != null)
            {
                panelCanvasGroup.alpha = t;
            }

            // Pop text
            if (resultText != null && elapsed < textScaleDuration)
            {
                float scaleT = Mathf.Clamp01(elapsed / textScaleDuration);
                // Simple elastic-like overshoot could be added here, but SmoothStep is fine for now
                float smoothScale = Mathf.SmoothStep(0f, 1f, scaleT);
                resultText.transform.localScale = Vector3.Lerp(textInitialScale, textFinalScale, smoothScale);
            }

            yield return null;
        }

        // Ensure final values
        if (panelCanvasGroup != null) panelCanvasGroup.alpha = 1f;
        if (resultText != null) resultText.transform.localScale = textFinalScale;
    }

    private void OnRematchClicked()
    {
        if (GameStateMachine.Instance != null)
        {
            GameStateMachine.Instance.RestartGame();

            // Hide UI immediately
            if (panel != null)
            {
                panel.SetActive(false);
                if (panelCanvasGroup != null)
                {
                    panelCanvasGroup.alpha = 0f;
                }
            }
        }
    }
}
