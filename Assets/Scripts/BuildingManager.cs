using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    public List<BuildingInstance> BuildingInstances;
    public Transform parent;

    private void Awake()
    {
        foreach (var buildingData in Balance.Instance.Inventory.CreatedBuildings.ToList())
        {
            SpawnAtZero(buildingData);
        }

        Balance.Instance.Inventory.OnBuildingAdded += SpawnAtZero;
        Balance.Instance.Inventory.OnBuildingRemoved += Remove;
    }

    private void SpawnAtZero(BuildingData buildingData)
    {
        Build(buildingData, Vector3.zero);
    }

    private void OnDestroy()
    {
        if (Balance.Instance != null)
        {
            Balance.Instance.Inventory.OnBuildingAdded -= SpawnAtZero;
            Balance.Instance.Inventory.OnBuildingRemoved -= Remove;
        }
    }

    public bool CanBuild(BuildingData data)
    {
        foreach (var requiredResource in data.Requirements)
        {
            foreach (var currentResource in Balance.Instance.Inventory.CountableResources)
            {
                if (currentResource.Count < requiredResource.Count)
                {
                    return false;
                }
            }
        }

        return true;
    }

    public void SpendResources(BuildingData data)
    {
        foreach (var requiredResource in data.Requirements)
        {
            foreach (var currentResource in Balance.Instance.Inventory.CountableResources)
            {
                if (currentResource.ResourceType == requiredResource.ResourceType)
                {
                    currentResource.Count -= requiredResource.Count;
                }
            }
        }
    }

    public int GetAllBuildingsOfData(BuildingData data)
    {
        return BuildingInstances.Count(x => x.Data == data);
    }

    public void Build(BuildingData data, Vector3 position)
    {
        var buildingInstance = Instantiate(data.PrefabToSpawn, position, Quaternion.identity, parent);
        BuildingInstances.Add(buildingInstance);
        SpendResources(data);
    }

    public void Remove(BuildingData data)
    {
        var buildingInstance = BuildingInstances.Find(x => x.Data == data);
        BuildingInstances.Remove(buildingInstance);

        Destroy(buildingInstance.gameObject);
    }
}
