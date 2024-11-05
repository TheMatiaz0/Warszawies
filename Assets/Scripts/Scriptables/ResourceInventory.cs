using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Resource", menuName = "CRPK/Inventory", order = 1)]
public class ResourceInventory : ResettableScriptableObject
{
    public event Action<BuildingData> OnBuildingAdded;
    public event Action<BuildingData> OnBuildingRemoved;

    public List<CountableResource> CountableResources;
    [Header("Please do not try to add/remove buildings at index in runtime")]
    public List<BuildingData> CreatedBuildings;

    private List<BuildingData> cachedBuildings;

    private void OnValidate()
    {
        foreach (var item in CountableResources)
        {
            item.Count = item._count;
        }

        if (cachedBuildings == null || cachedBuildings.Count == 0)
        {
            return;
        }

        var delta = CreatedBuildings.Count - cachedBuildings.Count;

        if (delta > 0)
        {
            OnBuildingAdded?.Invoke(CreatedBuildings[^1]);
        }
        else
        {
            OnBuildingRemoved?.Invoke(cachedBuildings[^1]);
        }


        Debug.Log($"Now: {CreatedBuildings.Count}, Cached: {cachedBuildings.Count}");

        cachedBuildings = new(CreatedBuildings);
    }
}
