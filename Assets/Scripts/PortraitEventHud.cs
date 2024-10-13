using Rubin;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PortraitEventHud : MonoBehaviour
{
    public Image Portrait;
    public Image TimeSlider;
    public Button Hyperlink;
    public Image CompleteOutline;

    [Header("Dynamic")]
    public EventData EventData;
    public CardModal Card;

    private Ticker ticker;

    public void Setup(EventData eventData, CardModal card)
    {
        ticker = TickerCreator.CreateNormalTime(GameManager.Instance.Balance.TimeToFinishEvent);

        Portrait.sprite = eventData.SmallThumbnail;
        Card = card;
       
        EventData = eventData;
        Hyperlink.onClick.AddListener(GoTo);

        foreach (var eventQueue in GameManager.Instance.EventManager.EventQueue.ToList())
        {
            eventQueue.OnAccomplishmentChanged += OnAccomplishmentChanged;
        }
    }

    private void Update()
    {
        var percentage = ticker.Passed / GameManager.Instance.Balance.TimeToFinishEvent;
        TimeSlider.fillAmount = percentage;

        if (percentage >= 0.99f)
        {
            GameManager.Instance.EventManager.Cancel(EventData);
        }
    }

    private void OnDestroy()
    {
        foreach (var eventQueue in GameManager.Instance.EventManager.EventQueue.ToList())
        {
            eventQueue.OnAccomplishmentChanged -= OnAccomplishmentChanged;
        }
    }

    private void OnAccomplishmentChanged(bool canAccept)
    {
        CompleteOutline.gameObject.SetActive(canAccept);
    }

    private void GoTo()
    {
        Card.OpenWith(EventData);
    }
}
