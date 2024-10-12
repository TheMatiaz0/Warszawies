using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceHUD : MonoBehaviour
{
    public ResourceData ResourceType;

    public Text CountDisplay;
    public Text IdleCountDisplay;

    public void Refresh(int count, int idleCount)
    {
        CountDisplay.text = count.ToString();
        IdleCountDisplay.text = $"(+{idleCount.ToString()})";

        // IconDisplay.sprite = ResourceType.Icon;
    }
}
