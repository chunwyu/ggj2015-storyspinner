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

    public List<Player> players;
    public GameObject playerObj;
    public GameState gameState;
    public List<CardData> deck;

	// Use this for initialization
	void Start () {
        deck = new List<CardData>();
        players = new List<Player>();

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
        if (gameState == GameState.Loading)
        {
            LoadGame();
            gameState = GameState.DrawCards;
        }
	    else if (gameState == GameState.DrawCards)
        {
            DrawCards();
            gameState = GameState.DrawGoals;
        }
	}

    void LoadGame()
    {
        for (int i = 0; i < 40; i++)
        {
            CardData c = new CardData();
            c.type = WordType.Noun;
            c.title = string.Format("Test card {0}", i);
            deck.Add(c);
        }
    }

    void DealCard(Player p)
    {
        // RPC call here

        if (deck.Count > 0)
        {
            CardData nextCard = deck[deck.Count - 1];
            deck.RemoveAt(deck.Count - 1);
            p.AddCard(nextCard);
        }
    }

    void DrawCards()
    {
        foreach (Player p in players)
        {
            for (int i = p.hand.Count; i <= HandSizeLimit; i++)
            {
                DealCard(p);
            }
        }
    }
}
