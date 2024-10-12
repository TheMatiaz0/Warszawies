using Rubin;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutomaticUpgrade : MonoBehaviour
{
    public BuildingManager BuildingManager;

    private CountableResource population;
    private BuildingData currentUpgrade;

    private void Awake()
    {
        population = GameManager.Instance.Inventory.CountableResources.ToType(ResourceType.Population)
        population.OnCountChanged += AutomaticUpgrade_OnCountChanged;
    }

    private void OnDestroy()
    {
        population.OnCountChanged -= AutomaticUpgrade_OnCountChanged;
    }

    private void AutomaticUpgrade_OnCountChanged(CountableResource obj)
    {
        var count = obj.Count;
        var upgradeBuilding = GetUpgradeBuilding();

        if (upgradeBuilding != null)
        {
            BuildingManager.SpawnAtZero(upgradeBuilding);
        }
    }

    private BuildingData GetUpgradeBuilding()
    {
        foreach (var thresholdBuilding in GameManager.Instance.Balance.PopulationThresholds)
        {
            if (population.Count >= thresholdBuilding.Threshold && currentUpgrade != thresholdBuilding.Building)
            {
                return thresholdBuilding.Building;
            }
        }

        return null;
    }
}
