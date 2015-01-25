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

    public string getTypeString(CardType type)
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
        return strType;
    }

    public override string ToString()
    {
        return string.Format("Card {0} {1} {2}", getTypeString(type), title, description);
    }
}

// Script to actually run on Card GameObject
public class Card : MonoBehaviour {
    public CardData data;
	public Game mGame;
	
	public Vector3 mMouseOffset;
	public RectTransform mParent;
	
	public RectTransform mDropZone;

    public Sprite nounBlank;
    public Sprite verbBlank;
    public Sprite adjectiveBlank;
    public Sprite goalBlank;
	
	// Use this for initialization
	void Start () {

	}

    public void Init (Game game, RectTransform dropZone)
    {
        DebugUtils.Assert(data.gameObj == null);

        data.gameObj = this.gameObject;
        Image cardBack = gameObject.GetComponent<Image>();

        // (1) placeholder card image
        if (data.type == CardType.Noun)
        {
            Sprite load = Resources.Load<Sprite>("CardBlanks/Noun_Card_Blank_HiRes");
            cardBack.sprite = nounBlank;
        }
        else if (data.type == CardType.Adjective)
        {
            cardBack.sprite = verbBlank;
        }
        else if (data.type == CardType.Verb)
        {
            cardBack.sprite = Resources.Load<Sprite>("CardBlanks/Verb_Card_Blank_HiRes");
        }
        else if (data.type == CardType.Goal)
        {
            cardBack.sprite = Resources.Load<Sprite>("CardBlanks/Goal_Card_Blank_HiRes");
        }

        // (1) replace with
        // cardBack.sprite = Resources.Load<Sprite>(graphicPath);

        Text cardTitle = GetComponentInChildren<Text>();
        cardTitle.text = data.title;

        Image cardImage = gameObject.transform.FindChild("CardGraphic").GetComponent<Image>();
        //string path = string.Format("CardGraphics/{0}_{1}", data.getTypeString(data.type), data.graphicPath);
        cardImage.sprite = Resources.Load<Sprite>(data.graphicPath);
        
        mGame = game;
        mDropZone = dropZone;
    }
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnMouseDown ()
	{
		mMouseOffset = transform.position - Input.mousePosition;
		RectTransform rectTransform = GetComponent <RectTransform> ();
		mParent = rectTransform.parent.GetComponent <RectTransform> ();
		rectTransform.SetParent (rectTransform.parent.parent);
	}
	
	void OnMouseDrag ()
	{
		transform.position = Input.mousePosition + mMouseOffset;
	}
	
	void OnMouseUp ()
	{
		RectTransform rTransform = GetComponent <RectTransform> ();
		if (rTransform.rect.Overlaps(mDropZone.rect, true))
		{
			//CARD LANDED IN CARD ZONE IT IS NOW PLAYED
			mGame.CardPlayed (data);
		}
		rTransform.SetParent (mParent);
	}
}
