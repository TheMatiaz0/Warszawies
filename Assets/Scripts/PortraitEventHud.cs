using UnityEngine;
using UnityEngine.UI;

public class PortraitEventHud : MonoBehaviour
{
    public Image Portrait;
    public Image TimeSlider;
    public Button Hyperlink;

    [Header("Dynamic")]
    public EventData EventData;
    public CardModal Card;

    public void Setup(float time, EventData eventData, CardModal card)
    {
        Portrait.sprite = eventData.SmallThumbnail;
        Card = card;

        var percentage = time / GameManager.Instance.Balance.TimeToFinishEvent;
        TimeSlider.fillAmount = percentage;

        EventData = eventData;
        Hyperlink.onClick.AddListener(GoTo);
    }

    private void GoTo()
    {
        Card.OpenWith(EventData);
    }
}
