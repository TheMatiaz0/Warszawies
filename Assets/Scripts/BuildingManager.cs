using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    public List<BuildingInstance> BuildingInstances;
    public ResourceInventory Inventory;
    public Transform parent;

    private void Awake()
    {
        foreach (var item in Inventory.CreatedBuildings)
        {
            Build(item, Vector3.zero);
        }

        Inventory.OnBuildingAdded += Inventory_OnBuildingAdded;
        Inventory.OnBuildingRemoved += Inventory_OnBuildingRemoved;
    }

    private void Inventory_OnBuildingRemoved(BuildingData obj)
    {
        Remove(obj);
    }

    private void Inventory_OnBuildingAdded(BuildingData obj)
    {
        Debug.Log("spawn?");
        Build(obj, Vector3.zero);
    }

    private void OnDestroy()
    {
        if (Inventory != null)
        {
            Inventory.OnBuildingAdded -= Inventory_OnBuildingAdded;
            Inventory.OnBuildingRemoved -= Inventory_OnBuildingRemoved;
        }
    }

    public bool CanBuild(BuildingData data)
    {
        foreach (var requiredResource in data.Requirements)
        {
            foreach (var currentResource in Inventory.CountableResources)
            {
                return currentResource.Count >= requiredResource.Count;
            }
        }

        return false;
    }

    public void Build(BuildingData data, Vector3 position)
    {
        var buildingInstance = Instantiate(data.PrefabToSpawn, position, Quaternion.identity, parent);
        buildingInstance.Initialize(data);
        BuildingInstances.Add(buildingInstance);
    }

    public void Remove(BuildingData data)
    {
        var buildingInstance = BuildingInstances.Find(x => x.Data == data);
        BuildingInstances.Remove(buildingInstance);
        Destroy(buildingInstance.gameObject);
    }
}
