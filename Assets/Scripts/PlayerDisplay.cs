using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerDisplay : MonoBehaviour 
{	
	public Text mDisplayText;
	
	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
	
	public void SetName (string name)
	{
		mDisplayText.text = name;
	}
}
