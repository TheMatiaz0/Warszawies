using Rubin;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    private Ticker Ticker;
    private readonly Queue<EventData> EventQueue = new();

    private void Awake()
    {
        Ticker = TickerCreator.CreateNormalTime(GameManager.Instance.Balance.TimeToFinishEvent);
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
    }

    public EventData GetFromQueue()
    {
        return EventQueue.Dequeue();
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
        }
    }
}
