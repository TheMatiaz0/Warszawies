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

    public void Refresh(int count, Sprite representation)
    {
        CountDisplay.text = count.ToString();
        IconDisplay.sprite = representation;
    }
}
