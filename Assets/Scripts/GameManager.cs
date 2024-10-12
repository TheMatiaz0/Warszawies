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

        populationResource = GameManager.Instance.Inventory.CountableResources.ToType(ResourceType.Population);
        populationResource.OnCountChanged += OnPopulationChanged;
    }

    private void OnDestroy()
    {
        populationResource.OnCountChanged -= OnPopulationChanged;
    }

    private void OnPopulationChanged(CountableResource countable)
    {
        CheckWinCondition();
        CheckLoseCondition();
    }

    public void CheckWinCondition()
    {
        if (populationResource.Count >= Balance.PopulationThresholds[^1].Threshold)
        {
            // last threshold overflown
            Debug.Log("you win!");
        }
    }

    public void CheckLoseCondition()
    {
        if (populationResource.Count <= Balance.MinPopulationToLose)
        {
            Debug.Log("you lose!");
        }
    }
}
