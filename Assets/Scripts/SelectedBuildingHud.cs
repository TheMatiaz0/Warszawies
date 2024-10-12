using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectedBuildingHud : MonoBehaviour
{
    public Text BuildingName;
    public ResourceAmountHud ResultDisplayPrefab;
    public Transform parent;

    public List<ResourceAmountHud> RequirementsDisplay;

    public void Setup(BuildingData buildingData)
    {
        // clear up all children from parent
        foreach (Transform item in parent)
        {
            Destroy(item.gameObject);
        }

        // create children
        BuildingName.text = buildingData.Name;
        foreach (var resultData in buildingData.Result)
        {
            var resultDisplay = Instantiate(ResultDisplayPrefab, parent);
            resultDisplay.Setup(resultData.ResourceType.Icon, resultData.Count);
        }
    } 
}
