using System;
using System.Collections;
using System.Collections.Generic;
using Mono.Cecil;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ESCMenu : MonoBehaviour
{
    private bool isPaused = false;
    public GameObject pauseMenuUI;
    public GameObject settingsMenuUI;
    public GameObject[] settingsPanels;
    public List<Button> buttons;
    public CameraFadeScript cameraFadeScript;

    // Main Events

    void Awake()
    {
        pauseMenuUI = GameObject.FindGameObjectWithTag("PauseMenu");
        settingsMenuUI = GameObject.FindGameObjectWithTag("PauseSettings");
        cameraFadeScript = FindFirstObjectByType<CameraFadeScript>();

        if (pauseMenuUI == null || settingsMenuUI == null || cameraFadeScript == null)
        {
            Debug.LogError("Pause Menu UI or Settings Menu UI or Camera Fade Script not found in the scene.");
        }

        pauseMenuUI.SetActive(false);
        settingsMenuUI.SetActive(false);
        isPaused = false;
        int count = 0;
        foreach (Button btn in buttons)
        {
            int index = count; 
            btn.onClick.AddListener(() => OpenMenu(index));
            count++;
        }
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }
    }

    private void TogglePauseMenu()
    {
        if (isPaused)
        {
            Time.timeScale = 1f;
            pauseMenuUI.SetActive(false);
            settingsMenuUI.SetActive(false);
            isPaused = false;
        }
        else
        {
            Time.timeScale = 0f;
            pauseMenuUI.SetActive(true);
            settingsMenuUI.SetActive(false);
            isPaused = true;
        }
    }

    // Button Events

    public void OpenMenu(int index){
        for (int i = 0; i < settingsPanels.Length; i++)
        {
            settingsPanels[i].SetActive(false);
        }
        settingsPanels[index].SetActive(true);
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;
        isPaused = false;
        pauseMenuUI.SetActive(false);
        settingsMenuUI.SetActive(false);
        StartCoroutine(MainMenuCoroutine());
    }
    public IEnumerator MainMenuCoroutine()
    {
        cameraFadeScript.StartFade(0.2f,true,true);
        yield return new WaitForSeconds(0.2f);
        SceneData.SceneToLoad = "MainMenu";
        SceneData.LoadScene();

    }

    public void Settings()
    {
        pauseMenuUI.SetActive(false);
        settingsMenuUI.SetActive(true);
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        isPaused = false;
        pauseMenuUI.SetActive(false);
        settingsMenuUI.SetActive(false);
        StartCoroutine(RestartCoroutine());
    }

    public IEnumerator RestartCoroutine()
    {
        cameraFadeScript.StartFade(0.1f,true,true);
        yield return new WaitForSeconds(0.1f);
        SceneLoader.ReloadCurrentScene();
    }

    public void BackButton()
    {
        pauseMenuUI.SetActive(true);
        settingsMenuUI.SetActive(false);
    }


}
