using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ThresholdBuilding
{
    public BuildingData Building;
    public int Threshold;
}

[CreateAssetMenu(fileName = "Balance", menuName = "CRPK/Balance", order = 1)]
public class Balance : ScriptableObject
{
    public float CooldownForAllBuildings = 2;
    [Header("Food")]
    public float CooldownForCheckingFood = 2;
    public int FoodEatenPerTick;
    public int PopulationEatenByStarvation;
    public int MinPopulationToLose = 0;
    [Header("Palace")]
    public List<ThresholdBuilding> PopulationThresholds;
    [Header("Event")]
    public float TimeToFinishEvent;
}