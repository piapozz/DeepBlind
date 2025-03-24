using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class PerlinNoiseLight : MonoBehaviour
{
    [SerializeField]
    float maxIntensity;

    [SerializeField]
    float blinkSpeed;

    UnityEngine.Light blinkLight;

    void Start()
    {
        blinkLight = this.gameObject.GetComponent<UnityEngine.Light>();
    }

    void LateUpdate()
    {
        blinkLight.intensity = Mathf.PerlinNoise(Time.time * blinkSpeed, 0) * maxIntensity;
    }
}
