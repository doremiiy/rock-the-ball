using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Detector : MonoBehaviour {

    public GameObject limit;
    private Renderer limitTexture;


    private void Start()
    {
        limitTexture = limit.GetComponent<Renderer>();
        limitTexture.material.SetFloat("_Cutoff", 1f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player is too close to the wall");
            limitTexture.material.SetFloat("_Cutoff", 06716418f);
        }
    }
}
