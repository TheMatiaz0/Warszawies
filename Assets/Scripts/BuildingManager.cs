using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    public List<BuildingInstance> BuildingInstances;
    public Transform parent;

    private List<CountableResource> CountableResources;

    private void Start()
    {
        foreach (var buildingData in GameManager.Instance.Inventory.CreatedBuildings.ToList())
        {
            SpawnAtZero(buildingData);
        }

        GameManager.Instance.Inventory.OnBuildingAdded += SpawnAtZero;
        GameManager.Instance.Inventory.OnBuildingRemoved += Remove;
    }

    public void SpawnAtZero(BuildingData buildingData)
    {
        Build(buildingData, buildingData.StartPosition);
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.Inventory.OnBuildingAdded -= SpawnAtZero;
            GameManager.Instance.Inventory.OnBuildingRemoved -= Remove;
        }
    }

    public bool CanBuild(BuildingData data)
    {
        foreach (var requiredResource in data.Requirements)
        {
            foreach (var currentResource in GameManager.Instance.Inventory.CountableResources)
            {
                if (requiredResource.ResourceType == currentResource.ResourceType && currentResource.Count < requiredResource.Count)
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
            foreach (var currentResource in GameManager.Instance.Inventory.CountableResources)
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

    public List<BuildingInstance> GetAllBuildingsOfFlag(BlockingObjects blockingObjects)
    {
        return BuildingInstances.FindAll(x => x.Data.AllowedObjects.HasFlag(blockingObjects));
    }

    public int GetIdleCount(ResourceData resourceData)
    {
        return BuildingInstances.Where(x => x.Data == resourceData)
            .Sum(x => x.Data.Result.Count);
    }

    public void Build(BuildingData data, Vector3 position)
    {
        if (data.PrefabToSpawn == null) return;
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
