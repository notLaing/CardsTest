using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardPile : MonoBehaviour
{
    public Text deckCardCountText;
    public Text discardedCardCountText;
    public int testMoveCount = 1;

    public void MoveCardsToPile(int num, GameManager.CardList fromPile, GameManager.CardList toPile)//, CardScriptableObj card2)
    {
        // drawing cards, but there's less than num left in the deck
        if (num > fromPile.cards.Count)
        {
            while (GameManager.Instance.discardedCards.cards.Count > 0)
            {
                GameManager.Instance.deckCards.cards.Add(GameManager.Instance.discardedCards.cards[0]);
                GameManager.Instance.discardedCards.cards.RemoveAt(0);
            }
        }

        for (int i = 0; i < num; ++i)
        {
            GameManager.Card temp = fromPile.cards[0];
            fromPile.cards.RemoveAt(0);
            toPile.cards.Add(temp);
        }

        UpdateCardCounts();
    }

    public void PlayCard(int index)
    {
        GameManager.Instance.discardedCards.cards.Add(GameManager.Instance.handCards.cards[index]);
        GameManager.Instance.handCards.cards.RemoveAt(index);
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
