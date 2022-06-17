using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public CardList baseCards = new CardList();
    public CardList deckCards = new CardList();
    public CardList handCards = new CardList();
    public CardList discardedCards = new CardList();
    public string url = "https://client.dev.kote.robotseamonster.com/TEST_HARNESS/json_files/cards.json";
    public GameState state;
    public static event Action<GameState> OnGameStateChanged;
    public int delayTime = 2000;

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
                break;
            case GameState.PlayerTurnWait:
                HandlePlayerTurnWait();
                break;
            case GameState.PlayerTurnPlay:
                break;
            case GameState.PlayerTurnEnd:
                break;
        }

        OnGameStateChanged?.Invoke(newState);
    }

    async void HandlePlayerTurnWait()
    {
        await Task.Delay(delayTime);
    }
}
