using Rubin;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    private Ticker Ticker;
    public readonly Queue<EventInstance> EventQueue = new();

    public List<EventData> AllPossibleEvents;
    public Transform Parent;
    public PortraitEventHud PortraitPrefab;
    public CardModal Card;

    private readonly List<PortraitEventHud> portraits = new();

    private void Start()
    {
        Ticker = TickerCreator.CreateNormalTime(GameManager.Instance.Balance.TimeToFinishEvent);
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
        }
        else
        {
            AddToQueue(eventData);
        }
    }

    public void Cancel(EventData eventData)
    {
        var instances = EventQueue.ToList();

        var instance = instances.Find(x => x.Data == eventData);
        instances.Remove(instance);
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

        var p = Ticker.TimeGetter;
        var time = p.Invoke();
        createdPortrait.Setup(time, eventData, Card);

        portraits.Add(createdPortrait);
    }

    public EventInstance GetFromQueue()
    {
        var eventData = EventQueue.Dequeue();
        var portraitToBeRemoved = portraits.Find(x => x.EventData == eventData.Data);
        if (portraitToBeRemoved != null)
        {
            Destroy(portraitToBeRemoved.gameObject);
        }
        portraits.Remove(portraitToBeRemoved);

        return eventData;
    }

    private void Update()
    {
        if (Ticker.Push() && EventQueue != null)
        {
            for (int i = 0; i < EventQueue.Count; i++)
            {
                var eventDataToCancel = GetFromQueue();
                Cancel(eventDataToCancel.Data);
            }

            var rngNum = UnityEngine.Random.Range(1, 3);

            for (int i = 0; i < rngNum; i++)
            {
                var rngEventData = Randomer.Base.NextRandomElement(AllPossibleEvents);
                Card.OpenWith(rngEventData);
            }
        }
    }
}
