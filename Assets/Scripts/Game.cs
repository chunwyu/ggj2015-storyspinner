﻿using UnityEngine;
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

	private const string SENTENCE_VOTE = "Should we end this sentence?";
	private const string GOAL_VOTE = "Was this goal accomplished?";
    private int HandSizeLimit = 7;
    private int ScoringLimit = 10;

    private List<Player> players;
    private Queue<Player> turnQueue;
    private List<Player> votedPlayers;
    public Player currentPlayer;

    public GameObject playerObj;
    public GameState gameState;
    public List<CardData> deck;
    public List<CardData> goalDeck;

    public GameObject mCardButtonObj;
    public GameObject mVoteCardObj;
    public GameObject mCardList;
    public GameObject mEndSentenceButton;
    
    public GameObject mPrefixPanel;
    public InputField mPrefixInput;
    
    public GameObject mVotingPanel;
    public Text mVotingText;
    public GameObject mTurnNotificationPanel;
    
    private Player mLocalPlayer;
    private bool mbIsMyTurn;
    private bool mbNextTurnReady;
    private Card mCurrentlySelectedCard;
    
    
    public RectTransform mDropZone;
    public Text mPlayedText;

    public GameObject mWinnerDisplayObj;
    public GameObject mWinnerTextObj;
    public GameObject mCanvas;
    private bool winnerShown;
    
    private int mTallyFor, mTallyAgainst;

	// Use this for initialization
	void Start () {
        deck = new List<CardData>();
        goalDeck = new List<CardData>();
        players = new List<Player>();
        votedPlayers = new List<Player> ();
        turnQueue = new Queue<Player>();

        // Placeholder: starting at DrawCards until net connection code is in

		gameState = GameState.MainMenu;
        mLocalPlayer = new Player ();
        winnerShown = false;
        mbNextTurnReady = true;
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
			SetUpBoard ();
            /* At some condition:
             * gameState = GameState.ScoringVote;
             */
        }
        else if (gameState == GameState.ScoringVote)
        {
            ScoringVote();
            //After votes:
            foreach (Player p in players)
            {
                if (p.score > ScoringLimit)
                {
                    gameState = GameState.GameEnd;
                }
            }
            gameState = GameState.MakeStory;
        }
        else if (gameState == GameState.GameEnd)
        {
            if (!winnerShown)
            {
                ProcessEndGame();
            }
            winnerShown = true;
        }
	}

    void ProcessTurn()
    {
    	if (Network.isServer)
    	{
	        if (mbNextTurnReady)
	        {
	            currentPlayer = turnQueue.Dequeue();
	            //TURN LOGIC GOES HERE
	            if (!mLocalPlayer.mPlayer.Equals (currentPlayer.mPlayer))
	            {
					networkView.RPC ("RecieveTurn", currentPlayer.mPlayer);
	            }
	            else
	            {
	            	mbIsMyTurn = true;
	            }
	            
	            turnQueue.Enqueue(currentPlayer);
	            
	            mbNextTurnReady = false;
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

        DataAccess da = new DataAccess();
        List<CardData> cardList = new List<CardData>();
        cardList = da.readCard();

        foreach (var c in cardList)
        {
            if (c.type != CardType.Goal)
            {
                deck.Add(c);
            }
            else
            {
                goalDeck.Add(c);
            }

        }
        ShuffleDecks ();

        // TODO: Load cards from database instead of this placeholder
        //for (int i = 0; i < 40; i++)
        //{
        //    CardData c = new CardData();
        //    c.type = CardType.Noun;
        //    c.title = string.Format("Test card {0}", i);
        //    deck.Add(c);
        //}

        //for (int i = 0; i < 40; i++)
        //{
        //    CardData c = new CardData();
        //    c.type = CardType.Goal;
        //    c.title = string.Format("Test goal {0}", i);
        //    goalDeck.Add(c);
        //}
    }
    
    void ShuffleDecks()
    {
    	for (int i = 0; i < deck.Count; i++)
    	{
    		int random = (int) (Random.value * (deck.Count - 1));
    		var temp = deck[i];
    		deck[i] = deck[random];
    		deck[random] = temp;
    	}
		for (int i = 0; i < goalDeck.Count; i++)
		{
			int random = (int) (Random.value * (goalDeck.Count - 1));
			var temp = goalDeck[i];
			goalDeck[i] = goalDeck[random];
			goalDeck[random] = temp;
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
    	bool bCardFound;
    	foreach (CardData card in mLocalPlayer.hand)
    	{
    		bCardFound = false;
    		foreach (Transform child in mCardList.transform)
    		{
    			Card c = child.GetComponent <Card> ();
    			if (c.data.title.Equals (card.title))
    			{
    				bCardFound = true;
    			}
    		}
    		if (!bCardFound)
    		{
				MakeNewCardDisplay (card);
			}
    	}
    	
    	mTurnNotificationPanel.SetActive (mbIsMyTurn);
    	mEndSentenceButton.SetActive (mbIsMyTurn);

    }
    
    
    public void ClickEventMethod (Button b)
    {
    	if (mbIsMyTurn)
    	{
    		Card selectedCard = b.gameObject.GetComponent <Card> ();
    		mCurrentlySelectedCard = selectedCard;
    		Debug.Log ("Selected Card: " + selectedCard.data.title);
    		//possibly display a dialog to allow players to add extra connectors between their words
    		
    		mPrefixPanel.SetActive (true);
    	}
    }
    
    public void SubmitSelection ()
    {    
		if (!Network.isServer)
		{
			networkView.RPC ("SendSelection", RPCMode.Server, mPrefixInput.text + " " + mCurrentlySelectedCard.data.title);
		}
		else
		{
			SendSelection (mPrefixInput.text + " " + mCurrentlySelectedCard.data.title);
		}
		mbIsMyTurn = false;
		mPrefixPanel.SetActive (false);
		mLocalPlayer.hand.Remove (mCurrentlySelectedCard.data);
		Destroy (mCurrentlySelectedCard.data.gameObj);
    }
    
    public void CancelSelection ()
    {
    	mPrefixPanel.SetActive (false);
    	mPrefixInput.text = "";
    }
    
    [RPC]
    public void SendSelection (string selection)
    {
    	mPlayedText.text += selection + " ";
    	networkView.RPC ("DistributeStory", RPCMode.All, mPlayedText.text);
    	mbNextTurnReady = true;
    	mPrefixInput.text = "";
    	DealCard (currentPlayer);
    }
    
    [RPC]
    public void DistributeStory (string currentStory)
    {
    	mPlayedText.text = currentStory;
    }

    // Sorry for duplication, but I figure I'd leave this open for adjustments.
    void MakeVotingCardDisplay(CardData cardData)
    {
        GameObject newVoteObj = (GameObject)GameObject.Instantiate(mVoteCardObj);
        Card cardScript = newVoteObj.GetComponent<Card>();

        RectTransform newTransform = newVoteObj.GetComponent<RectTransform>();
        newTransform.SetParent(mCardList.transform, false);

        cardScript.data = cardData;
        cardScript.Init(this, mDropZone);
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

    void ScoringVote()
    {

    }

    void ProcessEndGame()
    {

        List<Player> ranking = new List<Player>(players);

        // sort by score
        ranking.Sort();
        ranking.Reverse();

        List<Player> winners = new List<Player>();

        // top person
        winners.Add(ranking[0]);

        // look for ties
        if (ranking.Count > 1)
        {
            for (int i = 1; i < ranking.Count; i++)
            {
                if (ranking[i].score == winners[0].score)
                {
                    winners.Add(ranking[i]);
                }
            }
        }

        // pop up the winner image
        GameObject winnerDisplay = (GameObject)Instantiate(mWinnerDisplayObj);
        RectTransform winnerRect = winnerDisplay.GetComponent<RectTransform>();
        winnerRect.SetParent(mCanvas.transform, false);

        foreach (Player winner in winners)
        {
            GameObject winnerTextObj = (GameObject)Instantiate(mWinnerTextObj);
            Text winnerText = winnerTextObj.GetComponent<Text>();
            winnerText.text = winner.playerName;
            RectTransform newTextTransform = winnerTextObj.GetComponent<RectTransform>();
            newTextTransform.SetParent(winnerDisplay.transform, false);
        }
    }
    
    public void CardPlayed (CardData card)
    {
    	mPlayedText.text += card.title;
    }
    
	public void StartEndSentenceVote ()
	{
		if (mbIsMyTurn)
		{
			//TODO: check some things here?
			if (!Network.isServer)
			{
				networkView.RPC ("RequestSentenceVote", RPCMode.Server);
			}
			else
			{
				RequestSentenceVote ();
			}
		}
	}
	
	[RPC]
	public void RequestSentenceVote ()
	{
		networkView.RPC ("StartSentenceVote", RPCMode.All);
		votedPlayers.Clear ();
	}
	
	[RPC]
	public void StartSentenceVote ()
	{
		mVotingPanel.SetActive (true);
		mVotingText.text = SENTENCE_VOTE;
		mTallyFor = mTallyAgainst = 0;
	}
	
	public void SubmitVote (bool vote)
	{
		if (Network.isClient)
		{
			networkView.RPC ("RecieveVote", RPCMode.Server, vote);
		}
		else
		{
			votedPlayers.Add (mLocalPlayer);
			if (vote)
			{
				mTallyFor++;
			}
			else
			{
				mTallyAgainst++;
			}
		}
	}
	
	[RPC]
	public void RecieveVote(bool vote, NetworkMessageInfo info)
	{
		Player sendingPlayer = null;		
		foreach (Player p in players)
		{
			if (p.mPlayer.Equals (info.sender))
			{
				sendingPlayer = p;
			}
		}
		
		if (!votedPlayers.Contains (sendingPlayer) && sendingPlayer != null)
		{
			votedPlayers.Add (sendingPlayer);
			if (vote)
			{
				mTallyFor++;
			}
			else
			{
				mTallyAgainst++;
			}
		}
		
		if (votedPlayers.Count == players.Count)
		{
			if (mTallyFor >= mTallyAgainst)
			{
				networkView.RPC ("RecieveResults", RPCMode.All, true);
			}
			else
			{
				networkView.RPC ("RecieveResults", RPCMode.All, false);
			}
		}
	}
	
	[RPC]
	public void RecieveResults (bool success)
	{
		if (success)
		{
			mVotingText.text = "The ayes have it";
		}
		else
		{
			mVotingText.text = "They nays have it";
		}
	}
}
