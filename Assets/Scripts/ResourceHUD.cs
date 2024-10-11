using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceHUD : MonoBehaviour
{
    public ResourceData ResourceType;

    public Text CountDisplay;
    public Image IconDisplay;

    public void Refresh(CountableResource countableResource)
    {
        if (ResourceType == countableResource.ResourceType)
        {
            CountDisplay.text = countableResource.Count.ToString();
            IconDisplay.sprite = countableResource.ResourceType.Icon;
        }
    }
}
