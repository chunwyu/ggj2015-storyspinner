using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameLobby : MonoBehaviour
{
	public InputField mPlayerNameJoin;
	public InputField mPlayerNameHost;
	public GameObject mPlayerList;
	public GameObject mPlayerDisplayPrefab;
	public NetworkingController mNetworkController;
	public Animator mPlayerListAnimation;
	public Button mStartButton;
	public Game mGame;
	public GameObject mTextDisplay;
		
	private string[] mPlayers;
	private string mLocalPlayerName;
	private int mPlayersConnected = 0;
	private List <Player> mPlayerArray;
	
	void Start ()
	{
		mNetworkController.RefreshHostList ();
		if (Network.isServer)
		{
			mPlayers = new string[Network.maxConnections];
			mLocalPlayerName = mPlayerNameHost.text;
			networkView.RPC("AddPlayer", RPCMode.All, mPlayerNameHost.text);
			mStartButton.interactable = true;
		}
		else
		{
			mStartButton.gameObject.SetActive (false);
			mStartButton.interactable = false;
		}
	}
	
	//Called on connection to server. Notifies server that a new player has joined game.
	void OnConnectedToServer ()
	{
		mPlayers = new string[Network.maxConnections];
		
		if (Network.isClient)
		{
			mLocalPlayerName = mPlayerNameJoin.text;
		}
		else if (Network.isServer)
		{
			mLocalPlayerName = mPlayerNameHost.text;
		}
		networkView.RPC("AddPlayer", RPCMode.Server, mLocalPlayerName);

	}
	
	//Only called with RPCMode.Server
	[RPC]
	void AddPlayer (string playerName, NetworkMessageInfo info)
	{
		mPlayers[mPlayersConnected++] = playerName;
		
		//Debug logging of connected players
		Debug.Log ("Connected player: " + mPlayers[mPlayersConnected - 1]);
		
		string allPlayers = mPlayers[0];
		for (int i = 1; i < mPlayersConnected; i++)
		{
			allPlayers += "," + mPlayers[i];
		}
		
		networkView.RPC ("RecievePlayers", RPCMode.All, allPlayers);
		mGame.AddPlayer(playerName, info.sender);
	}
	
	//Only called with RPCMode.Server
	[RPC]
	void RemovePlayer (string playerName)
	{
		bool bPlayerFound = false;
		
		for (int i = 0; i < mPlayersConnected && !bPlayerFound; i++)
		{
			if (mPlayers[i].Equals (playerName))
			{
				bPlayerFound = true;
				mPlayersConnected--;
				for (int j = i; j < Network.maxConnections - 1; j++)
				{
					mPlayers[j] = mPlayers[j + 1];
				}
			}
		}
		
		Debug.Log ("Disconnected player: " + playerName);
		
		string allPlayers = mPlayers[0];
		for (int i = 1; i < mPlayersConnected; i++)
		{
			allPlayers += "," + mPlayers[i];
		}
		
		networkView.RPC ("RecievePlayers", RPCMode.Others, allPlayers);
	}
	
	//Only called by server
	[RPC]
	void RecievePlayers (string allPlayers)
	{
		string[] playerNames = allPlayers.Split (',');

		bool bNameFound;
		//looking for a new player
		foreach (string playerName in playerNames)
		{
			bNameFound = false;
			foreach (Transform child in mPlayerList.transform)
			{
				PlayerDisplay display = child.GetComponent <PlayerDisplay> ();
				if (display.GetName ().Equals (playerName))
				{
					bNameFound = true;
				}
			}
			
			//Here we have found a new player, so we add them to the player list.
			if (!bNameFound)
			{
				GameObject newListing = (GameObject) Instantiate (mPlayerDisplayPrefab);
				PlayerDisplay newDisplay = newListing.GetComponent <PlayerDisplay> ();
				newDisplay.SetName (playerName);
				newDisplay.SetLobby (this);
				RectTransform newTransform = newListing.GetComponent <RectTransform> ();
				//newTransform.localScale = new Vector3 (1,1,1);
				newTransform.SetParent (mPlayerList.transform, false);
				
				if (newDisplay.GetName ().Equals (mLocalPlayerName))
				{
					newDisplay.SetInteractableToggle (true);
				}
			}
		}
		//looking for a player that was removed
		foreach (Transform child in mPlayerList.transform)
		{
			PlayerDisplay display = child.GetComponent <PlayerDisplay> ();
			bNameFound = false;
			
			foreach (string playerName in playerNames)
			{
				if (playerName.Equals(display.GetName ()))
				{
					bNameFound = true;
				}
			}
			
			if (!bNameFound)
			{	
				Destroy (child.gameObject);
			}
		}
	}
	
	public void ChangeReadyStatus (bool bReady, string playerName)
	{
		networkView.RPC ("RPCReadyStatus", RPCMode.All, bReady, playerName);
	}
	
	[RPC]
	public void RPCReadyStatus (bool bReady, string playerName)
	{
		foreach (Transform child in mPlayerList.transform)
		{
			PlayerDisplay display = child.GetComponent <PlayerDisplay> ();
			if (display.GetName ().Equals (playerName))
			{
				display.SetReady (bReady);
			}
		}
	}
	
	public string[] GetConnectedPlayers()
	{
		return mPlayers;
	}
	
	void OnDisconnectFromServer ()
	{
		Debug.Log ("Disconnected from Server!");
	}
	
	void Update()
	{

	}
	
	public void StartGame ()
	{
		bool bAllReady = true;
		
		foreach (Transform child in mPlayerList.transform)
		{
			PlayerDisplay display = child.GetComponent <PlayerDisplay> ();
			if (!display.GetReady ())
			{
				bAllReady = false;
			}
		}
		
		if (bAllReady)
		{
			networkView.RPC ("RPCStartGame", RPCMode.All);
		}
	}
	
	[RPC]
	public void RPCStartGame ()
	{
		//CODE TO START GAME
		ActivateMenu ();
		mGame.ChangeState (GameState.Loading);
		mTextDisplay.SetActive (true);
		//mStartButton.gameObject.SetActive (false);
	}
	
	public void ActivateMenu ()
	{
		mPlayerListAnimation.SetBool ("IsActive", true);
	}
	
}


