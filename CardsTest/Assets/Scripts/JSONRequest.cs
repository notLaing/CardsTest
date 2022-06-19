using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

/// <summary>
/// Sits on GameManager object.
/// Requests JSON file and creates base cards
/// </summary>

public class JSONRequest : MonoBehaviour
{
    int[] order = { 0, 1, 2 };
    int[] copies = { 4, 3, 2 };

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
        if(GameManager.Instance.deckCards.Count > 0)
        {
            GameManager.Instance.UpdateGameState(GameManager.GameState.PlayerTurnPrep);
            GameManager.OnGameStateChanged -= GameManager_OnGameStateChanged;
            return;
        }

        GameManager.Instance.baseCards = JsonUtility.FromJson<GameManager.CardList>(jsonString);

        

        foreach(GameManager.Card c in GameManager.Instance.baseCards.cards)
        {
            CardScriptableObj test = ScriptableObject.CreateInstance<CardScriptableObj>();

            // populate with data
            test.FillWithData(c);

            // add to test list
            GameManager.Instance.baseCardsSO.Add(test);
        }

        // swap around the 3 cards to randomly determine which card will have 4/3/2 copies
        for(int i = 0; i < order.Length; ++i)
        {
            int temp = order[i];
            int randIndex = Random.Range(0, order.Length);
            order[i] = order[randIndex];
            order[randIndex] = temp;
        }

        // create the cards in a 4/3/2 ratio
        for(int i = 0; i < order.Length; ++i)
        {
            CardScriptableObj currentCard = GameManager.Instance.baseCardsSO[order[i]];
            for(int j = 0; j < copies[i]; ++j)
            {
                GameManager.Instance.deckCards.Add(currentCard);
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
                //Debug.Log(request.error);
            }
            else
            {
                CreateCards(request.downloadHandler.text);
            }
        }
    }
}
