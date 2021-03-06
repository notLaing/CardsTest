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
    public bool playingCard = false;

    public void DrawCardsFromDeck(int num)
    {
        // reset cards the first time drawing a card after a turn ends
        if (GameManager.Instance.maxCardsInHand == GameManager.Instance.handCards.Count)
        {
            foreach (GameObject c in GameManager.Instance.displayCards)
            {
                c.GetComponent<CardDisplay>().inHand = false;
            }
        }

        // check if the number of displayCards is less than the number of cards we need to play
        while (GameManager.Instance.handCards.Count + 1 > GameManager.Instance.displayCards.Count)
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
            int newestCardIndex = GameManager.Instance.handCards.Count - 1;
            GameManager.Instance.displayCards[newestCardIndex].GetComponent<CardDisplay>().CreateCardDisplay(GameManager.Instance.handCards[newestCardIndex]);
            GameManager.Instance.displayCards[newestCardIndex].GetComponent<CardDisplay>().ResetOrientation();
            GameManager.Instance.displayCards[newestCardIndex].GetComponent<CardDisplay>().inHand = true;

            // animate from deck to hand
            GameManager.Instance.displayCards[newestCardIndex].GetComponent<CardDisplay>().ForceReset();
            GameManager.Instance.displayCards[newestCardIndex].GetComponent<CardDisplay>().SetAnimState(CardDisplay.StateAnimation.Draw);
        }

        if (GameManager.Instance.handCards.Count < GameManager.Instance.maxCardsInHand) StartCoroutine(KeepDrawing());
        else UpdateCardCounts(true);
    }

    public void PlayCard(int indHand, int indOverall)
    {
        if (playingCard) return;

        CardScriptableObj temp = GameManager.Instance.handCards[indHand];

        // play and move the card (data and display) if the player has enough energy
        if(EnergyHandler.Instance.HaveEnoughEnergy(temp.cost))
        {
            playingCard = true;
            GameManager.Instance.discardedCards.Add(GameManager.Instance.handCards[indHand]);
            
            CardDisplay displayedCard = GameManager.Instance.displayCards[indOverall].GetComponent<CardDisplay>();
            displayedCard.SetAnimState(CardDisplay.StateAnimation.Play);
            displayedCard.indexInHand = GameManager.Instance.maxCardsInHand + 1;
            displayedCard.inHand = false;
            
            GameManager.Instance.handCards.RemoveAt(indHand);

            StartCoroutine(UpdateCardCountAfterTime(displayedCard.playStillAnimTime + displayedCard.discardPlayAnimTime, true));
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

        float endTime = GameManager.Instance.displayCards[0].GetComponent<CardDisplay>().discardEndAnimTime;
        StartCoroutine(UpdateCardCountAfterTime(endTime));
        StartCoroutine(StartNextTurn(endTime));
    }

    public void UpdateCardCounts(bool resetSpread = false)
    {
        deckCardCountText.text = GameManager.Instance.deckCards.Count.ToString();
        discardedCardCountText.text = GameManager.Instance.discardedCards.Count.ToString();

        int counter = 0;
        for (int i = 0; i < GameManager.Instance.displayCards.Count; ++i)
        {
            CardDisplay currentCard = GameManager.Instance.displayCards[i].GetComponent<CardDisplay>();

            if (currentCard.inHand)
            {
                currentCard.ChangeColor(GameManager.Instance.playerEnergy >= currentCard.cardScriptObj.cost);
                currentCard.indexInHand = counter;
                ++counter;
                if(resetSpread) currentCard.SetAnimState(CardDisplay.StateAnimation.Spread);
            }
        }
    }

    IEnumerator KeepDrawing()
    {
        UpdateCardCounts(true);
        yield return new WaitForSeconds(GameManager.Instance.displayCards[0].GetComponent<CardDisplay>().drawAnimTime);
        GameManager.Instance.UpdateGameState(GameManager.GameState.PlayerTurnPrep);
    }

    IEnumerator UpdateCardCountAfterTime(float t, bool b = false)
    {
        yield return new WaitForSeconds(t);
        UpdateCardCounts(b);
    }

    IEnumerator StartNextTurn(float t)
    {
        yield return new WaitForSeconds(t);
        GameManager.Instance.UpdateGameState(GameManager.GameState.PlayerTurnPrep);
    }
}
