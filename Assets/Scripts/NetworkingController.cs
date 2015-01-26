using UnityEngine;
using UnityEngine.UI;

public class NetworkingController : MonoBehaviour
{
	//constants for game type and name
	private const string TYPE_NAME = "TaleSpinner";
	private const string GAME_NAME = "RoomName";
	private const string SERVER_IP = "24.19.57.246";
	private const int LISTEN_PORT = 25000;
	private const int MAX_CONNECTIONS = 8;
	
	private HostData [] mHostList;
	
	public InputField mRoomName;
	
	public void RefreshHostList ()
	{
		MasterServer.ClearHostList ();
		MasterServer.RequestHostList (TYPE_NAME);
	}
	
	void OnMasterServerEvent (MasterServerEvent msEvent)
	{
		if (msEvent == MasterServerEvent.HostListReceived)
		{
			Debug.Log ("Host List Recieved");
			mHostList = MasterServer.PollHostList ();
			if (mHostList != null)
			{
				foreach (HostData data in mHostList)
				{
					Debug.Log ("Host: " + data.gameName);
				}
			}
		}
	}
	
	//Will join the best server (somehow). for now just joins first with empty slot in list.
	public void JoinServer ()
	{
		bool bConnected = false;
		
		if (mHostList != null)
		{
			for (int i = 0; i < mHostList.Length && !bConnected; i++)
			{
				if (mHostList[i].connectedPlayers < mHostList[i].playerLimit)
				{
					Network.Connect (mHostList[i]);
					bConnected = true;
					Debug.Log ("Connecting to server " + mHostList[i].gameName);
				}
			}
			if (!bConnected)
			{
				//No open spots in games, show error
			}
		}
		else
		{
			//No currently existing games, show error
		}
	}
	
	public HostData[] GetHosts ()
	{
		return mHostList;
	}
	
	void OnConnectedToServer ()
	{
		Debug.Log ("Server Joined!");
	}
	
	public void StartServer ()
	{
		Debug.Log ("Call to StartServer");
		Network.InitializeServer (MAX_CONNECTIONS, LISTEN_PORT, !Network.HavePublicAddress ());
		MasterServer.RegisterHost (TYPE_NAME, mRoomName.text);
	}
	
	public void UnregisterServer ()
	{
		MasterServer.UnregisterHost ();
	}
	
	void OnServerInitialized ()
	{
		Debug.Log ("Server Initialized.");
	}
	
	void Start ()
	{
		DontDestroyOnLoad (this);
		MasterServer.ipAddress = SERVER_IP;
		
		RefreshHostList ();
	}
	
	void Update ()
	{
	
	}
}

