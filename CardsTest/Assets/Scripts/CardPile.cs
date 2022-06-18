using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Sits on GameManager object.
/// Handles the deck pile, player hand, and discard pile information.
/// </summary>

public class CardPile : MonoBehaviour
{
    public Text deckCardCountText;
    public Text discardedCardCountText;
    public int testMoveCount = 1;

    public void MoveCardsToPile(int num, GameManager.CardList fromPile, GameManager.CardList toPile)//, CardScriptableObj card2)
    {
        // drawing cards, but there's less than num left in the deck
        int drawFirst = 0;
        if (num > fromPile.cards.Count)
        {
            drawFirst = fromPile.cards.Count;
            while (GameManager.Instance.discardedCards.cards.Count > 0)
            {
                GameManager.Instance.deckCards.cards.Add(GameManager.Instance.discardedCards.cards[0]);
                GameManager.Instance.discardedCards.cards.RemoveAt(0);
            }
        }

        for (int i = 0; i < num; ++i)
        {
            // draw the cards that were already in the deck first
            if (i < drawFirst)
            {
                GameManager.Card temp = fromPile.cards[0];
                fromPile.cards.RemoveAt(0);
                toPile.cards.Add(temp);
            }
            else
            {
                int randomCard = Random.Range(0, fromPile.cards.Count);
                GameManager.Card temp = fromPile.cards[randomCard];
                fromPile.cards.RemoveAt(randomCard);
                toPile.cards.Add(temp);
            }
        }

        UpdateCardCounts();
    }

    public void PlayCard(int index)
    {
        GameManager.Card temp = GameManager.Instance.handCards.cards[index];
        if(EnergyHandler.Instance.HaveEnoughEnergy(temp.cost))
        {
            GameManager.Instance.discardedCards.cards.Add(GameManager.Instance.handCards.cards[index]);
            GameManager.Instance.handCards.cards.RemoveAt(index);
            // TODO: animate card

            UpdateCardCounts();
        }
        else
        {
            Debug.Log("Not enough energy");
            
            // TODO: return card (sprite) to hand
        }
    }

    public void EndTurn()
    {
        while(GameManager.Instance.handCards.cards.Count > 0)
        {
            GameManager.Instance.discardedCards.cards.Add(GameManager.Instance.handCards.cards[0]);
            GameManager.Instance.handCards.cards.RemoveAt(0);
        }

        UpdateCardCounts();
    }

    public void UpdateCardCounts()
    {
        deckCardCountText.text = GameManager.Instance.deckCards.cards.Count.ToString();
        discardedCardCountText.text = GameManager.Instance.discardedCards.cards.Count.ToString();
    }

    public void TestMoving()
    {
        MoveCardsToPile(testMoveCount, GameManager.Instance.deckCards, GameManager.Instance.handCards);
    }
    
    public void TestPlayCard()
    {
        PlayCard(testMoveCount);
    }
}
