using UnityEngine;

public class AutomaticUpgrade : MonoBehaviour
{
    public BuildingManager BuildingManager;

    private CountableResource population;
    private int currentUpgradeIndex;

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
        }
    }

    private BuildingData GetUpgradeBuilding()
    {
        for (int i = 0; i < GameManager.Instance.Balance.PopulationThresholds.Count; i++)
        {
            var current = GameManager.Instance.Balance.PopulationThresholds[i];
            if (i < currentUpgradeIndex)
            {
                if (BuildingManager.BuildingInstances.Find(x => x.Data == current.Building))
                {
                    BuildingManager.Remove(current.Building);
                }

                continue;
            }
            if (population.Count >= current.Threshold && BuildingManager.GetAllBuildingsOfData(current.Building) == 0)
            {
                currentUpgradeIndex = i;

                return current.Building;
            }
        }

        return null;
    }
}
