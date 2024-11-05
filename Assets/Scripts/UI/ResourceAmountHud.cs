using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class ResourceAmountHud : MonoBehaviour
{
    public Image ResourceIcon;
    public Text Count;

    public void Setup(Sprite resourceIcon, int count, bool shouldAddPrefix = false)
    {
        ResourceIcon.sprite = resourceIcon;

        string prefix = string.Empty;
        if (shouldAddPrefix)
        {
            prefix = count > 0 ? "+" : "";
        }

        Count.text = $"{prefix}{count}";
    }
}
