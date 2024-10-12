using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Balance Balance;
    public ResourceInventory Inventory;
    public int NextPopulationThreshold;
    public CardModal Card;
    public EventData GameOverEvent;

    public static GameManager Instance { get; private set; }

    private CountableResource populationResource;

    private void Awake()
    {
        Instance = this;
        Inventory = Instantiate(Inventory);

        NextPopulationThreshold = Balance.PopulationThresholds[1].Threshold;
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
            // Run GameOver card
            Card.Setup(GameOverEvent);

            Debug.Log("you lose!");
        }
    }
}
