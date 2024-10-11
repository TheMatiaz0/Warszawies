using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public ResourceInventory Inventory;

    private void Awake()
    {
        foreach (var resource in Inventory.CountableResources)
        {
            resource.OnCountChanged += UpdateCount;
        }
    }

    private void UpdateCount(ResourceData data, int obj)
    {

    }

    private void OnDestroy()
    {
        foreach (var resource in Inventory.CountableResources)
        {
            resource.OnCountChanged += UpdateCount;
        }
    }
}
