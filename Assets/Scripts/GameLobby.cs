using UnityEngine;
using UnityEngine.UI;

public class GameLobby : MonoBehaviour
{
	public InputField mPlayerNameJoin;
	public InputField mPlayerNameHost;

	private Player[] mPlayers;
	private Player mLocalPlayer;
	
	private int mPlayersConnected = 0;
	
	void Start ()
	{
	}
	
	//Called on connection to server. Notifies server that a new player has joined game.
	void OnConnectedToServer ()
	{
		mPlayers = new Player[Network.maxConnections];
		
		if (Network.isClient)
		{
			mLocalPlayer.playerName = mPlayerNameJoin.text;
		}
		else if (Network.isServer)
		{
			mLocalPlayer.playerName = mPlayerNameHost.text;
		}
		
		networkView.RPC("AddPlayer", RPCMode.Server, mLocalPlayer.name);
	}
	
	//Only called with RPCMode.Server
	[RPC]
	void AddPlayer (string playerName)
	{
		mPlayers[mPlayersConnected] = new Player ();
		mPlayers[mPlayersConnected++].playerName = playerName;
		
		//Debug logging of connected players
		Debug.Log ("Connected player: " + mPlayers[mPlayersConnected - 1].playerName);
		
		string allPlayers = mPlayers[0].playerName;
		for (int i = 0; i < mPlayersConnected; i++)
		{
			allPlayers += "," + mPlayers[i].playerName;
		}
		
		networkView.RPC ("RecievePlayers", RPCMode.Others, allPlayers);
	}
	
	//Only called with RPCMode.Server
	[RPC]
	void RemovePlayer (string playerName)
	{
		bool bPlayerFound = false;
		
		for (int i = 0; i < mPlayersConnected && !bPlayerFound; i++)
		{
			if (mPlayers[i].playerName.Equals (playerName))
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
		
		string allPlayers = mPlayers[0].playerName;
		for (int i = 0; i < mPlayersConnected; i++)
		{
			allPlayers += "," + mPlayers[i].playerName;
		}
		
		networkView.RPC ("RecievePlayers", RPCMode.Others, allPlayers);
	}
	
	//Only called by server with RPCMode.Others
	[RPC]
	void RecievePlayers (string allPlayers)
	{
		string[] playerNames = allPlayers.Split (',');
		bool bNameFound;
		//looking for a new player
		foreach (string playerName in playerNames)
		{
			bNameFound = false;
			foreach (Player player in mPlayers)
			{
				if (player.playerName.Equals (playerName))
				{
					bNameFound = true;
				}
			}
			
			//Here we have found a new player, so we add them to the player list.
			if (!bNameFound)
			{
				mPlayers[mPlayersConnected] = new Player ();
				mPlayers[mPlayersConnected++].playerName = playerName;
			}
		}
		//looking for a player that was removed
		foreach (Player player in mPlayers)
		{
			bNameFound = false;
			
			foreach (string playerName in playerNames)
			{
				if (playerName.Equals(player.playerName))
				{
					bNameFound = true;
				}
			}
			
			if (!bNameFound)
			{
				for (int i = 0; i < mPlayersConnected && !bNameFound; i++)
				{
					if (mPlayers[i].playerName.Equals (player.playerName))
					{
						bNameFound = true;
						mPlayersConnected--;
						for (int j = i; j < Network.maxConnections - 1; j++)
						{
							mPlayers[j] = mPlayers[j + 1];
						}
					}
				}
			}
		}
	}
	
	public Player[] GetConnectedPlayers()
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
	
}


