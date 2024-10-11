using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    public List<BuildingInstance> BuildingInstances;
    public ResourceInventory Inventory;
    public Transform parent;

    private void Awake()
    {
        foreach (var buildingData in Inventory.CreatedBuildings.ToList())
        {
            SpawnAtZero(buildingData);
        }

        Inventory.OnBuildingAdded += SpawnAtZero;
        Inventory.OnBuildingRemoved += Remove;
    }

    private void SpawnAtZero(BuildingData buildingData)
    {
        Build(buildingData, Vector3.zero);
    }

    private void OnDestroy()
    {
        if (Inventory != null)
        {
            Inventory.OnBuildingAdded -= SpawnAtZero;
            Inventory.OnBuildingRemoved -= Remove;
        }
    }

    public bool CanBuild(BuildingData data)
    {
        foreach (var requiredResource in data.Requirements)
        {
            foreach (var currentResource in Inventory.CountableResources)
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
            foreach (var currentResource in Inventory.CountableResources)
            {
                currentResource.Count -= requiredResource.Count;
            }
        }
    }

    public void Build(BuildingData data, Vector3 position)
    {
        var buildingInstance = Instantiate(data.PrefabToSpawn, position, Quaternion.identity, parent);
        buildingInstance.Initialize(data);
        BuildingInstances.Add(buildingInstance);
        // SpendResources(data);
    }

    public void Remove(BuildingData data)
    {
        var buildingInstance = BuildingInstances.Find(x => x.Data == data);
        BuildingInstances.Remove(buildingInstance);

        Destroy(buildingInstance.gameObject);
    }
}
