using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Detector : MonoBehaviour {

    public GameObject limit;
    private Renderer limitTexture;
    public float cutoffSup;
    public float cutoffInf;
    public float cutoffDefault;
    private bool isAppearing = false;
    private bool isDisappearing = false;
    private float lerpValue = 0.0f;
    public float speedApparitionMultiplier;


    private void Start()
    {
        limitTexture = limit.GetComponent<Renderer>();
        limitTexture.material.SetFloat("_Cutoff", cutoffDefault);
    }

    private void Update()
    {
        if (isAppearing)
        {
            limitTexture.material.SetFloat("_Cutoff", Mathf.Lerp(cutoffSup, cutoffInf, lerpValue));
            lerpValue += Time.deltaTime * speedApparitionMultiplier;
        } else if (isDisappearing)
        {
            limitTexture.material.SetFloat("_Cutoff", Mathf.Lerp(cutoffInf, cutoffSup, lerpValue));
            lerpValue += Time.deltaTime * speedApparitionMultiplier;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Head"))
        {
            lerpValue = 0f;
            isAppearing = true;
            isDisappearing = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Head"))
        {
            lerpValue = 0f;
            isDisappearing = true;
            isAppearing = false;
        }
    }
}
