using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    [SerializeField] 
    Transform cameraRotationPivot;

    [SerializeField] 
    float maxDistanceToPivotPoint = 70;

    [SerializeField] 
    float minDistanceToPivotPoint = 30;

    float currentDistanceToPivotPoint = 40;
    float currentTargetDistanceToPivotPoint = 40;

    [SerializeField]
    private float xRotSpeed = 1;

    public BuildingData SelectedBuilding;

    public BuildingManager BuildingManager;

    private int GridSize = 30;

    [SerializeField]
    SelectedBuildingHud hud;

    [SerializeField]
    Sprite BuildingPieMenuSprite;

   
    public int HouseFromCollisionDistance = 2;
    public int FisherhutFromRiverDistance = 2;
    public int LumberjackFromForestDistance = 2;
    public int StonecutterFromCaveDistance = 2;

    private Vector3 previousPosition = new Vector3();

    // Start is called before the first frame update
    void Start()
    {
        SelectedBuilding = Buildings[0];
        //UpdateCameraPosition();
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
            OpenBuildingPieMenu();
            //Debug.Log($"key: { new Vector2Int((int)newPosition.x, (int)newPosition.z) }");
            if (BuildingManager.CanBuild(SelectedBuilding) && player.GridData.TryGetValue(new Vector2Int((int)newPosition.x, (int)newPosition.z), out var fieldData) && fieldData.ObjectPlaced == false)
            {
                int riverDistance = CheckForNearestRiver((int)newPosition.x, (int)newPosition.z);
                int forestDistance = CheckForNearestForest((int)newPosition.x, (int)newPosition.z);
                int caveDistance = CheckForNearestCave((int)newPosition.x, (int)newPosition.z);
                //Debug.Log("Distance from river: " + riverDistance);
                //Debug.Log("Distance from cave: " + caveDistance);
               // Debug.Log("Distance from forest: " + forestDistance);

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

        // Esc to Main Menu
        if (Input.GetKey(KeyCode.Escape))
        {
            SceneManager.LoadScene("MainMenu");
        }

        // Camera rotation
        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            currentTargetDistanceToPivotPoint = Mathf.Clamp(currentDistanceToPivotPoint + Input.GetAxis("Mouse ScrollWheel") * -100, minDistanceToPivotPoint, maxDistanceToPivotPoint);
            
            previousPosition = PlayerCamera.ScreenToViewportPoint(Input.mousePosition);
            UpdateCameraPosition();
        }

        if (Input.GetMouseButtonDown(2))
        {
            previousPosition = PlayerCamera.ScreenToViewportPoint(Input.mousePosition);
        }
        else if(Input.GetMouseButton(2))
        {
            UpdateCameraPosition();
            currentDistanceToPivotPoint = Mathf.Lerp(currentDistanceToPivotPoint, currentTargetDistanceToPivotPoint, 0.1f);
            previousPosition = PlayerCamera.ScreenToViewportPoint(Input.mousePosition);
        }
        else
        {
            currentDistanceToPivotPoint = Mathf.Lerp(currentDistanceToPivotPoint, currentTargetDistanceToPivotPoint, 0.1f);
            previousPosition = PlayerCamera.ScreenToViewportPoint(Input.mousePosition);
            UpdateCameraPosition();
        }
    }

    void OpenBuildingPieMenu()
    {
        Vector3 mousePosition = Input.mousePosition;
        //Instantiate(BuildingPieMenuSprite, mousePosition, Quaternion.identity);
        
        Debug.Log("Pie menu");

    }
    void UpdateCameraPosition()
    {

        Vector3 currentPosition = PlayerCamera.ScreenToViewportPoint(Input.mousePosition);
        Vector3 direction = previousPosition - currentPosition;

        float rotationAroundYAxis = -direction.x * 180 * xRotSpeed;

        PlayerCamera.transform.position = cameraRotationPivot.position;

        PlayerCamera.transform.Rotate(new Vector3(0, 1, 0), rotationAroundYAxis, Space.World);
        PlayerCamera.transform.Translate(new Vector3(0, 0, -currentDistanceToPivotPoint));

        previousPosition = currentPosition;
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
        hud.SelectedBuilding = SelectedBuilding;
    }
}
