using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingPieInstance : MonoBehaviour
{
    public List<ButtonEvents> ButtonEvents;

    private void Awake()
    {
        foreach (var item in ButtonEvents)
        {
            item.OnPointerClickEvent += OnPointerClick;
            item.OnPointerEnterEvent += OnPointerEnter;
            item.OnPointerExitEvent += OnPointerExit;
        }
    }

    private void OnDestroy()
    {
        foreach (var item in ButtonEvents)
        {
            item.OnPointerClickEvent -= OnPointerClick;
            item.OnPointerEnterEvent -= OnPointerEnter;
            item.OnPointerExitEvent -= OnPointerExit;
        }
    }

    private void OnPointerClick(UnityEngine.EventSystems.PointerEventData pointerData)
    {

    }

    private void OnPointerEnter(UnityEngine.EventSystems.PointerEventData pointerData)
    {

    }

    private void OnPointerExit(UnityEngine.EventSystems.PointerEventData pointerData)
    {

    }
}
