using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectedBuildingHud : MonoBehaviour
{
    public CanvasGroup CanvasGroup;
    public Text BuildingName;
    public Image BuildingIcon;
    public ResourceAmountHud ResourceAmountPrefab;
    public Transform resultParent;
    public Transform requirementParent;

    private void Awake()
    {
        Close();
    }

    public void Setup(BuildingData buildingData)
    {
        if (buildingData == null)
        {
            Close();
            return;
        }

        foreach (Transform item in resultParent)
        {
            Destroy(item.gameObject);
        }

        foreach (Transform item in requirementParent)
        {
            Destroy(item.gameObject);
        }

        BuildingName.text = buildingData.Name;
        if (buildingData.BuildingIcon != null)
        {
            BuildingIcon.sprite = buildingData.BuildingIcon;
        }
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

        Open();
    } 
    private void Open()
    {
        CanvasGroup.alpha = 1;
    }

    private void Close()
    {
        CanvasGroup.alpha = 0;
    }
}
