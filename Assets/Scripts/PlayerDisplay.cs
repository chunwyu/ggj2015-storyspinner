using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerDisplay : MonoBehaviour 
{	
	public Text mDisplayText;
	public Toggle mReadyToggle;
	public GameLobby mLobby;
	
	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
	
	public void SetLobby (GameLobby lobby)
	{
		mLobby = lobby;
	}
	
	public void SetName (string name)
	{
		mDisplayText.text = name;
	}
	
	public string GetName ()
	{
		return mDisplayText.text;
	}
	
	public void SetReady (bool bReady)
	{
		mReadyToggle.isOn = bReady;
	}
	
	public bool GetReady ()
	{
		return mReadyToggle.isOn;
	}
	
	public void SetInteractableToggle (bool bToggle)
	{
		mReadyToggle.interactable = bToggle;
	}
	
	public void ReadyChanged ()
	{
		mLobby.ChangeReadyStatus (mReadyToggle.isOn, mDisplayText.text);
	}
	
}
