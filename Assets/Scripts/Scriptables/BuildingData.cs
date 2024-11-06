using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Building", menuName = "CRPK/Building", order = 1)]
public class BuildingData : ResettableScriptableObject
{
    public string Name;

    [Header("Crafting")]
    public List<CountableResource> Requirements;
    public List<CountableResource> Result;
    [Header("World")]
    public BuildingInstance PrefabToSpawn;
    public BlockingObjects AllowedObjects;
    public Vector3 StartPosition;
    public Sprite BuildingIcon;

    [Header("Audio")]
    public AudioClip OnBuiltClip;
    public AudioClip OnIdleClip;
}
