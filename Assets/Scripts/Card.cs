using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum CardType
{
    Noun = 0,
    Verb = 1,
    Adjective = 2,
    Goal = 3,
};

public class CardData
{
    public string title;
    public string description;
    public string graphicPath;
    public CardType type;
    public GameObject gameObj;

    public override string ToString()
    {
        string strType = "";
        switch (type)
        {
            case CardType.Noun: strType = "Noun"; break;
            case CardType.Verb: strType = "Verb"; break;
            case CardType.Adjective: strType = "Adjective"; break;
            case CardType.Goal: strType = "Goal"; break;
            default: strType = "UNKNOWN_TYPE"; break;
        }

        return string.Format("Card {0} {1} {2}", strType, title, description);
    }
}

// Script to actually run on Card GameObject
public class Card : MonoBehaviour {
    public CardData data;

	// Use this for initialization
	void Start () {

	}

    public void Init ()
    {
        DebugUtils.Assert(data.gameObj == null);

        data.gameObj = this.gameObject;

        // (1) placeholder card image
        if (data.type == CardType.Noun)
        {
            SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = Resources.Load<Sprite>("Origs/WordCardNounExampleOrig300DPI");
        }
        else if (data.type == CardType.Adjective)
        {
            SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = Resources.Load<Sprite>("Origs/WordCardAdjectiveExampleOrig300DPI");
        }
        else if (data.type == CardType.Verb)
        {
            SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = Resources.Load<Sprite>("Origs/WordCardVerbExampleOrig300DPI");
        }
        else if (data.type == CardType.Goal)
        {
            SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = Resources.Load<Sprite>("Origs/GoalCardExampleOrig300DPI");
        }

        // (1) replace with
        // spriteRenderer.sprite = Resources.Load<Sprite>(graphicPath);

        Text cardTitle = GetComponentInChildren<Text>();
        cardTitle.text = data.title;
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}