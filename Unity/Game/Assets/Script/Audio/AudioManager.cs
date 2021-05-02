using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public AudioClip[] playlist;
    public AudioSource audioSource;
    private int _musicIndex;
    public bool isPause {get; set;}
    
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        audioSource.clip = playlist[0];
        audioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (!audioSource.isPlaying && !isPause)
        {
            PlayNextSong();
        }
    }

    void PlayNextSong()
    {
        _musicIndex = (_musicIndex + 1) % playlist.Length;
        audioSource.clip = playlist[_musicIndex];
        audioSource.Play();
    }
}
