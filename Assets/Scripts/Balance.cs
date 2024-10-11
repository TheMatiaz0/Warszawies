using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Balance", menuName = "CRPK/Balance", order = 1)]
public class Balance : ScriptableObject
{
    public static Balance Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public ResourceInventory Inventory;
    public float CooldownForAllBuildings = 2;
    [Header("Food")]
    public float CooldownForCheckingFood = 2;
    public int FoodEatenPerPerson;
    public int PopulationEatenByStarvation;

}
