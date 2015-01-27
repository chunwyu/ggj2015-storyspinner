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
	public Game mGame;
	
	public Vector3 mMouseOffset;
	public RectTransform mParent;
	
	public RectTransform mDropZone;
	
	// Use this for initialization
	void Start () {

	}

    public void Init (Game game, RectTransform dropZone)
    {
        DebugUtils.Assert(data.gameObj == null);

        data.gameObj = this.gameObject;
        Image cardImage = gameObject.GetComponent<Image>();

        // (1) placeholder card image
        if (data.type == CardType.Noun)
        {
            cardImage.sprite = Resources.Load<Sprite>("Origs/WordCardNounExampleOrig300DPI");
        }
        else if (data.type == CardType.Adjective)
        {
            cardImage.sprite = Resources.Load<Sprite>("Origs/WordCardAdjectiveExampleOrig300DPI");
        }
        else if (data.type == CardType.Verb)
        {
            cardImage.sprite = Resources.Load<Sprite>("Origs/WordCardVerbExampleOrig300DPI");
        }
        else if (data.type == CardType.Goal)
        {
            cardImage.sprite = Resources.Load<Sprite>("Origs/GoalCardExampleOrig300DPI");
        }

        // (1) replace with
        // cardImage.sprite = Resources.Load<Sprite>(graphicPath);

        Text cardTitle = GetComponentInChildren<Text>();
        cardTitle.text = data.title;
        
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
