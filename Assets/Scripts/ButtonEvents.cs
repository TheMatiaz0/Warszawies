using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonEvents : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Button Button { get; private set; }

    public event Action OnPointerClickEvent;
    public event Action<PointerEventData> OnPointerEnterEvent;
    public event Action<PointerEventData> OnPointerExitEvent;

    private void Awake()
    {
        Button = GetComponent<Button>();
    }

    private void Start()
    {
        Button.onClick.AddListener(InvokeEvent);
    }

    private void InvokeEvent()
    {
        OnPointerClickEvent?.Invoke();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnPointerEnterEvent?.Invoke(eventData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnPointerExitEvent?.Invoke(eventData);
    }
}
