using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Light2D))]
public class LightFlicker : MonoBehaviour
{

    [SerializeField, Range(0f, 10f)] float MinIntensity = 0.8f;
    [SerializeField, Range(0f, 10f)] float MaxIntensity = 1.2f;
    [SerializeField] float LerpSpeed = 10f;
    Light2D light2D;

    public float ChangeSpeed = 0.3f;

    float LerpValue;
    float RandomValue;
    float OuterRadius;
    void Start()
    {
        light2D = GetComponent<Light2D>();
        LerpValue = Random.Range(MinIntensity, MaxIntensity);
        InvokeRepeating(nameof(AssingRandom), 0.1f, ChangeSpeed);
        light2D.intensity = 0f;
        OuterRadius = light2D.pointLightOuterRadius;
        light2D.pointLightOuterRadius = 0f;
    }

    void Update()
    {
        LerpLight();
    }

    void LerpLight()
    {
        light2D.intensity = Mathf.Lerp(light2D.intensity, RandomValue, LerpSpeed * Time.deltaTime);
        light2D.pointLightOuterRadius = Mathf.Lerp(light2D.pointLightOuterRadius, OuterRadius, LerpSpeed/2 * Time.deltaTime);
    }

    void AssingRandom()
    {
        RandomValue = Random.Range(MinIntensity, MaxIntensity);
    }
}
