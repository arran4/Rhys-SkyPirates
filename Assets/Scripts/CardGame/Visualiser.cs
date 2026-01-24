using UnityEngine;

public class Visualiser : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer rend;

    private Slate card;

    public void Setup(Slate card)
    {
        this.card = card;
        rend.sprite = card.CardFace;

        if (card is Card)
        {
            Card cardData = (Card)card;

            // Set the card data on the CardSelect component
            CardSelect cardSelect = GetComponent<CardSelect>();
            if (cardSelect != null)
            {
                cardSelect.SetCardData(cardData);
            }

            // Update visual appearance based on capture status
            UpdateVisual(cardData);
        }
    }

    /// <summary>
    /// Updates the visual appearance of the card based on its capture status
    /// </summary>
    public void UpdateVisual(Card cardData)
    {
        if (cardData.Capture)
        {
            rend.color = Color.blue;
        }
        else
        {
            rend.color = Color.red;
            this.GetComponent<CardSelect>().enabled = false;
        }
    }

    /// <summary>
    /// Gets the card data this visualiser represents
    /// </summary>
    public Slate GetCard()
    {
        return card;
    }

    /// <summary>
    /// Updates the card data and refreshes the visual
    /// Used when a card is captured and needs to change ownership
    /// </summary>
    public void UpdateCard(Card newCardData)
    {
        card = newCardData;
        UpdateVisual(newCardData);
    }
}
