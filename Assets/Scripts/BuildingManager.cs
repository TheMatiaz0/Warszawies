using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    public List<BuildingInstance> BuildingInstances;

    public void Build(BuildingData data, Vector3 position)
    {
        var buildingInstance = Instantiate(data.PrefabToSpawn, position, Quaternion.identity);
        BuildingInstances.Add(buildingInstance);
    }
}
