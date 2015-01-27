using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public enum GameState
{
    Loading,
    MainMenu,
    WaitingForConnections,
    DrawCards,
    DrawGoals,
    SetupBoard,
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
    private bool mbIsMyTurn;
    
    public RectTransform mDropZone;
    public Text mPlayedText;

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

    public void AddPlayer(string name, NetworkPlayer player)
    {
        Player newPlayer = new Player (name, this, player);
        newPlayer.playerName = name;
        
		if (player.Equals (Network.player))
		{
			newPlayer.isYou = true;
			mLocalPlayer = newPlayer;
		}
		else if (player.guid.Equals ("0"))
		{
			//this is the server, and it is us
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

            gameState = GameState.SetupBoard;
        }
        else if (gameState == GameState.SetupBoard)
        {
        	SetUpBoard ();
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
	            //TURN LOGIC GOES HERE
	            networkView.RPC ("ReceiveTurn", currentPlayer.mPlayer);
	            
	            turnQueue.Enqueue(currentPlayer);
	        }
        }
    }
    
    [RPC]
    void RecieveTurn ()
    {
    	mbIsMyTurn = true;
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

	            if (!mLocalPlayer.mPlayer.Equals (p.mPlayer))
	            {
                    networkView.RPC("RecieveCard", p.mPlayer, DataAccess.GetJSONfromCard(nextCard));
	        	}
	        }
        }
    }
    
    void SetUpBoard ()
    {
    	foreach (CardData card in mLocalPlayer.hand)
    	{
    		MakeNewCardDisplay (card);
    	}
    }
    
    
    public void ClickEventMethod (Button b)
    {
    	if (mbIsMyTurn)
    	{
    		Card selectedCard = b.gameObject.GetComponent <Card> ();
    		Debug.Log ("Selected Card: " + selectedCard.data.title);
    	}
    }

    void MakeNewCardDisplay(CardData newCard)
    {
        GameObject newCardObj = (GameObject)GameObject.Instantiate(mCardButtonObj);
        Card cardScript = newCardObj.GetComponent<Card>();

        RectTransform newTransform = newCardObj.GetComponent<RectTransform>();
        newTransform.SetParent(mCardList.transform, false);

        cardScript.data = newCard;
        cardScript.Init(this, mDropZone);
        
        Button cardButton = newCardObj.GetComponent <Button> ();
        cardButton.onClick.RemoveAllListeners ();
        cardButton.onClick.AddListener ( () => ClickEventMethod(cardButton));
        
    }
    
    [RPC]
    void RecieveCard (string cardData)
    {
    	CardData data = DataAccess.GetCardFromJSON (cardData);
    	mLocalPlayer.AddCard (data);

        //MakeNewCardDisplay(data);
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
				if (!mLocalPlayer.mPlayer.Equals (p.mPlayer))
				{
	            	networkView.RPC ("RecieveGoal", p.mPlayer, DataAccess.GetJSONfromCard (nextGoal));
	            }
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
    
    public void CardPlayed (CardData card)
    {
    	mPlayedText.text += card.title;
    }
}
