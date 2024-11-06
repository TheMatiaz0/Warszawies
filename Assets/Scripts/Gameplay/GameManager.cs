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
    public BuildingManager BuildingManager;
    public EventManager EventManager;
    public PlayerController PlayerController;
    public AudioSource MusicMain;

    public static GameManager Instance { get; private set; }

    private CountableResource populationResource;
    private int currentIndex;

    private void Awake()
    {
        Instance = this;
        Inventory = Instantiate(Inventory);

        currentIndex = 1;
        NextPopulationThreshold = Balance.PopulationThresholds[currentIndex].Threshold;
        populationResource = Inventory.CountableResources.ToType(ResourceType.Population);
        populationResource.OnCountChanged += OnPopulationChanged;
    }

    private void OnDestroy()
    {
        populationResource.OnCountChanged -= OnPopulationChanged;
    }

    private void OnPopulationChanged(CountableResource countable)
    {
        CheckAdvanceCondition(countable);

        CheckWinCondition();
        CheckLoseCondition();
    }

    private void CheckAdvanceCondition(CountableResource countable)
    {
        if (countable.Count >= NextPopulationThreshold)
        {
            NextPopulationThreshold = Balance.PopulationThresholds[currentIndex + 1].Threshold;
        }
    }

    public void CheckWinCondition()
    {
        if (populationResource.Count >= Balance.PopulationThresholds[^1].Threshold)
        {
            Debug.Log("you win!");
        }
    }

    public void CheckLoseCondition()
    {
        if (populationResource.Count <= Balance.MinPopulationToLose)
        {
            Card.OpenWith(GameOverEvent);

            Debug.Log("you lose!");
        }
    }
}
