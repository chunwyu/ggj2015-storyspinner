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

    public GameObject mCardButtonObj;
    public GameObject mCardList;
    
    private Player mLocalPlayer;

	// Use this for initialization
	void Start () {
        deck = new List<CardData>();
        goalDeck = new List<CardData>();
        players = new List<Player>();
        turnQueue = new Queue<Player>();

        // Placeholder: starting at DrawCards until net connection code is in

        gameState = GameState.MainMenu;
        mLocalPlayer = new Player ();
	}

    public void AddPlayer(string name, bool isYou, NetworkPlayer player)
    {
        Player newPlayer = new Player (name, this, player);
        newPlayer.playerName = name;
        
		if (player.Equals (Network.player))
		{
			newPlayer.isYou = true;
			mLocalPlayer = newPlayer;
		}
		else
		{
			newPlayer.isYou = false;
		}
		
		players.Add(newPlayer);
    }
    
    public void ChangeState (GameState state)
    {
    	gameState = state;
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
    	if (Network.isServer)
    	{
	        if (currentPlayer == null)
	        {
	            currentPlayer = turnQueue.Dequeue();
	            turnQueue.Enqueue(currentPlayer);
	        }
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
    	if (Network.isServer)
    	{
	        if (deck.Count > 0)
	        {
	            CardData nextCard = deck[deck.Count - 1];
	            deck.RemoveAt(deck.Count - 1);
	            p.AddCard (nextCard);

	            if (!p.Equals (mLocalPlayer))
	            {
	            	networkView.RPC ("RecieveCard", p.mPlayer, DataAccess.GetJSONfromCard (nextCard));
	        	}
                else
                {
                    GameObject newCard = (GameObject)GameObject.Instantiate(mCardButtonObj);
                    Card cardScript = newCard.GetComponent<Card>();

                    RectTransform newTransform = newCard.GetComponent<RectTransform>();
                    newTransform.SetParent(mCardList.transform, false);

                    cardScript.data = nextCard;
                    cardScript.Init();
                }

				//produces an error when called on the server that is meaningless but that i can't remove
	            networkView.RPC ("RecieveCard", p.mPlayer, DataAccess.GetJSONfromCard (nextCard));

	        }
        }
    }
    
    [RPC]
    void RecieveCard (string cardData)
    {
    	CardData data = DataAccess.GetCardFromJSON (cardData);
    	mLocalPlayer.AddCard (data);
    }

    void DealGoal(Player p)
    {
    	if (Network.isServer)
    	{
	        if (goalDeck.Count > 0)
	        {
	            CardData nextGoal = goalDeck[goalDeck.Count - 1];
	            goalDeck.RemoveAt(goalDeck.Count - 1);
	            p.AddGoal(nextGoal);
				//produces an error when called on the server that is meaningless but that i can't remove
	            networkView.RPC ("RecieveCard", p.mPlayer, DataAccess.GetJSONfromCard (nextGoal));
	        }
        }
        // TODO else { reshuffle }
    }
    
    [RPC]
    void RecieveGoal(string cardData)
    {
    	CardData data = DataAccess.GetCardFromJSON (cardData);
    	mLocalPlayer.AddGoal (data);
    }

    void DrawCards()
    {
    	if (Network.isServer)
    	{
	        foreach (Player p in players)
	        {
	            for (int i = p.hand.Count; i < HandSizeLimit; i++)
	            {
	                DealCard(p);
	            }
	        }
	        
	        foreach (Player p in players)
	        {
	        	Debug.Log ("Hand for player: " + p.playerName);
	        	foreach (CardData data in p.hand)
	        	{
	        		Debug.Log (data);
	        	}
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
