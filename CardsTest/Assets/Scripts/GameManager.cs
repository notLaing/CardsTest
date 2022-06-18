using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;



/// OOOOOOIIIIIII LOOK HERE!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
/// 
/// The PlayerTurnPrep is being called twice in the beginning. It probably has something to do with the
/// JSONRequest script adding itself to the OnGameStateChanged
/// 
/// After JSONRequest, fill the data into a scriptable object. For each scriptable object, make a card. How?
/// Well, should be able to dig through the baseCards (if it has become a list of scriptable objects),
/// set some variable of type CardScriptableObj equal to that, then have a function that fills out the information.
/// This is an alternative to creating a scriptable obj in the assets folder and moving it via inspector
/// 
/// YOU NEED TO FIX THIS STUFF!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!



/// <summary>
/// Sits on GameManager object.
/// Holds player and card data. Manages the game via GameState enumerator.
/// </summary>

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public CardList baseCards = new CardList();
    public CardList deckCards = new CardList();
    public CardList handCards = new CardList();
    public CardList discardedCards = new CardList();

    public List<CardScriptableObj> testCSO = new List<CardScriptableObj>();
    public List<GameObject> displayCards = new List<GameObject>();
    public int maxCardsInHand = 4;

    public CardPile pileManager;
    public string url = "https://client.dev.kote.robotseamonster.com/TEST_HARNESS/json_files/cards.json";
    public GameState state;
    public static event Action<GameState> OnGameStateChanged;
    public int delayTime = 2000;
    public int playerEnergy = 4;
    public int playerMaxEnergy = 4;

    public enum GameState
    {
        GameStart,//steps 1, 2
        PlayerTurnPrep,//steps 3, 4, 10
        PlayerTurnWait,//steps 5, 8
        PlayerTurnPlay,//steps 6, 7, 8
        PlayerTurnEnd//step 9
    }

    /// <summary>
    /// card classes start
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
        //public Card[] cards;
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

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateGameState(GameState newState)
    {
        state = newState;

        switch (newState)
        {
            case GameState.GameStart:
                break;
            case GameState.PlayerTurnPrep:
                HandlePlayerTurnPrep();
                break;
            case GameState.PlayerTurnWait:
                HandlePlayerTurnWait();
                break;
            case GameState.PlayerTurnPlay:
                break;
            case GameState.PlayerTurnEnd:
                HandlePlayerTurnEnd();
                break;
        }

        OnGameStateChanged?.Invoke(newState);
    }

    void HandlePlayerTurnPrep()
    {
        EnergyHandler.Instance.ResetEnergy();

        // draw 4 cards
        if(handCards.cards.Count == 0) pileManager.DrawCardsFromDeck(maxCardsInHand, deckCards, handCards);
    }

    async void HandlePlayerTurnWait()
    {
        await Task.Delay(delayTime);
    }

    void HandlePlayerTurnEnd()
    {
        pileManager.EndTurn();
    }
}
