using UnityEngine;
using System.Collections;

public class Music : MonoBehaviour {

    public AudioSource source;

	// Use this for initialization
	void Start () {

        source = (AudioSource)gameObject.AddComponent("AudioSource");
        AudioClip music;
        music = (AudioClip)Resources.Load("Music/Kites");
        source.clip = music;
        source.loop = true;
        source.Play();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
