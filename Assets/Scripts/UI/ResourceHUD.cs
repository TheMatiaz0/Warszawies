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

    public virtual void Refresh(int count, int idleCount)
    {
        CountDisplay.text = count.ToString();

        string prefix = idleCount == 0 ? "" : idleCount > 0 ? "+" : "-";
        string color = idleCount == 0 ? "white" : idleCount > 0 ? "green" : "red";

        IdleCountDisplay.text = $"<color={color}>({prefix}{idleCount})</color>";
    }
}
