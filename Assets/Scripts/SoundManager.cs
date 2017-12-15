using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

    // Sounds
    public AudioClip goalSound;
    public AudioClip wallHit;
    public AudioClip racketHit;
    public AudioClip trainingInitial;
    public AudioClip trainingGoal;
    public AudioClip trainingService;
    public AudioClip trainingFree;
    public AudioClip niceShot;
    public AudioClip outShot;


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
            { "Goal", goalSound },
            { "WallHit", wallHit },
            { "RacketHit", racketHit},
            { "TrainingInitial", trainingInitial },
            { "TrainingGoal", trainingGoal },
            { "TrainingService", trainingService },
            { "TrainingFree", trainingFree },
            { "NiceShot", niceShot},
            { "Out", outShot }
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
