using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Building", menuName = "CRPK/Building", order = 1)]
public class BuildingData : ScriptableObject
{
    [Header("Crafting")]
    public List<CountableResource> Requirements;
    public ResourceData Result;
    [Header("World")]
    public BuildingInstance PrefabToSpawn;
}
