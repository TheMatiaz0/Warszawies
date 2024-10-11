using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public ResourceInventory Inventory;

    public List<ResourceHUD> ResourceHuds;

    private void Awake()
    {
        foreach (var resource in Inventory.CountableResources)
        {
            RefreshHud(resource);
            resource.OnCountChanged += RefreshHud;
        }
    }

    private void RefreshHud(CountableResource countableResource)
    {
        var resourceHud = ResourceHuds.Find(x => x.ResourceType == countableResource.ResourceType);
        resourceHud.Refresh(countableResource.Count, countableResource.ResourceType.Icon);
    }

    private void OnDestroy()
    {
        foreach (var resource in Inventory.CountableResources)
        {
            resource.OnCountChanged -= RefreshHud;
        }
    }
}
