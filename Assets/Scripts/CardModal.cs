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

    public Transform RequirementLayout;
    public Transform PenaltyLayout;
    public Transform RewardLayout;

    public Button CancelButton;
    public Button AcceptButton;
    public Button RestartButton;

    public CanvasGroup CanvasGroup;
    public bool IsActive = true;

    private EventData EventData;

    private void Awake()
    {
        Close();
    }

    public void OpenWith(EventData eventData)
    {
        if (!IsActive) return;

        Purge();

        EventData = eventData;

        if (!eventData.AbleToRestart)
        {
            CloseButton.onClick.AddListener(Close);
            ModalBackground.onClick.AddListener(Close);
            AcceptButton.onClick.AddListener(Accept);
            CancelButton.onClick.AddListener(Cancel);
            SpawnPayments(eventData);
        }
        else
        {
            RestartButton.onClick.AddListener(Restart);
        }

        SetupVisuals(eventData);

        RequirementLayout.gameObject.SetActive(!eventData.AbleToRestart);
        PenaltyLayout.gameObject.SetActive(eventData.AbleToCancel);
        RewardLayout.gameObject.SetActive(eventData.AbleToAccept);

        AcceptButton.gameObject.SetActive(eventData.AbleToAccept);
        CancelButton.gameObject.SetActive(eventData.AbleToCancel);
        RestartButton.gameObject.SetActive(eventData.AbleToRestart);

        Open();
    }

    private void Purge()
    {
        foreach (Transform item in RequirementLayout)
        {
            Destroy(item.gameObject);
        }

        foreach (Transform item in PenaltyLayout)
        {
            Destroy(item.gameObject);
        }

        foreach (Transform item in RewardLayout)
        {
            Destroy(item.gameObject);
        }
    }

    private void SpawnPayments(EventData data)
    {
        foreach (var resource in data.Requirement)
        {
            SpawnSinglePayment(RequirementLayout, resource, false);
        }

        foreach (var resource in data.FailCondition)
        {
            SpawnSinglePayment(PenaltyLayout, resource, true);
        }

        foreach (var resource in data.WinCondition)
        {
            SpawnSinglePayment(RewardLayout, resource, true);
        }
    }

    private void SpawnSinglePayment(Transform parent, CountableResource resource, bool shouldAddPrefix)
    {
        var payment = Instantiate(PaymentPrefab, parent);
        payment.Setup(resource.ResourceType.Icon, resource.Count, shouldAddPrefix);
    }

    private void SetupVisuals(EventData eventData)
    {
        Title.text = eventData.Title;
        Description.text = eventData.Description;
        IllustrationSlot.sprite = eventData.Illustration;
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
        CanvasGroup.blocksRaycasts = false;
        CanvasGroup.interactable = false;
        Time.timeScale = 1;
        Clear();
    }

    private void Open()
    {
        CanvasGroup.blocksRaycasts = true;
        CanvasGroup.interactable = true;
        CanvasGroup.alpha = 1;
        Time.timeScale = 0;
    }

    private void Restart()
    {
        SceneManager.LoadScene("GameplayScene");
    }
}
