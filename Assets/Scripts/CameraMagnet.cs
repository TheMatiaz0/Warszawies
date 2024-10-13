using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMagnet : MonoBehaviour
{
    public float DestinationY = 1000;
    public float Duration = 30;

    private void Awake()
    {
        GameManager.Instance.PlayerController.IsInputActive = false;
        GameManager.Instance.PlayerController.PlayerCamera.transform.parent = this.transform;

        // GameManager.Instance.PlayerController.PlayerCamera.transform.eulerAngles = new(60, 40, 30);
        // GameManager.Instance.PlayerController.PlayerCamera.transform.localPosition = new(-37.4f, 37.6f, -27.7f);

        GameManager.Instance.PlayerController.PlayerCamera.transform.eulerAngles = new(43, -158, 0);
        GameManager.Instance.PlayerController.PlayerCamera.transform.localPosition = new(25.5f, 37f, 57.7f);

        this.transform.DOMoveY(DestinationY, Duration);


        GameManager.Instance.PlayerController.PlayerCamera.transform.DOLookAt(this.transform.position, Duration / 10);
        GameManager.Instance.PlayerController.PlayerCamera.transform.DORotate(new(0, 180, 0), Duration);

        GameManager.Instance.PlayerController.PlayerCamera.DOShakeRotation(Duration * 20f, 0.5f, 1, 60).SetLoops(-1, LoopType.Incremental);

        GameManager.Instance.PlayerController.PlayerCamera.DOShakePosition(Duration * 20f, 0.5f, 1, 60).SetLoops(-1, LoopType.Incremental);
    }
}
