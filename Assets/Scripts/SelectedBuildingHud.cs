using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectedBuildingHud : MonoBehaviour
{
    public Text BuildingName;
    public ResourceAmountHud ResourceAmountPrefab;
    public Transform resultParent;
    public Transform requirementParent;

    public void Setup(BuildingData buildingData)
    {
        // clear up all children from parent
        foreach (Transform item in resultParent)
        {
            Destroy(item.gameObject);
        }

        foreach (Transform item in requirementParent)
        {
            Destroy(item.gameObject);
        }

        // create children
        BuildingName.text = buildingData.Name;
        foreach (var resultData in buildingData.Result)
        {
            var display = Instantiate(ResourceAmountPrefab, resultParent);
            display.Setup(resultData.ResourceType.Icon, resultData.Count);
        }
        foreach (var item in buildingData.Requirements)
        {
            var display = Instantiate(ResourceAmountPrefab, requirementParent);
            display.Setup(item.ResourceType.Icon, item.Count);
        }
    } 
}
