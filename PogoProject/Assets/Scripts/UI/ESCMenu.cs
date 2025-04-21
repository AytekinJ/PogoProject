using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ESCMenu : MonoBehaviour
{
    private bool isPaused = false;

    [Header("UI Panels")]
    public GameObject pauseMenuUI;
    public GameObject settingsMenuUI;
    public GameObject[] settingsPanels;

    [Header("Settings Navigation Buttons")]
    public List<Button> buttons;

    [Header("Camera Fade Script")]
    public CameraFadeScript cameraFadeScript;

    void Awake()
    {
        pauseMenuUI = GameObject.FindGameObjectWithTag("PauseMenu");
        settingsMenuUI = GameObject.FindGameObjectWithTag("PauseSettings");
        cameraFadeScript = FindFirstObjectByType<CameraFadeScript>();

        if (pauseMenuUI == null || settingsMenuUI == null || cameraFadeScript == null)
        {
            Debug.LogError("🚨 UI veya Fade Script eksik! Sahneye düzgün atılmamış olabilir.");
            return;
        }


        Transform parent = GameObject.Find("BackGround").transform;
        settingsPanels = new GameObject[4];
        for (int i = 0; i < 4; i++)
        {
            settingsPanels[i] = parent.GetChild(i + 1).gameObject; 
        }
        settingsPanels[0].SetActive(true); 
        GameObject[] buttonObjects = GameObject.FindGameObjectsWithTag("SettingsNavigation");
        buttons = new List<Button>();
        for (int i = buttonObjects.Length - 1; i >= 0; i--)
        {
            Button btn = buttonObjects[i].GetComponent<Button>();
            if (btn != null)
                buttons.Add(btn);
        }

        for (int i = 0; i < buttons.Count; i++)
        {
            int index = i; // closure için sabitleme
            buttons[i].onClick.AddListener(() => OpenMenu(index));
        }

        pauseMenuUI.SetActive(false);
        settingsMenuUI.SetActive(false);
        isPaused = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            TogglePauseMenu();
    }

    private void TogglePauseMenu()
    {
        isPaused = !isPaused;

        Time.timeScale = isPaused ? 0f : 1f;
        pauseMenuUI.SetActive(isPaused);
        settingsMenuUI.SetActive(false);
    }

    public void OpenMenu(int index)
    {
        for (int i = 0; i < settingsPanels.Length; i++)
            settingsPanels[i].SetActive(i == index);
    }

    public void MainMenu()
    {
        ResetPauseState();
        StartCoroutine(MainMenuCoroutine());
    }

    private IEnumerator MainMenuCoroutine()
    {
        cameraFadeScript.StartFade(0.2f, true, true);
        yield return new WaitForSeconds(0.2f);
        SceneData.SceneToLoad = "MainMenu";
        SceneData.LoadScene();
    }

    public void Restart()
    {
        ResetPauseState();
        StartCoroutine(RestartCoroutine());
    }

    private IEnumerator RestartCoroutine()
    {
        cameraFadeScript.StartFade(0.1f, true, true);
        yield return new WaitForSeconds(0.1f);
        SceneLoader.ReloadCurrentScene();
    }

    public void Settings()
    {
        pauseMenuUI.SetActive(false);
        settingsMenuUI.SetActive(true);
    }
    public void BackButtonMain()
    {
        TogglePauseMenu();
    }

    public void BackButton()
    {
        pauseMenuUI.SetActive(true);
        settingsMenuUI.SetActive(false);
    }

    private void ResetPauseState()
    {
        Time.timeScale = 1f;
        isPaused = false;
        pauseMenuUI.SetActive(false);
        settingsMenuUI.SetActive(false);
    }
}
