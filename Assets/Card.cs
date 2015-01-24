using UnityEngine;
using System.Collections;

public enum WordType
{
    Noun,
    Verb,
    Adjective
};

public class Card : MonoBehaviour {

    string title;
    string description;
    string graphicPath;
    WordType type;

	// Use this for initialization
	void Start () {
        this.type = WordType.Noun; //default
	}

    void Init ()
    {
        // (1) placeholder card image
        if (type == WordType.Noun)
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