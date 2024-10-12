using Rubin;
using System;
using UnityEngine;

public class ParticleRoutine : MonoBehaviour
{
    public ParticleSystem ParticleSystem;
    public Ticker Ticker;
    [Header("Uses seconds")]
    public float Cooldown = 10f;
    public float PercentageChance = 0.3f;

    private void Awake()
    {
        Ticker = TickerCreator.CreateNormalTime(Cooldown);
    }

    private void Update()
    {
        if (Ticker.Push())
        {
            ParticleSystem.Stop();

            var percentage = UnityEngine.Random.Range(0f, 1f);
            if (percentage <= PercentageChance)
            {
                ParticleSystem.Play();
            }
        }
    }
}
