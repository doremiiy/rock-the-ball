using UnityEngine;

public class ServiceZone : MonoBehaviour
{

    private bool isValid = false;
    private ServiceManager serviceManager;

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

    public ServiceManager ServiceManager
    {
        get
        {
            return serviceManager;
        }

        set
        {
            serviceManager = value;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            serviceManager.HandleService(IsValid);
        }
    }
}