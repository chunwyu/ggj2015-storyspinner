using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {

    public List<CardData> hand;
    public bool isYou;
    int points;

	// Use this for initialization
	void Start () {
        hand = new List<CardData>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnGUI()
    {
        if (isYou)
        {
            if (hand.Count == 0)
            {
                GUI.Label(new Rect(10, 10, 200, 20), "HAND IS EMPTY");
            }

            for (int i = 0; i < hand.Count; i++)
            {
                GUI.Label(new Rect(10, 10 + 25*i, 200, 20), hand[i].ToString());
            }
        }
    }

    public void AddCard(CardData c)
    {
        hand.Add(c);

        if (isYou)
        {
            //var cardObject = GameObject.Instantiate(Resources.Load("Card", Vector3.zero, Quaternion.identity));
        }
    }
}
