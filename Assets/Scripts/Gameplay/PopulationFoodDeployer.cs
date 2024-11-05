using Rubin;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopulationFoodDeployer : MonoBehaviour
{
    public ResourceData FoodResource;
    public ResourceData PopulationResource;

    private Ticker Ticker;
    private CountableResource population;
    private CountableResource food;

    private void Start()
    {
        Ticker = TickerCreator.CreateNormalTime(GameManager.Instance.Balance.CooldownForCheckingFood);

        population = GameManager.Instance.Inventory.CountableResources.ToType(ResourceType.Population);
        food = GameManager.Instance.Inventory.CountableResources.ToType(ResourceType.Food);
    }

    public void CheckRequiredFood()
    {
        var requiredFood = ((population.Count / 10) * GameManager.Instance.Balance.FoodEatenPerTick) / 2;

        if (food.Count >= requiredFood)
        {
            food.Count -= requiredFood;
        }
        else
        {
            StartStarving();
        }
    }

    private void StartStarving()
    {
        population.Count -= GameManager.Instance.Balance.PopulationEatenByStarvation;
    }

    private void Update()
    {
        if (Ticker.Push())
        {
            CheckRequiredFood();
        }
    }
}
