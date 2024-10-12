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
    public EventManager EventManager;

    public void Setup(float time, EventData eventData, CardModal card)
    {
        Portrait.sprite = eventData.SmallThumbnail;
        Card = card;

        // time
        var percentage = time / GameManager.Instance.Balance.TimeToFinishEvent;
        TimeSlider.fillAmount = percentage;

        EventData = eventData;
        Hyperlink.onClick.AddListener(GoTo);

        foreach (var eventQueue in GameManager.Instance.EventManager.EventQueue.ToList())
        {
            eventQueue.OnAccomplishmentChanged += OnAccomplishmentChanged;
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
