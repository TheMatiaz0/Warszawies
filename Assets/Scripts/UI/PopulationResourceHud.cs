public class PopulationResourceHud : ResourceHUD
{
    public override void Refresh(int count, int idleCount)
    {
        var goalForPopulation = GameManager.Instance.NextPopulationThreshold;
        CountDisplay.text = $"{count}/{goalForPopulation}";
    }
}
