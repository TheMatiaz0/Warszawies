using Rubin;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BuildingInstance : MonoBehaviour
{
    public BuildingData Data;
    public List<ParticleSystem> ParticleSystem;
    public AudioSource Audio;

    private Ticker ticker;
    private Ticker audioTicker;

    private void Awake()
    {
        Audio.PlayOneShot(Data.OnBuiltClip);
        Invoke(nameof(PlayFirstIdle), 0.2f);
        ticker = TickerCreator.CreateNormalTime(GameManager.Instance.Balance.CooldownForAllBuildings);
        audioTicker = TickerCreator.CreateNormalTime(GameManager.Instance.Balance.CooldownForAllBuildings + 10f + Random.Range(-1f, 1f));
    }

    private void PlayFirstIdle()
    {
        Audio.PlayOneShot(Data.OnIdleClip);
    }

    private void Update()
    {
        if (ticker.Push())
        {
            foreach (var result in Data.Result)
            {
                var inventoryRef = GameManager.Instance.Inventory.CountableResources.Find(x => x.ResourceType == result.ResourceType);
                inventoryRef.Count += result.Count;
            }
            foreach (var particle in ParticleSystem)
            {
                particle.Play();
            }
        }

        if (audioTicker.Push() && !Audio.isPlaying)
        {
            audioTicker.RequiredTime = GameManager.Instance.Balance.CooldownForAllBuildings + Random.Range(-1f, 1f);
            Audio.volume = Random.Range(0.75f, 1f);
            Audio.pitch = Random.Range(0.8f, 1.15f);
            Audio.PlayOneShot(Data.OnIdleClip);
        }
    }
}
