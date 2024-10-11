using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Resource", menuName = "CRPK/Inventory", order = 1)]
public class ResourceInventory : ScriptableObject
{
    public List<CountableResource> CountableResources;
    public List<BuildingData> Buildings;

    private void OnValidate()
    {
        foreach (var item in CountableResources)
        {
            item.Count = item._count;
        }
    }
}
