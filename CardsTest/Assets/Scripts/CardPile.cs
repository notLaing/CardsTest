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
    public GameObject cardTemplate;
    public Text deckCardCountText;
    public Text discardedCardCountText;

    public void DrawCardsFromDeck(int num)
    {
        foreach (GameObject c in GameManager.Instance.displayCards)
        {
            c.transform.position = GameManager.Instance.cardSpawnLocation.position;
            c.transform.localScale = GameManager.Instance.cardSpawnLocation.localScale;
        }

        // check if the number of displayCards is less than the number of cards we need to play
        while (GameManager.Instance.maxCardsInHand > GameManager.Instance.displayCards.Count)
        {
            GameObject spawnCard = Instantiate(cardTemplate, GameManager.Instance.cardSpawnLocation.position, Quaternion.identity);
            spawnCard.transform.SetParent(GameManager.Instance.canvasObj.transform, false);
            GameManager.Instance.displayCards.Add(spawnCard);
            GameManager.Instance.displayCards[GameManager.Instance.displayCards.Count - 1].GetComponent<CardDisplay>().indexOverall = GameManager.Instance.displayCards.Count - 1;
        }

        // drawing cards, but there's less than num left in the deck
        int drawFirst = 0;
        if (num > GameManager.Instance.deckCards.Count)
        {
            drawFirst = GameManager.Instance.deckCards.Count;

            // move all the cards from the discard pile to the deck pile
            while (GameManager.Instance.discardedCards.Count > 0)
            {
                GameManager.Instance.deckCards.Add(GameManager.Instance.discardedCards[0]);
                GameManager.Instance.discardedCards.RemoveAt(0);
            }
        }

        for (int i = 0; i < num; ++i)
        {
            int drawIndex = 0;

            // draw the cards that were already in the deck first
            if (!(i < drawFirst)) drawIndex = Random.Range(0, GameManager.Instance.deckCards.Count);

            GameManager.Instance.handCards.Add(GameManager.Instance.deckCards[drawIndex]);
            GameManager.Instance.deckCards.RemoveAt(drawIndex);

            // create the appropriate type of card
            GameManager.Instance.displayCards[i].GetComponent<CardDisplay>().CreateCardDisplay(GameManager.Instance.handCards[GameManager.Instance.handCards.Count - 1]);
            GameManager.Instance.displayCards[i].GetComponent<CardDisplay>().inHand = true;

            // animate from deck to hand
            GameManager.Instance.displayCards[i].GetComponent<CardDisplay>().ForceReset();
            GameManager.Instance.displayCards[i].GetComponent<CardDisplay>().SetAnimState(CardDisplay.StateAnimation.Draw);
        }

        UpdateCardCounts();
    }

    public void PlayCard(int indHand, int indOverall)
    {
        CardScriptableObj temp = GameManager.Instance.handCards[indHand];

        // play and move the card (data and display) if the player has enough energy
        if(EnergyHandler.Instance.HaveEnoughEnergy(temp.cost))
        {
            GameManager.Instance.discardedCards.Add(GameManager.Instance.handCards[indHand]);
            GameManager.Instance.displayCards[indOverall].GetComponent<CardDisplay>().SetAnimState(CardDisplay.StateAnimation.Play);
            GameManager.Instance.displayCards[indOverall].GetComponent<CardDisplay>().indexInHand = GameManager.Instance.maxCardsInHand + 1;
            GameManager.Instance.displayCards[indOverall].GetComponent<CardDisplay>().inHand = false;
            GameManager.Instance.handCards.RemoveAt(indHand);

            UpdateCardCounts(true);
        }
        else
        {
            Debug.Log("Not enough energy");
            // card automatically returns to hand via CardDisplay.OnMouseExit()
        }
    }

    public void EndTurn()
    {
        while(GameManager.Instance.handCards.Count > 0)
        {
            GameManager.Instance.discardedCards.Add(GameManager.Instance.handCards[0]);
            GameManager.Instance.handCards.RemoveAt(0);
        }

        foreach(GameObject c in GameManager.Instance.displayCards)
        {
            c.GetComponent<CardDisplay>().inHand = false;
        }

        //UpdateCardCounts();
        StartCoroutine(UpdateCardCountAfterTime());
    }

    public void UpdateCardCounts(bool resetSpread = false)
    {
        deckCardCountText.text = GameManager.Instance.deckCards.Count.ToString();
        discardedCardCountText.text = GameManager.Instance.discardedCards.Count.ToString();

        int counter = 0;
        for (int i = 0; i < GameManager.Instance.displayCards.Count; ++i)
        {
            CardDisplay currentCard = GameManager.Instance.displayCards[i].GetComponent<CardDisplay>();
            currentCard.ChangeColor(GameManager.Instance.playerEnergy >= currentCard.cardScriptObj.cost);

            if (currentCard.inHand)
            {
                currentCard.indexInHand = counter;
                ++counter;
                if(resetSpread) currentCard.SetAnimState(CardDisplay.StateAnimation.Spread);
            }
        }
    }

    IEnumerator UpdateCardCountAfterTime()
    {
        yield return new WaitForSeconds(GameManager.Instance.displayCards[0].GetComponent<CardDisplay>().discardEndAnimTime);
        UpdateCardCounts();
    }
}
