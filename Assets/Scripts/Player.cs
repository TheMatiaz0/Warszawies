using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

   [System.Flags]
    public enum BlockingObjects
    {
        None = 0,
        River = 1,
        Cave = 2,
        Forest = 4,
        Blocked = 8,
        House = 16,
        Palace = 32,
    }


public class Player : MonoBehaviour
{
    public List<ResourceHUD> ResourceHuds;
    public SelectedBuildingHud SelectedBuildingHud;
    public CardModal EventCard;
    public BuildingManager BuildingManager;

    /// <summary>
    /// 2 cases:
    /// - game over from GameManager
    /// - event based on time
    /// </summary>

    public int GridSize = 30;


    public int[,] RiverDistanceArray = new int[31, 31];
    public int[,] ForestDistanceArray = new int[31, 31];
    public int[,] CaveDistanceArray = new int[31, 31];

    public class FieldData
    {
        public BlockingObjects Objects;
        public bool ObjectPlaced = false;
    };

    public Dictionary<Vector2Int, FieldData> GridData = new Dictionary<Vector2Int, FieldData>();


    private void Start()
    {
        CreateFields();
        UpdateFieldCollisions();
        RemoveLandscapeColliders();
        CalculateDistanceArrays();

        if (GameManager.Instance != null || GameManager.Instance.Inventory != null)
        {
            foreach (var resource in GameManager.Instance.Inventory.CountableResources)
            {
                RefreshHud(resource);
                resource.OnCountChanged += RefreshHud;
            }
        }
    }

    private void RefreshHud(CountableResource countableResource)
    {
        var resourceHud = ResourceHuds.Find(x => x.ResourceType == countableResource.ResourceType);
        resourceHud.Refresh(countableResource.Count, BuildingManager.GetIdleCount(countableResource.ResourceType));
    }

    private void OnDestroy()
    {
        if (GameManager.Instance.Inventory != null)
        {
            foreach (var resource in GameManager.Instance.Inventory.CountableResources)
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
                var key = new Vector2Int(i * 5, j * 5);
                GridData.Add(key, new FieldData());
            }
        }
    }

    private void RemoveLandscapeColliders() // huh?!
    {
        /*
        var collidersRiver = FindObjectsOfType<RiverObstacle>();
        foreach (var collider in collidersRiver)
        {
            Destroy(collider.gameObject);
        }
        var collidersForest = FindObjectsOfType<ForestObstacle>();
        foreach (var collider in collidersForest)
        {
            Destroy(collider.gameObject);
        }
        var collidersCave = FindObjectsOfType<CaveObstacle>();
        foreach (var collider in collidersCave)
        {
            Destroy(collider.gameObject);
        }
        */
    }
    private void UpdateFieldCollisions()
    {
        foreach(var Field in GridData)
        {
            Vector3 coordinates = new Vector3(Field.Key.x, 0, Field.Key.y);
            RaycastHit hit;
            if(Physics.Raycast(coordinates, Vector3.up, out hit, 1000))
            {
                // debug start
                string hitObj = "";
                if (hit.collider.GetComponent<RiverObstacle>() != null)
                {
                    Field.Value.Objects = Field.Value.Objects | BlockingObjects.River;
                }
                else if (hit.collider.GetComponent<ForestObstacle>() != null)
                {
                    Debug.Log("Hit forest");
                    Field.Value.Objects = Field.Value.Objects | BlockingObjects.Forest;
                }
                else if (hit.collider.GetComponent<CaveObstacle>() != null)
                {
                    Debug.Log("Hit cave");
                    Field.Value.Objects = Field.Value.Objects | BlockingObjects.Cave;
                }

                // debug end
            }

        }
    }
 

    void CalculateDistanceArrays()
    {
        for (int i = 0; i <= GridSize; i++)
        {
            for (int j = 0; j <= GridSize; j++)
            {
                RiverDistanceArray[i, j] = 999;
                CaveDistanceArray[i, j] = 999;
                ForestDistanceArray[i, j] = 999;
            }
        }
        for (int i = 0; i < GridSize; i++)
        {
            for (int j = 0; j < GridSize; j++)
            {
                if (GridData.ContainsKey(new Vector2Int(i * 5, j * 5)))
                {
                    if (GridData[new Vector2Int(i * 5, j * 5)].Objects.HasFlag(BlockingObjects.River))
                    {
                        RiverDistanceArray[i, j] = 0;
                    }
                    if (GridData[new Vector2Int(i * 5, j * 5)].Objects.HasFlag(BlockingObjects.Forest))
                    {
                        ForestDistanceArray[i, j] = 0;
                    }
                    if (GridData[new Vector2Int(i * 5, j * 5)].Objects.HasFlag(BlockingObjects.Cave))
                    {
                        CaveDistanceArray[i, j] = 0;
                    }
                }

            }
        }
        for (int a = 0; a < GridSize; a++)
        {
            for (int i = 1; i < GridSize; i++)
            {
                for (int j = 1; j < GridSize; j++)
                {
                    int minRiver = RiverDistanceArray[i, j];
                    if (RiverDistanceArray[i + 1, j] + 1 < minRiver) minRiver = RiverDistanceArray[i + 1, j] +1;
                    if (RiverDistanceArray[i - 1, j] + 1< minRiver) minRiver = RiverDistanceArray[i - 1, j] + 1;
                    if (RiverDistanceArray[i, j - 1] + 1< minRiver) minRiver = RiverDistanceArray[i, j - 1] + 1;
                    if (RiverDistanceArray[i, j + 1] + 1< minRiver) minRiver = RiverDistanceArray[i, j + 1] + 1;
                    RiverDistanceArray[i, j] = minRiver;

                    //TODO:
                    int minForest = ForestDistanceArray[i, j];
                    if (ForestDistanceArray[i + 1, j] + 1 < minForest) minForest = ForestDistanceArray[i + 1, j] + 1;
                    if (ForestDistanceArray[i - 1, j] + 1 < minForest) minForest = ForestDistanceArray[i - 1, j] + 1;
                    if (ForestDistanceArray[i, j - 1] + 1 < minForest) minForest = ForestDistanceArray[i, j - 1] + 1;
                    if (ForestDistanceArray[i, j + 1] + 1 < minForest) minForest = ForestDistanceArray[i, j + 1]    ;
                    ForestDistanceArray[i, j] = minForest;

                    int minCave = CaveDistanceArray[i, j];
                    if (CaveDistanceArray[i + 1, j] + 1 < minCave) minCave = CaveDistanceArray[i + 1, j] + 1;
                    if (CaveDistanceArray[i - 1, j] + 1 < minCave) minCave = CaveDistanceArray[i - 1, j] + 1;
                    if (CaveDistanceArray[i, j - 1] + 1 < minCave) minCave = CaveDistanceArray[i, j - 1] + 1;
                    if (CaveDistanceArray[i, j + 1] + 1 < minCave) minCave = CaveDistanceArray[i, j + 1] + 1;
                    CaveDistanceArray[i, j] = minCave;
                }
            }
        }
        /*
        for (int i = 1; i < GridSize; i++)
        {
                //Debug.Log("i = " + i);
            string debugStr = "i = " + i;
            for (int j = 1; j < GridSize; j++)
            {
                debugStr += "[j=" + j + "]:";
                debugStr += RiverDistanceArray[i, j];
                debugStr += " ";
            }
            Debug.Log(debugStr);
        }*/
    }
}
