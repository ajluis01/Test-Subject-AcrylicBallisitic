using PrimeTween;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class LightBlink : MonoBehaviour
{
    Light lightSource;

    void Start()
    {
        lightSource = GetComponent<Light>();
        float randomIntensityMultiplier = Random.Range(0.5f, 0.75f);
        float randomDuration = Random.Range(0.3f, 1.0f);
        Tween.LightIntensity(lightSource, lightSource.intensity * randomIntensityMultiplier, randomDuration, Ease.InOutBounce, -1, CycleMode.Yoyo);
    }
}