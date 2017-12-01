using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

    // Sounds
    public AudioClip goalSound;

    private Dictionary<string, AudioClip> sounds;

    private AudioSource soundSource;
    private AudioSource musicSource;

    public Dictionary<string, AudioClip> Sounds
    {
        get
        {
            return sounds;
        }

        set
        {
            sounds = value;
        }
    }

    private void Start()
    {
        Sounds = new Dictionary<string, AudioClip>
        {
            { "Goal", goalSound }
        };
        soundSource = GetComponent<AudioSource>();

        AudioSource[] sources = GetComponents<AudioSource>();
        soundSource = sources[0];
        musicSource = sources[1];
    }

    public void PlaySound(string soundName)
    {
        soundSource.PlayOneShot(Sounds[soundName]);
    } 
}
