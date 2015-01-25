using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DataAccess : MonoBehaviour {

    public static List<CardData> readCard()
    {
        string data = string.Empty;
        List<CardData> cardList = new List<CardData>();

        TextAsset saveLocation = new TextAsset();
        saveLocation = Resources.Load("Dictionary/Cards") as TextAsset;
        JSONObject cardDictionary = new JSONObject(saveLocation.text);

        foreach (JSONObject dictionaryList in cardDictionary.list)
        {
            foreach (var cardElements in dictionaryList.list)
            {
                CardType x = CardType.Noun;

                switch ((int)cardElements.GetField("type").n)
                {
                    case 0: x = CardType.Noun; break;
                    case 1: x = CardType.Verb; break;
                    case 2: x = CardType.Adjective; break;
                    case 3: x = CardType.Goal; break;
                }

                CardData card = new CardData()
                {
                    title = cardElements.GetField("title").str,
                    description = cardElements.GetField("description").str,
                    graphicPath = cardElements.GetField("graphicPath").str,
                    type = x

                };
                Debug.Log(card.ToString());
                cardList.Add(card);
            }
        }

        return cardList;
    }
    
    public static string GetJSONfromCard (CardData card)
    {
        return string.Format(" {\"CardArray\": [{\"title\": \"?title{0}\",\"description\": \"?description{1}\",\"type\": type{2},\"graphicPath\": \"graphicPath{3}\"}", card.title, card.description, card.type, card.graphicPath);
    }
    
    public static CardData GetCardFromJSON (string data)
    {
        JSONObject card = new JSONObject(data);
        CardData newCard = new CardData();

        foreach (JSONObject cardElements in card.list)
        {
                CardType x = CardType.Noun;

                switch ((int)cardElements.GetField("type").n)
                {
                    case 0: x = CardType.Noun; break;
                    case 1: x = CardType.Verb; break;
                    case 2: x = CardType.Adjective; break;
                    case 3: x = CardType.Goal; break;
                }

                newCard = new CardData()
                {
                    title = cardElements.GetField("title").str,
                    description = cardElements.GetField("description").str,
                    graphicPath = cardElements.GetField("graphicPath").str,
                    type = x

                };
        }

        return newCard;
    }

            
	// Use this for initialization
	void Start () 
    {
        readCard();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
