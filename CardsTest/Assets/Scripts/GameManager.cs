using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Sits on GameManager object.
/// Holds player and card data. Manages the game via GameState enumerator.
/// 
/// Notes:
/// The PlayerTurnPrep is being called twice in the beginning. It probably has something to do with the JSONRequest script adding itself to the OnGameStateChanged
/// </summary>

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public CardList baseCards = new CardList();

    public List<CardScriptableObj> baseCardsSO = new List<CardScriptableObj>();
    public List<GameObject> displayCards = new List<GameObject>();
    public List<CardScriptableObj> deckCards = new List<CardScriptableObj>();
    public List<CardScriptableObj> handCards = new List<CardScriptableObj>();
    public List<CardScriptableObj> discardedCards = new List<CardScriptableObj>();
    public Sprite[] imageList = new Sprite[78];
    public int maxCardsInHand = 4;

    public Transform cardSpawnLocation;
    public Transform cardHandLocation;
    public Transform cardDiscardLocation;
    public GameObject canvasObj;

    public CardPile pileManager;
    public string url = "https://client.dev.kote.robotseamonster.com/TEST_HARNESS/json_files/cards.json";
    public GameState state;
    public static event Action<GameState> OnGameStateChanged;
    public int delayTime = 2000;
    public int playerEnergy = 4;
    public int playerMaxEnergy = 4;
    public bool turnInProgress = false;

    public enum GameState
    {
        GameStart,
        PlayerTurnPrep,
        PlayerTurnEnd
    }

    /// <summary>
    /// card classes start. Used for grabbing data from JSON
    /// </summary>
    [System.Serializable]
    public class Effect
    {
        public string type;
        public int value;
        public string target;
    }

    [System.Serializable]
    public class Card
    {
        public int id;
        public string name;
        public int cost;
        public string type;
        public int image_id;
        public Effect[] effects;
    }

    [System.Serializable]
    public class CardList
    {
        public List<Card> cards;
    }
    /// <summary>
    /// card classes end
    /// </summary>

    void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        UpdateGameState(GameState.GameStart);
    }

    public void UpdateGameState(GameState newState)
    {
        state = newState;

        switch (newState)
        {
            case GameState.GameStart:
                // currently only for allowing JSONRequest to activate
                break;
            case GameState.PlayerTurnPrep:
                HandlePlayerTurnPrep();
                break;
            case GameState.PlayerTurnEnd:
                HandlePlayerTurnEnd();
                break;
        }

        OnGameStateChanged?.Invoke(newState);//probably don't need this and then could edit the JSON
    }

    void HandlePlayerTurnPrep()
    {
        //EnergyHandler.Instance.ResetEnergy();

        // draw 4 cards
        //if(handCards.Count == 0) pileManager.DrawCardsFromDeck(maxCardsInHand);
        /*while(handCards.Count < maxCardsInHand)
        {
            pileManager.DrawCardsFromDeck(1);
            new WaitForSeconds(1);
        }*/

        if(!turnInProgress)
        {
            EnergyHandler.Instance.ResetEnergy();
            if (handCards.Count < maxCardsInHand) pileManager.DrawCardsFromDeck(maxCardsInHand);
            else turnInProgress = true;
        }
    }

    void HandlePlayerTurnEnd()
    {
        foreach(GameObject card in displayCards)
        {
            if(card.GetComponent<CardDisplay>().inHand)
            {
                card.GetComponent<CardDisplay>().SetAnimState(CardDisplay.StateAnimation.Discard);
            }
        }
        turnInProgress = false;
        pileManager.EndTurn();
    }
}
