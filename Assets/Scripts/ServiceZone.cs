using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServiceZone : MonoBehaviour
{

    private bool isValid = false;

    public bool IsValid
    {
        get
        {
            return isValid;
        }

        set
        {
            isValid = value;
        }
    }
}