using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class ResourceRepresentation
{
    public ResourceData ResourceType;

    public Text Count;
    public Image Icon;
}

public class HUD : MonoBehaviour
{
    public List<ResourceRepresentation> ResourceRepresentations;

    public void Refresh(CountableResource resource)
    {
        var resourceHud = ResourceRepresentations.Find(x => x.ResourceType == resource.ResourceType);

        if (resourceHud != null)
        {
            resourceHud.Count.text = resource.Count.ToString();
            resourceHud.Icon.sprite = resource.ResourceType.Icon;
        }
    }
}
