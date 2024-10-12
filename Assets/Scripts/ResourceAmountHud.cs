using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class ResourceAmountHud : MonoBehaviour
{
    public Image ResourceIcon;
    public Text Count;

    public void Setup(Sprite resourceIcon, int count)
    {
        ResourceIcon.sprite = resourceIcon;
        Count.text = count.ToString();
    }
}
