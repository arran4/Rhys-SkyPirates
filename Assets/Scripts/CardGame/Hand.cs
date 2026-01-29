using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    [SerializeField]
    public List<SOCard> SOPlayerHand = new List<SOCard>();

    [SerializeField]
    public List<Card> PlayerHand = new List<Card>();

    [SerializeField]
    public bool Player;

    [SerializeField]
    private Visualiser cardView;

    [SerializeField]
    private CardBoard cardBoard;

    [SerializeField]
    private GameLayout gameLayout;

    private List<Visualiser> handVisualisers = new List<Visualiser>();

    public void Start()
    {
        // Find CardBoard if not set
        if (cardBoard == null)
        {
            cardBoard = FindObjectOfType<CardBoard>();
        }

        if (gameLayout == null)
        {
            gameLayout = FindObjectOfType<GameLayout>();
        }

        InitializeHand();
    }

    public void InitializeHand()
    {
        foreach (SOCard a in SOPlayerHand)
        {
            Card card = new Card(a, Player);
            Visualiser view = Instantiate(cardView);
            view.Setup(card);
            gameLayout.RegisterVisual(card, view);
            PlayerHand.Add(card);
            handVisualisers.Add(view);
        }
    }

    public void ResetHand()
    {
        PlayerHand.Clear();
        handVisualisers.Clear();
        InitializeHand();
    }


    public List<Visualiser> GetVisualisers()
    {
        return handVisualisers;
    }

    /// <summary>
    /// Adds a card to the hand, reusing existing visualizer if available
    /// </summary>
    public void AddCard(Card ToAdd)
    {
        if (PlayerHand.Count >= 4)
        {
            Debug.LogWarning($"Hand full! Cannot add {ToAdd.Name}");
            return;
        }

        // Check if card is already in hand (shouldn't happen, but be safe)
        if (PlayerHand.Contains(ToAdd))
        {
            Debug.LogWarning($"Card {ToAdd.Name} already in hand! Skipping.");
            return;
        }

        PlayerHand.Add(ToAdd);

        // Try to find existing visualizer for this card in GameLayout's registry
        Visualiser existingVisual = null;
        if (gameLayout != null)
        {
            existingVisual = gameLayout.GetVisualForCard(ToAdd);
        }

        if (existingVisual != null)
        {
            // Check if visualizer is already in our list
            if (handVisualisers.Contains(existingVisual))
            {
                Debug.LogWarning($"Visualizer for {ToAdd.Name} already in hand visualizers!");
                return;
            }

            // Reuse existing visualizer
            existingVisual.Setup(ToAdd); // Refresh the visual
            existingVisual.transform.SetParent(transform);
            handVisualisers.Add(existingVisual);

            // Re-enable CardSelect component
            CardSelect cardSelect = existingVisual.GetComponent<CardSelect>();
            if (cardSelect != null)
            {
                cardSelect.enabled = true;
            }

            Debug.Log($"Reused existing visualizer for {ToAdd.Name}");
        }
        else
        {
            // Create new visualizer (shouldn't happen during redistribution, but keeps it safe)
            Visualiser view = Instantiate(cardView);
            view.Setup(ToAdd);
            if (gameLayout != null)
            {
                gameLayout.RegisterVisual(ToAdd, view);
            }
            handVisualisers.Add(view);

            Debug.Log($"Created new visualizer for {ToAdd.Name}");
        }
    }

    public void RemoveCard(Card toRemove)
    {
        int index = PlayerHand.IndexOf(toRemove);
        if (index < 0)
            return;

        PlayerHand.RemoveAt(index);

        if (index < handVisualisers.Count)
        {
            // Just detach from hand — DO NOT DESTROY
            Visualiser visualiser = handVisualisers[index];
            handVisualisers.RemoveAt(index);


        }
    }


    /// <summary>
    /// Removes a visualiser from the hand (called when card is placed on board)
    /// </summary>
    public void RemoveVisualiser(Visualiser visualiser)
    {
        if (handVisualisers.Contains(visualiser))
        {
            handVisualisers.Remove(visualiser);

            // Also remove the corresponding card data
            Card cardData = visualiser.GetCard() as Card;
            if (cardData != null && PlayerHand.Contains(cardData))
            {
                PlayerHand.Remove(cardData);
            }
        }
    }

    /// <summary>
    /// Clears all cards from hand without destroying visualizers
    /// Used during redistribution
    /// </summary>
    public void ClearHand()
    {
        PlayerHand.Clear();
        handVisualisers.Clear();
    }
}
