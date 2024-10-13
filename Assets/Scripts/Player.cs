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
    public PlayerController pc;

    public int GridSize = 20;


    public static int[,] RiverDistanceArray = new int[31, 31];
    public static int[,] ForestDistanceArray = new int[31, 31];
    public static int[,] CaveDistanceArray = new int[31, 31];

    public class FieldData
    {
        public GameObject ObjectReference;
        public BlockingObjects Objects;
        public bool ObjectPlaced = false;
    };

    public static Dictionary<Vector2Int, FieldData> GridData;


    private void Start()
    {
        GridData = new Dictionary<Vector2Int, FieldData>();
        RiverDistanceArray = new int[31, 31];
        ForestDistanceArray = new int[31, 31];
        CaveDistanceArray = new int[31, 31];
        CreateFields();
        CalculateDistanceArrays();
        UpdateFieldCollisions();
        RemoveLandscapeColliders();

        if (GameManager.Instance != null || GameManager.Instance.Inventory != null)
        {
            foreach (var resource in GameManager.Instance.Inventory.CountableResources)
            {
                RefreshHud(resource);
                resource.OnCountChanged += RefreshHud;
                GameManager.Instance.Inventory.OnBuildingAdded += OnNewBuilding;
                GameManager.Instance.Inventory.OnBuildingRemoved += OnNewBuilding;
            }
        }
    }

    private void OnNewBuilding(BuildingData obj)
    {
        foreach (var resource in GameManager.Instance.Inventory.CountableResources)
        {
            RefreshHud(resource);
        }
    }

    private void RefreshHud(CountableResource countableResource)
    {
        var resourceHud = ResourceHuds.Find(x => x.ResourceType == countableResource.ResourceType);

        //Debug.Log(BuildingManager.GetIdleCount(countableResource.ResourceType));

        resourceHud.Refresh(countableResource.Count, BuildingManager.GetIdleCount(countableResource.ResourceType));
    }

    private void OnDestroy()
    {
        if (GameManager.Instance.Inventory != null)
        {
            foreach (var resource in GameManager.Instance.Inventory.CountableResources)
            {
                resource.OnCountChanged -= RefreshHud;
                GameManager.Instance.Inventory.OnBuildingAdded -= OnNewBuilding;
                GameManager.Instance.Inventory.OnBuildingRemoved -= OnNewBuilding;
            }
        }
    }

    private void CreateFields()
    {
        for(int i = 0; i < GridSize; i++)
        {
            for (int j = 0; j < GridSize; j++)
            {
                var key = new Vector2Int(j * 10 + 5, i * 10 + 5);
                GridData.Add(key, new FieldData());
            }
        }
        Debug.Log("Field count: " + GridData.Count);

    }

    private void RemoveLandscapeColliders()
    {
        
        var collidersRiver = FindObjectsOfType<RiverObstacle>();
        foreach (var collider in collidersRiver)
        {
            Destroy(collider.GetComponent<BoxCollider>());
        }
        var collidersForest = FindObjectsOfType<ForestObstacle>();
        foreach (var collider in collidersForest)
        {
            Destroy(collider.GetComponent<BoxCollider>());
        }
        var collidersCave = FindObjectsOfType<CaveObstacle>();
        foreach (var collider in collidersCave)
        {
            Destroy(collider.GetComponent<BoxCollider>());
        }
        var collidersDeco = FindObjectsOfType<DecorativeTile>();
        foreach (var collider in collidersDeco)
        {
            Destroy(collider.GetComponent<BoxCollider>());
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
                // debug start
                if (hit.collider.GetComponent<RiverObstacle>() != null)
                {
                    Debug.Log("River on: " + Field.Key.x + " "+ Field.Key.y);
                    int i = (Field.Key.x - 5) / 10;
                    int j = (Field.Key.y - 5) / 10;
                    RiverDistanceArray[i, j] = 0;
                    Debug.Log("River after calc: " + i + " " + j);
                    Field.Value.Objects = Field.Value.Objects | BlockingObjects.River;
                }
                else if (hit.collider.GetComponent<ForestObstacle>() != null)
                {

                    int i = (Field.Key.x - 5) / 10;
                    int j = (Field.Key.y - 5) / 10;
                    ForestDistanceArray[i, j] = 0;
                    //Debug.Log("Forest on: " + Field.Key.x + " " + Field.Key.y);
                    Field.Value.Objects = Field.Value.Objects | BlockingObjects.Forest;
                }
                else if (hit.collider.GetComponent<CaveObstacle>() != null)
                {
                    int i = (Field.Key.x - 5) / 10;
                    int j = (Field.Key.y - 5) / 10;
                    CaveDistanceArray[i, j] = 0;
                    Field.Value.Objects = Field.Value.Objects | BlockingObjects.Cave;
                    //Debug.Log("Cave on: " + Field.Key.x + " "+ Field.Key.y);
                }
                else if (hit.collider.GetComponent<DecorativeTile>() != null)
                {
                    Field.Value.ObjectReference = hit.collider.gameObject;
                }

                Field.Value.ObjectReference = hit.collider.gameObject;

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
        /*for (int i = 0; i <= GridSize; i++)
        {
            for (int j = 0; j <= GridSize; j++)
            {
                if (GridData.ContainsKey(new Vector2Int(j * 10 + 5, i * 10 + 5)))
                {
                    if (GridData[new Vector2Int(j * 10 + 5, i * 10 + 5)].Objects.HasFlag(BlockingObjects.River))
                    {
                        RiverDistanceArray[j, i] = 0;
                        //Debug.Log("River hit on "+ (j * 10 + 5) + ", " + (i * 10 + 5));
                    }
                    if (GridData[new Vector2Int(j * 10 + 5, i * 10 + 5)].Objects.HasFlag(BlockingObjects.Forest))
                    {
                        ForestDistanceArray[j, i] = 0;
                        //Debug.Log("Forest hit");
                    }
                    if (GridData[new Vector2Int(j * 10 + 5, i * 10 + 5)].Objects.HasFlag(BlockingObjects.Cave))
                    {
                        CaveDistanceArray[j, i] = 0;
                        //Debug.Log("Cave hit");
                    }
                }

            }
        }*/
        /*for (int a = 0; a < GridSize; a++)
        {
            for (int i = 1; i < GridSize; i++)
            {
                for (int j = 1; j < GridSize; j++)
                {
                    int minRiver = RiverDistanceArray[i, j];
                    if (RiverDistanceArray[i + 1, j] + 1 < minRiver) minRiver = RiverDistanceArray[i + 1, j] +1;
                    if (RiverDistanceArray[i - 1, j] + 1 < minRiver) minRiver = RiverDistanceArray[i - 1, j] + 1;
                    if (RiverDistanceArray[i, j - 1] + 1 < minRiver) minRiver = RiverDistanceArray[i, j - 1] + 1;
                    if (RiverDistanceArray[i, j + 1] + 1 < minRiver) minRiver = RiverDistanceArray[i, j + 1] + 1;
                    RiverDistanceArray[i, j] = minRiver;

                    int minForest = ForestDistanceArray[i, j];
                    if (ForestDistanceArray[i + 1, j] + 1 < minForest) minForest = ForestDistanceArray[i + 1, j] + 1;
                    if (ForestDistanceArray[i - 1, j] + 1 < minForest) minForest = ForestDistanceArray[i - 1, j] + 1;
                    if (ForestDistanceArray[i, j - 1] + 1 < minForest) minForest = ForestDistanceArray[i, j - 1] + 1;
                    if (ForestDistanceArray[i, j + 1] + 1 < minForest) minForest = ForestDistanceArray[i, j + 1] + 1;
                    ForestDistanceArray[i, j] = minForest;

                    int minCave = CaveDistanceArray[i, j];
                    if (CaveDistanceArray[i + 1, j] + 1 < minCave) minCave = CaveDistanceArray[i + 1, j] + 1;
                    if (CaveDistanceArray[i - 1, j] + 1 < minCave) minCave = CaveDistanceArray[i - 1, j] + 1;
                    if (CaveDistanceArray[i, j - 1] + 1 < minCave) minCave = CaveDistanceArray[i, j - 1] + 1;
                    if (CaveDistanceArray[i, j + 1] + 1 < minCave) minCave = CaveDistanceArray[i, j + 1] + 1;
                    CaveDistanceArray[i, j] = minCave;
                }
            }
        }*/
        
        for (int i = 1; i < GridSize; i++)
        {
            string debugStr = "i = " + i + " ";
            for (int j = 1; j < GridSize; j++)
            {
                //debugStr += "[j=" + j + "]:";
                debugStr += ForestDistanceArray[j, i];
                debugStr += " ";
            }
            //Debug.Log(debugStr);
        }
    }
}
