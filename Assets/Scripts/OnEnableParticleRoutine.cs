using Rubin;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnEnableParticleRoutine : MonoBehaviour
{
    public ParticleSystem ParticleSystem;
    public float LockForSeconds;
    private Ticker Ticker;

    private void OnEnable()
    {
        if (!Ticker.Done)
        {
            return;
        }

        Ticker = TickerCreator.CreateNormalTime(LockForSeconds);

        ParticleSystem.Play();
    }

    private void OnDisable()
    {
        ParticleSystem.Stop();
    }
}
