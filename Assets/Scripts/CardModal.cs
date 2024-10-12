using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardModal : MonoBehaviour
{
    public EventManager EventManager;

    public Button CloseButton;
    public Button ModalBackground;

    public Text Title;
    public Text Description;

    public ResourceAmountHud PaymentPrefab;

    public Button CancelButton;
    public Button AcceptButton;

    public CanvasGroup CanvasGroup;

    private EventData EventData;

    public void Setup(EventData eventData)
    {
        EventData = eventData;
        CloseButton.onClick.AddListener(Close);
        ModalBackground.onClick.AddListener(Close);
        AcceptButton.onClick.AddListener(Accept);
        CancelButton.onClick.AddListener(Cancel);
    }

    private void Clear()
    {
        CloseButton.onClick.RemoveListener(Close);
        ModalBackground.onClick.RemoveListener(Close);
        AcceptButton.onClick.RemoveListener(Accept);
        CancelButton.onClick.RemoveListener(Cancel);
    }

    // accept -> if has required items: drop items else add to quest backlog
    // cancel -> pay the price

    private void Accept()
    {
        EventManager.Accept(EventData);
        Close();
    }

    private void Cancel()
    {
        EventManager.Cancel(EventData);
        Close();
    }

    private void Close()
    {
        CanvasGroup.alpha = 0;
        Clear();
    }

    private void Open()
    {
        CanvasGroup.alpha = 1;
    }
}
