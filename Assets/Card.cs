using UnityEngine;
using System.Collections;

public enum WordType
{
    Noun,
    Verb,
    Adjective
};

public class CardData
{
    public string title;
    public string description;
    public string graphicPath;
    public WordType type;

    public override string ToString()
    {
        string strType = "";
        switch (type)
        {
            case WordType.Noun: strType = "Noun"; break;
            case WordType.Verb: strType = "Verb"; break;
            case WordType.Adjective: strType = "Adjective"; break;
            default: strType = "UNKNOWN_TYPE"; break;
        }

        return string.Format("Card {0} {1} {2}", strType, title, description);
    }
}

public class Card : MonoBehaviour {
    public CardData data;

	// Use this for initialization
	void Start () {
	}

    void Init ()
    {
        // (1) placeholder card image
        if (data.type == WordType.Noun)
        {
            SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = Resources.Load<Sprite>("Origs/WordCardNounExampleOrig300DPI");
        }

        // (1) replace with
        // spriteRenderer.sprite = Resources.Load<Sprite>(graphicPath);
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}