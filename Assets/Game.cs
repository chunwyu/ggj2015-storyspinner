using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum ProcessState
{
    MainMenu,
    WaitingForConnections,
    InGame
}

public enum GameState
{
    DrawCards,
    DrawGoals,
    MakeStory,
    ScoringVote,
    GameEnd
}

public class Game : MonoBehaviour {

    public List<Player> players;
    public GameObject playerObj;

	// Use this for initialization
	void Start () {
	            
	}

    void AddPlayer(string name, bool isYou)
    {
        GameObject ob = (GameObject)GameObject.Instantiate(playerObj, Vector3.zero, Quaternion.identity);
        players.Add(ob.GetComponent<Player>());
    }
	
	// Update is called once per frame
	void Update () {
	    
	}
}
