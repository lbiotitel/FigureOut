using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleUI : MonoBehaviour
{
    [SerializeField] private Button gameStartButton;
    [SerializeField] private Button gameExitButton;


    private void Awake()
    {
        gameStartButton.onClick.AddListener(OnGameStartButtonClicked);
        gameExitButton.onClick.AddListener(OnGameExitButtonClicked);
    }


    private void OnGameStartButtonClicked()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(EScene.BootStrap.ToString());
    }

    private void OnGameExitButtonClicked()
    {
        Application.Quit();
    }


}
