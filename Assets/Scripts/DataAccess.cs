using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DataAccess : MonoBehaviour {


    //public static void saveCard(List<Card> cardList)
    //{
        
    //    JSONObject json = new JSONObject(JSONObject.Type.OBJECT);
    //    JSONObject cardData = new JSONObject(JSONObject.Type.OBJECT);
    //    JSONObject cardArray = new JSONObject(JSONObject.Type.ARRAY);
    //    //j.AddField("field3", cardData);
    //    foreach(var c in cardList)
    //    {
    //        json.AddField("title", c.data.title);
    //        json.AddField("description", c.data.description);
    //        json.AddField("graphicPath", c.data.graphicPath);
    //        json.AddField("type", c.data.type );
    //    }
    //    //System.IO.File.WriteAllText(Application.dataPath, json.print());

    //}

    void accessData(JSONObject obj)
    {
        switch (obj.type)
        {
            case JSONObject.Type.OBJECT:
                for (int i = 0; i < obj.list.Count; i++)
                {
                    string key = (string)obj.keys[i];
                    JSONObject j = (JSONObject)obj.list[i];
                    Debug.Log(key);
                    accessData(j);
                }
                break;
            case JSONObject.Type.ARRAY:
                foreach (JSONObject j in obj.list)
                {
                    accessData(j);
                }
                break;
            case JSONObject.Type.STRING:
                Debug.Log(obj.str);
                break;
            case JSONObject.Type.NUMBER:
                Debug.Log(obj.n);
                break;
            case JSONObject.Type.BOOL:
                Debug.Log(obj.b);
                break;
            case JSONObject.Type.NULL:
                Debug.Log("NULL");
                break;
        }
    }



    public List<CardData> readCard()
    {
        string data = string.Empty;
        List<CardData> cardList = new List<CardData>();
        //Card card = new Card();
        //data = System.IO.File.ReadAllText(Application.dataPath);

        TextAsset saveLocation = new TextAsset();
        saveLocation = Resources.Load("Dictionary/Cards") as TextAsset;
        JSONObject j = new JSONObject(saveLocation.text);
        JSONObject results = new JSONObject(JSONObject.Type.ARRAY);
        accessData(j);

        foreach (JSONObject c in results.list)
        {
            CardType x = CardType.Noun;
            
            switch ((int)results.GetField("type").n)
            {
                case 0: x = CardType.Noun; break;
                case 1: x = CardType.Verb; break;
                case 2: x = CardType.Adjective; break;
                case 3: x = CardType.Goal; break;
            }

            CardData card = new CardData()
            {
                title = results.GetField("title").str,
                description = results.GetField("description").str,
                graphicPath = results.GetField("graphicPath").str,
               type = x

            };
            Debug.Log(card.ToString());
            cardList.Add(card);
        }

        return cardList;
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
