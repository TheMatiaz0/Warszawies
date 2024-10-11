using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public ResourceInventory Inventory;

    public List<ResourceHUD> ResourceHuds;

    public int GridSize = 30;
    public struct FieldData
    {
        [System.Flags]
        public enum AvailableBuildings
        {
            River = 0,
            Cave = 1,
            Forest = 2,
            Blocked = 4
        }
    };

    public Dictionary<Vector2Int, FieldData> GridData = new Dictionary<Vector2Int, FieldData>();


    private void Awake()
    {
        CreateFields();
        foreach (var resource in Inventory?.CountableResources)
        {
            RefreshHud(resource);
            resource.OnCountChanged += RefreshHud;
        }
    }

    private void RefreshHud(CountableResource countableResource)
    {
        var resourceHud = ResourceHuds.Find(x => x.ResourceType == countableResource.ResourceType);
        resourceHud.Refresh(countableResource.Count);
    }

    private void OnDestroy()
    {
        if (Inventory != null)
        {
            foreach (var resource in Inventory.CountableResources)
            {
                resource.OnCountChanged -= RefreshHud;
            }
        }
    }

    private void CreateFields()
    {
        for(int i = 0; i < GridSize; i++)
        {
            for (int j = 0; j < GridSize; j++)
            {
                GridData.Add(new Vector2Int(i*5, j*5), new FieldData());
            }
        }
    }
}
