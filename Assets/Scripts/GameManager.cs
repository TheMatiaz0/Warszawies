using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Balance Balance;
    public ResourceInventory Inventory;

    public static GameManager Instance { get; private set; }

    private CountableResource populationResource;

    private void Awake()
    {
        Instance = this;
        Inventory = Instantiate(Inventory);
    }

    public void Win()
    {

    }

    public void Lose()
    {
        populationResource = GameManager.Instance.Inventory.CountableResources.ToType(ResourceType.Population);
        if (populationResource.Count <= GameManager.Instance.Balance.MinPopulationToLose)
        {
            Debug.Log("you lose!");
        }
    }
}
