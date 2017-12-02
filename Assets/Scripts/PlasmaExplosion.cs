using UnityEngine;

public class PlasmaExplosion : MonoBehaviour {

    private ParticleSystem plasmaExplosion;

    private void Start()
    {
        plasmaExplosion = GetComponent<ParticleSystem>();
    }

    void Update () {
        if (!plasmaExplosion.isPlaying)
        {
            Debug.Log("Object should be destroyed");
            Destroy(this.gameObject);
        }
    }
}
