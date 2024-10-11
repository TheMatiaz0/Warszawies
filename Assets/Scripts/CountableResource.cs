using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ResourceType
{
    Wood,
    Stone,
    Population,
    Food
}

public static class CountableResourceExtension
{
    public static ResourceType[] resourceTypes = new ResourceType[]
    {
        ResourceType.Wood,
        ResourceType.Stone,
        ResourceType.Population,
        ResourceType.Food
    };

    public static CountableResource ToType(this List<CountableResource> countables, ResourceType resourceType)
    {
        return countables[(int)resourceType];
    }
}


[Serializable]
public class CountableResource
{
    public event Action<CountableResource> OnCountChanged = delegate { };

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

    [Header("Dynamic")]
    public int _count;

    public bool ShouldChangePermanently = true;
}
