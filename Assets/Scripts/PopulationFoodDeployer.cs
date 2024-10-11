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

    private void Awake()
    {
        Ticker = TickerCreator.CreateNormalTime(Balance.Instance.CooldownForCheckingFood);

        population = Balance.Instance.Inventory.CountableResources.Find(x => x.ResourceType == PopulationResource);
        food = Balance.Instance.Inventory.CountableResources.Find(x => x.ResourceType == FoodResource);
    }

    public void CheckRequiredFood()
    {
        var requiredFood = population.Count * Balance.Instance.FoodEatenPerPerson;

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
        population.Count -= Balance.Instance.PopulationEatenByStarvation;
    }

    private void Update()
    {
        if (Ticker.Push())
        {
            CheckRequiredFood();
        }
    }
}
