using Rubin;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingInstance : MonoBehaviour
{
    public BuildingData Data;
    public ResourceInventory Inventory;
    public float Cooldown;

    private Ticker ticker;

    private void Awake()
    {
        ticker = TickerCreator.CreateNormalTime(Cooldown);
    }

    private void Update()
    {
        if (ticker.Push())
        {
            foreach (var result in Data.Result)
            {
                var inventoryRef = Inventory.CountableResources.Find(x => x.ResourceType == result.ResourceType);
                inventoryRef.Count += result.Count;
            }
        }
    }
}
