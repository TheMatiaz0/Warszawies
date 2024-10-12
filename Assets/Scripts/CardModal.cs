using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CardModal : MonoBehaviour
{
    public EventManager EventManager;

    public Button CloseButton;
    public Button ModalBackground;

    public Text Title;
    public Text Description;
    public Image IllustrationSlot;

    public ResourceAmountHud PaymentPrefab;

    public Transform RequiredLayout;
    public Transform PayLayout;
    public Transform RewardLayout;

    public Button CancelButton;
    public Button AcceptButton;
    public Button RestartButton;

    public CanvasGroup CanvasGroup;

    private EventData EventData;

    private void Awake()
    {
        Close();
    }

    public void OpenWith(EventData eventData)
    {
        EventData = eventData;
        CloseButton.onClick.AddListener(Close);
        ModalBackground.onClick.AddListener(Close);
        AcceptButton.onClick.AddListener(Accept);
        CancelButton.onClick.AddListener(Cancel);
        RestartButton.onClick.AddListener(Restart);

        Title.text = eventData.Title;
        Description.text = eventData.Description;
        IllustrationSlot.sprite = eventData.Illustration;
        
        RequiredLayout.gameObject.SetActive(!eventData.AbleToRestart);
        PayLayout.gameObject.SetActive(eventData.AbleToCancel);
        RewardLayout.gameObject.SetActive(eventData.AbleToAccept);

        AcceptButton.gameObject.SetActive(eventData.AbleToAccept);
        CancelButton.gameObject.SetActive(eventData.AbleToCancel);
        RestartButton.gameObject.SetActive(eventData.AbleToRestart);

        Open();
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

    private void Restart()
    {
        SceneManager.LoadScene("GameplayScene");
    }
}
