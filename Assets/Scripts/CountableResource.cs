using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
