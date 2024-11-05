using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuHud : MonoBehaviour
{
    public Button StartGameBtn;
    public Button CreditsBtn;
    public Button ExitGameBtn;
    public Button CancelCreditsBtn;

    public GameObject CreditsObject;
    public GameObject MenuObject;

    private void Awake()
    {
        StartGameBtn.onClick.AddListener(StartGame);
        CreditsBtn.onClick.AddListener(Credits);
        ExitGameBtn.onClick.AddListener(ExitGame);
        CancelCreditsBtn.onClick.AddListener(ExitCredits);
    }

    private void StartGame()
    {
        SceneManager.LoadScene("GameplayScene");
    }

    private void Credits()
    {
        MenuObject.SetActive(false);
        CreditsObject.SetActive(true);
    }

    private void ExitCredits()
    {
        MenuObject.SetActive(true);
        CreditsObject.SetActive(false);
    }

    private void ExitGame()
    {
        Application.Quit(0);
    }
}
