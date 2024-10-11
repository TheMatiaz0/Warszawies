using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Requirement
{
    public ResourceData resourceType;
    public int count;
}

[CreateAssetMenu(fileName = "Building", menuName = "CRPK/Buildings", order = 1)]
public class BuildingData : ScriptableObject
{
    public List<Requirement> requirements;
}
