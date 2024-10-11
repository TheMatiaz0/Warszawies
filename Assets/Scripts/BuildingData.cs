using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CountableResource
{
    public event Action<CountableResource> OnCountChanged = delegate {};

    public ResourceData ResourceType;

    public int Count 
    {
        get => _count;
        set
        {
            _count = value;
            OnCountChanged?.Invoke(this);
        }
    }

    public int _count;
}

[CreateAssetMenu(fileName = "Building", menuName = "CRPK/Building", order = 1)]
public class BuildingData : ScriptableObject
{
    public List<CountableResource> Requirements;
    public GameObject PrefabToSpawn;
}
