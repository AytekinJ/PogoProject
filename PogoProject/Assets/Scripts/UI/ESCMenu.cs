using System;
using System.Collections;
using Mono.Cecil;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ESCMenu : MonoBehaviour
{
    private bool isPaused = false;
    public GameObject pauseMenuUI;
    public GameObject settingsMenuUI;

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
        SceneManager.LoadScene("LoadingScene", LoadSceneMode.Single);
        GC.Collect();
        Resources.UnloadUnusedAssets();
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
        SceneLoader.ReloadCurrentScene_ClearAllocatedMemory();
    }

    public void BackButton()
    {
        pauseMenuUI.SetActive(true);
        settingsMenuUI.SetActive(false);
    }
}
