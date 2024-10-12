using Rubin;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutomaticUpgrade : MonoBehaviour
{
    public BuildingManager BuildingManager;

    private CountableResource population;
    private BuildingData currentUpgrade;

    private void Start()
    {
        population = GameManager.Instance.Inventory.CountableResources.ToType(ResourceType.Population);
        population.OnCountChanged += OnPopulationChanged;
    }

    private void OnDestroy()
    {
        population.OnCountChanged -= OnPopulationChanged;
    }

    private void OnPopulationChanged(CountableResource countableResource)
    {
        var count = countableResource.Count;
        var upgradeBuilding = GetUpgradeBuilding();

        if (upgradeBuilding != null)
        {
            BuildingManager.SpawnAtZero(upgradeBuilding);
            currentUpgrade = upgradeBuilding;
        }
    }

    private BuildingData GetUpgradeBuilding()
    {
        foreach (var thresholdBuilding in GameManager.Instance.Balance.PopulationThresholds)
        {
            if (population.Count >= thresholdBuilding.Threshold && BuildingManager.GetAllBuildingsOfData(thresholdBuilding.Building) == 0)
            {
                return thresholdBuilding.Building;
            }
        }

        return null;
    }
}
