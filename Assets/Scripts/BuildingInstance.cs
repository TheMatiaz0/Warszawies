using Rubin;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingInstance : MonoBehaviour
{
    public BuildingData Data;

    private Ticker ticker;

    private void Awake()
    {
        ticker = TickerCreator.CreateNormalTime(Balance.Instance.CooldownForAllBuildings);
    }

    private void Update()
    {
        if (ticker.Push())
        {
            foreach (var result in Data.Result)
            {
                var inventoryRef = Balance.Instance.Inventory.CountableResources.Find(x => x.ResourceType == result.ResourceType);
                inventoryRef.Count += result.Count;
            }
        }
    }
}
