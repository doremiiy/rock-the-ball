using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServiceZone : MonoBehaviour
{

    private bool isValid = false;

    public void SetIsValid(bool newValidityValue)
    {
        isValid = newValidityValue;
    }
    public bool GetIsValid()
    {
        return isValid;
    }
}