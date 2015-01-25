using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Music : MonoBehaviour {

    private AudioSource source;
    private AudioClip music;
    private int musicPosition = 0;
    private List<string> musicList = new List<string>();
	// Use this for initialization
	void Start () {

        source = (AudioSource)gameObject.AddComponent("AudioSource");
        source.loop = true;

        musicList.Add("Music/Kites");
        musicList.Add("Music/11_Temple_of_Rain");
        musicList.Add("Music/layer1");
        musicList.Add("Music/layer2");
        musicList.Add("Music/layer3");
        musicList.Add("Music/layer4");

        music = PickMusic(musicList[musicPosition]);
        PlayMusic(music);
	}

	// Update is called once per frame
	void Update () {
	
	}

    void PlayMusic(AudioClip musicFile)
    {
        source.clip = musicFile;
        source.Play();
    }

    AudioClip PickMusic(string resourceName)
    {
        return (AudioClip)Resources.Load(resourceName);
    }

    public void IncrementMusicPosition()
    {
        if (musicPosition == (musicList.Count - 1))
        {
            musicPosition = 0;
        }
        else
        {
            musicPosition++;
        }
        music = PickMusic(musicList[musicPosition]);
        PlayMusic(music);
    }

    public void MuteMusic()
    {
        source.Stop();
    }

}
