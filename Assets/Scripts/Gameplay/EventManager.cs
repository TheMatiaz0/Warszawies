using Rubin;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    private Ticker NextEventTicker;
    public Queue<EventInstance> EventQueue = new();

    public List<EventData> AllPossibleEvents;
    public Transform Parent;
    public PortraitEventHud PortraitPrefab;
    public CardModal Card;
    public BuildingManager BuildingManager;
    [Header("Audio")]
    public AudioSource Audio;
    public AudioClip AcceptQuestClip;
    public AudioClip DeclineQuestClip;
    public AudioClip CompleteQuestClip;
    public AudioClip ReminderQuestClip;

    private int PreviousEventsCount;
    private readonly List<PortraitEventHud> portraits = new();
    private bool test;

    private List<int> CopiedEventsNum;

    private void Start()
    {
        CopiedEventsNum = new List<int>();
        for(int i = 0; i < AllPossibleEvents.Count; i++)
        {
            CopiedEventsNum.Add(i);
        }
        
        NextEventTicker = TickerCreator.CreateNormalTime(GameManager.Instance.Balance.TimeToNextEvent);
        Clear();

        foreach (var item in GameManager.Instance.Inventory.CountableResources)
        {
            item.OnCountChanged += OnResourcesCountChanged;
        }
    }

    private void OnDestroy()
    {
        foreach (var item in GameManager.Instance.Inventory.CountableResources)
        {
            item.OnCountChanged -= OnResourcesCountChanged;
        }
    }

    private void OnResourcesCountChanged(CountableResource resource)
    {
        foreach (var eventData in EventQueue)
        {
            eventData.IsGoalAccomplished = IsEventAccomplished(eventData.Data);
        }
    }
    
    public CountableResource GetDeltaOfRequirements()
    {
        CountableResource requiredResource = null;
        var latest = EventQueue.ToArray()[^1];
        
        int maxValue = int.MinValue;
        int minValue = int.MaxValue;
            
        foreach (var resource in latest.Data.Requirement)
        {
            var buildings = BuildingManager.GetAllBuildingsThatGatherResourceWithPalace(resource);
                
            var inventoryResource = GameManager.Instance.Inventory.CountableResources.Find(x => x.ResourceType == resource.ResourceType);
            var delta = resource.Count - inventoryResource.Count;

            if (buildings < minValue)
            {
                minValue = buildings;
                if (delta > maxValue)
                {
                    maxValue = delta;
                    requiredResource = resource;
                }
            }
        }

        return requiredResource;
    }

    private void Clear()
    {
        foreach (Transform item in Parent)
        {
            Destroy(item.gameObject);
        }
    }

    public void Accept(EventData eventData)
    {
        if (IsEventAccomplished(eventData))
        {
            SpendResources(eventData);
            GetRewards(eventData);
            Remove(eventData);
            Audio.PlayOneShot(CompleteQuestClip);
        }

        var instance = EventQueue.FirstOrDefault(x => x.Data == eventData);
        if (instance != null)
        {
            return;
        }
        AddToQueue(eventData);
        Audio.Stop();
        Audio.PlayOneShot(AcceptQuestClip);
    }

    public void Cancel(EventData eventData)
    {
        EventQueue = new Queue<EventInstance>(EventQueue.Where(x => x.Data != eventData));
        Remove(eventData);
        PayResources(eventData);

        Audio.Stop();
        Audio.PlayOneShot(DeclineQuestClip);
    }

    public void PayResources(EventData eventData)
    {
        foreach (var requiredResource in eventData.FailCondition)
        {
            foreach (var currentResource in GameManager.Instance.Inventory.CountableResources)
            {
                if (currentResource.ResourceType == requiredResource.ResourceType)
                {
                    currentResource.Count -= requiredResource.Count;
                }
            }
        }
    }

    public void SpendResources(EventData eventData)
    {
        foreach (var requiredResource in eventData.Requirement)
        {
            foreach (var currentResource in GameManager.Instance.Inventory.CountableResources)
            {
                if (currentResource.ResourceType == requiredResource.ResourceType)
                {
                    currentResource.Count -= requiredResource.Count;
                }
            }
        }
    }

    public void GetRewards(EventData eventData)
    {
        foreach (var requiredResource in eventData.WinCondition)
        {
            foreach (var currentResource in GameManager.Instance.Inventory.CountableResources)
            {
                if (currentResource.ResourceType == requiredResource.ResourceType)
                {
                    currentResource.Count += requiredResource.Count;
                }
            }
        }
    }

    public bool IsEventAccomplished(EventData eventData)
    {
        foreach (var requiredResource in eventData.Requirement)
        {
            foreach (var currentResource in GameManager.Instance.Inventory.CountableResources)
            {
                if (requiredResource.ResourceType == currentResource.ResourceType && currentResource.Count < requiredResource.Count)
                {
                    return false;
                }
            }
        }

        return true;
    }

    public void AddToQueue(EventData eventData)
    {
        var t = new EventInstance(eventData, false);
        t.OnAccomplishmentChanged += OnGoalAccomplished;
        EventQueue.Enqueue(t);
        var createdPortrait = Instantiate(PortraitPrefab, Parent);

        createdPortrait.Setup(eventData, Card);

        portraits.Add(createdPortrait);
    }

    private void OnGoalAccomplished(EventInstance instance, bool isTrue)
    {
        if (isTrue)
        {
            instance.OnAccomplishmentChanged -= OnGoalAccomplished;
            Audio.PlayOneShot(ReminderQuestClip);
        }
    }

    public EventInstance GetFromQueue()
    {
        var eventData = EventQueue.Dequeue();
        Remove(eventData.Data);

        return eventData;
    }

    private void Remove(EventData eventData)
    {
        var portraitToBeRemoved = portraits.Find(x => x.EventData == eventData);
        if (portraitToBeRemoved != null)
        {
            Destroy(portraitToBeRemoved.gameObject);
        }
        portraits.Remove(portraitToBeRemoved);
        NextEventTicker.Reset();
        test = false;
    }

    private void Update()
    {
        if (NextEventTicker.Done && EventQueue != null && !test)
        {
            if(CopiedEventsNum.Count == 0)
            {
                CopiedEventsNum = new List<int>();
                for (int i = 0; i < AllPossibleEvents.Count; i++)
                {
                    CopiedEventsNum.Add(i);
                }
            }
            int selectedEventNum = Randomer.Base.NextRandomElement(CopiedEventsNum);
            var rngEventData = AllPossibleEvents[selectedEventNum];
            CopiedEventsNum.Remove(selectedEventNum);
            
            rngEventData = Instantiate(rngEventData);
            var percentage = PreviousEventsCount * GameManager.Instance.Balance.DifficultyPercentageForNextEvent;

            foreach (var item in rngEventData.Requirement)
            {
                // starting from 2
                var newRequirement = item.Count * (1 + percentage);
                item.Count = Mathf.CeilToInt(newRequirement);
            }

            Card.OpenWith(rngEventData);
            Audio.PlayOneShot(rngEventData.OnEventStartClip);
            PreviousEventsCount += 1;
            test = true;
        }
    }
}
