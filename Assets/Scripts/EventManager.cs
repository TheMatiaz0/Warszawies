using Rubin;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    private Ticker Ticker;
    private readonly Queue<EventData> EventQueue = new();

    // rng num (at start 1, later 2, maybe later 3?)
    // rng content
    public List<EventData> AllPossibleEvents;
    public Transform Parent;
    public PortraitEventHud PortraitPrefab;
    public CardModal Card;

    private List<PortraitEventHud> portraits = new();

    private void Start()
    {
        Ticker = TickerCreator.CreateNormalTime(GameManager.Instance.Balance.TimeToFinishEvent);
        Clear();
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
        if (CanGetReward(eventData))
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
        EventQueue.ToList().Remove(eventData);
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

    public bool CanGetReward(EventData eventData)
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
        EventQueue.Enqueue(eventData);
        var createdPortrait = Instantiate(PortraitPrefab, Parent);

        var p = Ticker.TimeGetter;
        var time = p.Invoke();
        createdPortrait.Setup(time, eventData, Card);

        portraits.Add(createdPortrait);
    }

    public EventData GetFromQueue()
    {
        var eventData = EventQueue.Dequeue();
        var portraitToBeRemoved = portraits.Find(x => x.EventData == eventData);
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
                Cancel(eventDataToCancel);
            }

            // get rng num
            // get rng content from nums for loop

            // 1 OR 2
            var rngNum = Random.Range(1, 3);

            for (int i = 0; i < rngNum; i++)
            {
                var rngEventData = Randomer.Base.NextRandomElement(AllPossibleEvents);
                Card.OpenWith(rngEventData);
            }
        }
    }
}
