using System;
using UnityEngine;

public class ParticleStepper : MonoBehaviour
{
    public ParticleSystem particleSystem;
    [SerializeField] private float fps; 
    private float frameTime = 1f;
    private float timer = 0f;

    private void Awake()
    {
        particleSystem = GetComponent<ParticleSystem>();
    }

    void Update()
    {
        frameTime = 1f / fps;
        timer += Time.deltaTime;

        while (timer >= frameTime)
        {
            particleSystem.Simulate(frameTime, true, false);
            timer -= frameTime;
        }
    }
}