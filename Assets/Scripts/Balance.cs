using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Balance", menuName = "CRPK/Balance", order = 1)]
public class Balance : ScriptableObject
{
    public float CooldownForAllBuildings = 2;
    [Header("Food")]
    public float CooldownForCheckingFood = 2;
    public int FoodEatenPerPerson;
    public int PopulationEatenByStarvation;
    public int MinPopulationToLose = 0;
}