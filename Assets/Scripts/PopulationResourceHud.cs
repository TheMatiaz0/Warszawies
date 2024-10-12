using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopulationResourceHud : ResourceHUD
{
    public override void Refresh(int count, int idleCount)
    {
        var goalForPopulation = GameManager.Instance.NextPopulationThreshold;
        CountDisplay.text = $"{count}/{goalForPopulation}";
        // IdleCountDisplay.text = $"(+{idleCount.ToString()})";
    }
}
