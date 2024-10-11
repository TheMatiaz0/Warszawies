using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Building", menuName = "CRPK/Building", order = 1)]
public class BuildingData : ResettableScriptableObject
{
    [Header("Crafting")]
    public List<CountableResource> Requirements;
    public List<CountableResource> Result;
    [Header("World")]
    public BuildingInstance PrefabToSpawn;
    public BlockingObjects AllowedObjects;
}
