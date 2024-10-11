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
        var resource = ResourceHuds.Find(x => x.ResourceType == countableResource.ResourceType);
        resource.CountDisplay.text = countableResource.Count.ToString();
    }

    private void OnDestroy()
    {
        foreach (var resource in Inventory.CountableResources)
        {
            resource.OnCountChanged -= RefreshHud;
        }
    }
}
