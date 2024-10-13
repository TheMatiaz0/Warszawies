using Rubin;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    private Ticker NextEventTicker;
    private Ticker FinishEventTicker;
    public Queue<EventInstance> EventQueue = new();

    public List<EventData> AllPossibleEvents;
    public Transform Parent;
    public PortraitEventHud PortraitPrefab;
    public CardModal Card;

    private int PreviousEventsCount;
    private readonly List<PortraitEventHud> portraits = new();
    private bool test;

    private void Start()
    {
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

    private void OnResourcesCountChanged(CountableResource obj)
    {
        foreach (var eventData in EventQueue)
        {
            eventData.IsGoalAccomplished = IsEventAccomplished(eventData.Data);
        }
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
        }

        var instance = EventQueue.FirstOrDefault(x => x.Data == eventData);
        if (instance != null)
        {
            return;
        }
        AddToQueue(eventData);
    }

    public void Cancel(EventData eventData)
    {
        EventQueue = new Queue<EventInstance>(EventQueue.Where(x => x.Data != eventData));
        Remove(eventData);
        PayResources(eventData);
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
        EventQueue.Enqueue(new(eventData, false));
        var createdPortrait = Instantiate(PortraitPrefab, Parent);

        createdPortrait.Setup(eventData, Card);

        portraits.Add(createdPortrait);
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
            for (int i = 0; i < EventQueue.Count; i++)
            {
                var eventDataToCancel = GetFromQueue();
                Cancel(eventDataToCancel.Data);
            }

            var rngEventData = Randomer.Base.NextRandomElement(AllPossibleEvents);
            rngEventData = Instantiate(rngEventData);
            var percentage = PreviousEventsCount * GameManager.Instance.Balance.DifficultyPercentageForNextEvent;

            foreach (var item in rngEventData.Requirement)
            {
                // starting from 2
                var newRequirement = item.Count * (1 + percentage);
                item.Count = Mathf.CeilToInt(newRequirement);
            }

            Card.OpenWith(rngEventData);
            PreviousEventsCount += 1;
            test = true;
        }
    }
}
