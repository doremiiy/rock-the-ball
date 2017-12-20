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
    private Dictionary<string, AudioSource> sources;


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

    public Dictionary<string, AudioSource> Sources
    {
        get
        {
            return sources;
        }

        set
        {
            sources = value;
        }
    }

    private void Start()
    {
        AudioSource[] sources = GetComponents<AudioSource>();
        soundSource = sources[0];
        musicSource = sources[1];

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

        Sources = new Dictionary<string, AudioSource>
        {
            { "Goal", soundSource },
            { "WallHit", soundSource },
            { "RacketHit", soundSource },
            { "TrainingInitial", soundSource },
            { "TrainingGoal", soundSource },
            { "TrainingService", soundSource },
            { "TrainingFree", soundSource },
            { "NiceShot", soundSource},
            { "Out", soundSource }
        };
    }

    public void PlaySound(string soundName)
    {
        Sources[soundName].PlayOneShot(Sounds[soundName]);
    }

    public void SetNewBallAudioSource()
    {
        Debug.Log("properly set");
        GameObject[] balls = GameObject.FindGameObjectsWithTag("Ball");
        GameObject ball;
        if (balls.Length > 1)
        {
            ball = balls[1];
        }
        else
        {
            ball = balls[0];
        }
        Sources["WallHit"] = ball.GetComponent<AudioSource>();
        Sources["RacketHit"] = ball.GetComponent<AudioSource>();
    }

}
