using System;

[Serializable]
public class EventInstance
{
    public event Action<bool> OnAccomplishmentChanged;

    public EventData Data;
    public bool IsGoalAccomplished 
    {
        get => _isGoalAccomplished;
        set
        {
            _isGoalAccomplished = value;
            OnAccomplishmentChanged?.Invoke(value);
        }
    }

    private bool _isGoalAccomplished;

    public EventInstance(EventData data, bool isGoalAccomplished)
    {
        Data = data;
        IsGoalAccomplished = isGoalAccomplished;
    }
}
