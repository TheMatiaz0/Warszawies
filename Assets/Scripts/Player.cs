using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class Player : MonoBehaviour
{
    public ResourceInventory Inventory;

    public List<ResourceHUD> ResourceHuds;

    public int GridSize = 30;
    
    [System.Flags]
    public enum BlockingObjects
    {
        River = 0,
        Cave = 1,
        Forest = 2,
        Blocked = 4
    }
    public class FieldData
    {
        public BlockingObjects Objects;
    };

    public Dictionary<Vector2Int, FieldData> GridData = new Dictionary<Vector2Int, FieldData>();


    private void Awake()
    {
        CreateFields();
        UpdateFieldCollisions();
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

    private void UpdateFieldCollisions()
    {
        foreach(var Field in GridData)
        {
            Vector3 coordinates = new Vector3(Field.Key.x, 0, Field.Key.y);
            RaycastHit hit;
            if(Physics.Raycast(coordinates, Vector3.up, out hit, 1000))
            {
                Debug.DrawRay(coordinates, Vector3.up * 1000, Color.yellow);
                // debug start
                string hitObj = "";
                if (hit.collider.GetComponent<RiverObstacle>() != null)
                {
                    Field.Value.Objects = Field.Value.Objects | BlockingObjects.River;
                }
                else if (hit.collider.GetComponent<ForestObstacle>() != null)
                {
                    Field.Value.Objects = Field.Value.Objects | BlockingObjects.Forest;
                }
                Debug.Log("Did Hit " + hitObj + " at: "+ Field.Key);

                // debug end
            }

        }
    }
    private void Update()
    {

        UpdateFieldCollisions();
    }
}
