using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class JSONRequest : MonoBehaviour
{
    void Awake()
    {
        GameManager.OnGameStateChanged += GameManager_OnGameStateChanged;
    }

    private void GameManager_OnGameStateChanged(GameManager.GameState obj)
    {
        GetData();
    }

    void CreateCards(string jsonString)
    {
        // TODO: this is a manual check because this is being called twice at start. fix later
        if(GameManager.Instance.deckCards.cards.Count > 0)
        {
            GameManager.Instance.UpdateGameState(GameManager.GameState.PlayerTurnPrep);
            GameManager.OnGameStateChanged -= GameManager_OnGameStateChanged;
            return;
        }

        GameManager.Instance.baseCards = JsonUtility.FromJson<GameManager.CardList>(jsonString);

        

        foreach(GameManager.Card c in GameManager.Instance.baseCards.cards)
        {
            CardScriptableObj test = ScriptableObject.CreateInstance<CardScriptableObj>();
            //populate with data
            test.FillWithData(c);

            //add to test list
            GameManager.Instance.testCSO.Add(test);
        }



        /*CardScriptableObj[] tempCards = new CardScriptableObj[3];
        tempCards = JsonUtility.FromJson<CardScriptableObj[]>(jsonString);
        GameManager.Instance.baseCards.cards = tempCards;*/

        // swap around the 3 cards to randomly determine which card will have 4/3/2 copies
        int[] order = { 0, 1, 2 };
        int[] copies = { 4, 3, 2 };
        for(int i = 0; i < 3; ++i)
        {
            int temp = order[i];
            int randIndex = Random.Range(0, 3);
            order[i] = order[randIndex];
            order[randIndex] = temp;
        }

        // create the cards in a 4/3/2 ratio
        //GameManager.Instance.deckCards.cards = new GameManager.Card[9];
        //GameManager.Instance.allCardsList.cards = new CardScriptableObj[9];
        //int indexer = 0;
        for(int i = 0; i < order.Length; ++i)
        {
            //CardScriptableObj currentCard = GameManager.Instance.baseCards.cards[order[i]];
            GameManager.Card currentCard = GameManager.Instance.baseCards.cards[order[i]];
            for(int j = 0; j < copies[i]; ++j)
            {
                GameManager.Instance.deckCards.cards.Add(currentCard);
                //GameManager.Instance.deckCards.cards[indexer] = currentCard;
                //++indexer;
            }
        }
        
        GameManager.Instance.UpdateGameState(GameManager.GameState.PlayerTurnPrep);
        GameManager.OnGameStateChanged -= GameManager_OnGameStateChanged;
    }

    void GetData() => StartCoroutine(GetDataCoroutine());

    IEnumerator GetDataCoroutine()
    {
        using(UnityWebRequest request = UnityWebRequest.Get(GameManager.Instance.url))
        {
            yield return request.SendWebRequest();
            if(request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                //testText.text = request.error;
            }
            else
            {
                //testText.text = request.downloadHandler.text;
                CreateCards(request.downloadHandler.text);
            }
        }
    }
}
