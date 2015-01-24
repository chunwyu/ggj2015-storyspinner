using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum GameState
{
    Loading,
    MainMenu,
    WaitingForConnections,
    DrawCards,
    DrawGoals,
    MakeStory,
    ScoringVote,
    GameEnd
}

public class Game : MonoBehaviour {
    private int HandSizeLimit = 7;

    private List<Player> players;
    private Queue<Player> turnQueue;
    public Player currentPlayer;

    public GameObject playerObj;
    public GameState gameState;
    public List<CardData> deck;
    public List<CardData> goalDeck;

	// Use this for initialization
	void Start () {
        deck = new List<CardData>();
        goalDeck = new List<CardData>();
        players = new List<Player>();
        turnQueue = new Queue<Player>();

        // Placeholder: starting at DrawCards until net connection code is in
        AddPlayer("testPlayer", true);

        gameState = GameState.Loading;
	}

    void AddPlayer(string name, bool isYou)
    {
        Player newPlayer = gameObject.AddComponent<Player>();
        newPlayer.name = name;
        newPlayer.isYou = isYou;
        players.Add(newPlayer);
    }
	
	// Update is called once per frame
	void Update () {
        if (gameState == GameState.MainMenu)
        {

        }
        else if (gameState == GameState.Loading)
        {
            LoadGame();
            gameState = GameState.DrawCards;
        }
	    else if (gameState == GameState.DrawCards)
        {
            DrawCards();
            gameState = GameState.DrawGoals;
        }
        else if (gameState == GameState.DrawGoals)
        {
            DrawGoals();

            foreach (Player p in players)
            {
                turnQueue.Enqueue(p);
            }

            gameState = GameState.MakeStory;
        }
        else if (gameState == GameState.MakeStory)
        {
            ProcessTurn();
        }
	}

    void ProcessTurn()
    {
        if (currentPlayer == null)
        {
            currentPlayer = turnQueue.Dequeue();
            turnQueue.Enqueue(currentPlayer);
        }
    }

    void LoadGame()
    {
        // TODO: Load cards from database instead of this placeholder
        for (int i = 0; i < 40; i++)
        {
            CardData c = new CardData();
            c.type = CardType.Noun;
            c.title = string.Format("Test card {0}", i);
            deck.Add(c);
        }

        for (int i = 0; i < 40; i++)
        {
            CardData c = new CardData();
            c.type = CardType.Goal;
            c.title = string.Format("Test goal {0}", i);
            goalDeck.Add(c);
        }
    }

    void DealCard(Player p)
    {
        // This is host-side code; p.AddCard should be an RPC call here

        if (deck.Count > 0)
        {
            CardData nextCard = deck[deck.Count - 1];
            deck.RemoveAt(deck.Count - 1);
            p.AddCard(nextCard);
        }
    }

    void DealGoal(Player p)
    {
        if (goalDeck.Count > 0)
        {
            CardData nextGoal = goalDeck[goalDeck.Count - 1];
            goalDeck.RemoveAt(goalDeck.Count - 1);
            p.AddGoal(nextGoal);
        }
        // TODO else { reshuffle }
    }

    void DrawCards()
    {
        foreach (Player p in players)
        {
            for (int i = p.hand.Count; i < HandSizeLimit; i++)
            {
                DealCard(p);
            }
        }
    }

    void DrawGoals()
    {
        foreach (Player p in players)
        {
            DealGoal(p);
        }
    }
}
