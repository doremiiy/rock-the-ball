using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSound : MonoBehaviour {

    private AudioSource audioSource;
    public AudioClip welcomeClip;

	void Start () {
        audioSource = GetComponent<AudioSource>();
        StartCoroutine(WaitAndPlay(2f));
	}

    IEnumerator WaitAndPlay(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        audioSource.PlayOneShot(welcomeClip);
    }
}
