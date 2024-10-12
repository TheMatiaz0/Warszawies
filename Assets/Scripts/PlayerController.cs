using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    GameObject Building;

    [SerializeField]
    Camera PlayerCamera;

    [SerializeField]
    Player player;

    [SerializeField]
    List<BuildingData> Buildings;

    BuildingData SelectedBuilding;

    public BuildingManager BuildingManager;

    private int GridSize = 30;

   
    public int HouseFromCollisionDistance = 2;
    public int FisherhutFromRiverDistance = 2;
    public int LumberjackFromForestDistance = 2;
    public int StonecutterFromCaveDistance = 2;



    // Start is called before the first frame update
    void Start()
    {
        SelectedBuilding = Buildings[0];
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 HitPoint = PlayerCamera.ScreenToViewportPoint(Input.mousePosition);

        Ray ray = PlayerCamera.ViewportPointToRay(PlayerCamera.ScreenToViewportPoint(Input.mousePosition));
        RaycastHit hit;
        Vector3 newPosition = new Vector3();
        if(Physics.Raycast(ray, out hit, 1000)) 
        {
            newPosition = hit.point;
            if(newPosition.x < 0) newPosition.x = newPosition.x - newPosition.x % 10 - 5;
            else newPosition.x = newPosition.x - newPosition.x % 10 + 5;


            if (newPosition.z < 0) newPosition.z = newPosition.z - newPosition.z % 10 - 5;
            else newPosition.z = newPosition.z - newPosition.z % 10 + 5;
           
            
            Building.transform.position = newPosition;
        }

        PickBuilding();

        if(Input.GetMouseButtonDown(0))
        {
            if (BuildingManager.CanBuild(SelectedBuilding) && player.GridData.TryGetValue(new Vector2Int((int)newPosition.x, (int)newPosition.z), out var fieldData) && fieldData.ObjectPlaced == false)
            {
                int riverDistance = CheckForNearestRiver((int)newPosition.x, (int)newPosition.z);
                int forestDistance = CheckForNearestForest((int)newPosition.x, (int)newPosition.z);
                int caveDistance = CheckForNearestCave((int)newPosition.x, (int)newPosition.z);
                Debug.Log("Distance from river: " + riverDistance);
                Debug.Log("Distance from cave: " + caveDistance);
                Debug.Log("Distance from forest: " + forestDistance);
                if (SelectedBuilding.AllowedObjects.HasFlag(BlockingObjects.River)) // Mozna postawic przy rzece
                {
                    if(riverDistance > 0 && riverDistance < FisherhutFromRiverDistance)
                    {
                        BuildingManager.Build(SelectedBuilding, newPosition);

                        player.GridData[new Vector2Int((int)newPosition.x, (int)newPosition.z)].ObjectPlaced = true;
                        GameManager.Instance.Inventory.CreatedBuildings.Add(SelectedBuilding);
                    }
                }
                else if(SelectedBuilding.AllowedObjects.HasFlag(BlockingObjects.Forest)) // Mozna postawic przy lesie
                {
                    if(forestDistance > 0 && forestDistance < LumberjackFromForestDistance)
                    {
                        BuildingManager.Build(SelectedBuilding, newPosition);

                        player.GridData[new Vector2Int((int)newPosition.x, (int)newPosition.z)].ObjectPlaced = true;
                        GameManager.Instance.Inventory.CreatedBuildings.Add(SelectedBuilding);
                    }
                }
                else if(SelectedBuilding.AllowedObjects.HasFlag(BlockingObjects.Cave)) // Mozna postawic przy jaskiniach
                {
                    if (caveDistance > 0 && caveDistance < StonecutterFromCaveDistance)
                    {
                        BuildingManager.Build(SelectedBuilding, newPosition);

                        player.GridData[new Vector2Int((int)newPosition.x, (int)newPosition.z)].ObjectPlaced = true;
                        GameManager.Instance.Inventory.CreatedBuildings.Add(SelectedBuilding);
                    }
                }
                else if(SelectedBuilding.AllowedObjects.HasFlag(BlockingObjects.House)) // to dom
                {
                    int minDist = Mathf.Min(caveDistance, forestDistance, riverDistance);
                    if (minDist > HouseFromCollisionDistance)
                    {
                        BuildingManager.Build(SelectedBuilding, newPosition);

                        player.GridData[new Vector2Int((int)newPosition.x, (int)newPosition.z)].ObjectPlaced = true;
                        GameManager.Instance.Inventory.CreatedBuildings.Add(SelectedBuilding);
                    }
                }
            }
        }
    }

    int CheckForNearestRiver(int x, int z)
    {
        return player.RiverDistanceArray[x / 5, z / 5];
        //player.GridData[new Vector2Int(x, z)].Objects.HasFlag(BlockingObjects.River)
    }
    int CheckForNearestForest(int x, int z)
    {
        return player.ForestDistanceArray[x / 5, z / 5];
    }
    int CheckForNearestCave(int x, int z)
    {
        return player.CaveDistanceArray[x / 5, z / 5];
    }
    void PickBuilding()
    {
        if(Input.GetKey(KeyCode.Alpha1))
        {
            SelectedBuilding = Buildings[0];
        }
        else if (Input.GetKey(KeyCode.Alpha2))
        {
            SelectedBuilding = Buildings[1];
        }
        else if (Input.GetKey(KeyCode.Alpha3))
        {
            SelectedBuilding = Buildings[2];
        }
        else if(Input.GetKey(KeyCode.Alpha4))
        {
            SelectedBuilding = Buildings[3];
        }
    }
}
