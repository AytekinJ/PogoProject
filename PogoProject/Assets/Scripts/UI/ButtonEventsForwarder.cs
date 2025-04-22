using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private GameObject general;
    [SerializeField] private GameObject sound;
    [SerializeField] private GameObject video;
    [SerializeField] private GameObject other;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject levelSelectionMenu;

    void Start()
    {
        if (SceneData.isLevelSelection)
        {
            mainMenu.SetActive(false);
            settingsMenu.SetActive(false);
            levelSelectionMenu.SetActive(true);
            SceneData.isLevelSelection = false;
        }
    }

    public void StartGame()
    {
        mainMenu.SetActive(false);
        levelSelectionMenu.SetActive(true);
        settingsMenu.SetActive(false);
    }

    public void Settings()
    {
        mainMenu.SetActive(false);
        settingsMenu.SetActive(true);
        ActivateSettingsPanel(general); 
    }

    public void ActivateSettingsPanel(GameObject panelToShow)
    {
        general.SetActive(false);
        sound.SetActive(false);
        video.SetActive(false);
        other.SetActive(false);
        levelSelectionMenu.SetActive(false);

        if (panelToShow != null)
        {
            panelToShow.SetActive(true);
        }
    }
    public void ActivateAllPanels(){
        general.SetActive(true);
        sound.SetActive(true);
        video.SetActive(true);
        other.SetActive(true);
        levelSelectionMenu.SetActive(true);
    }

    public void ActivateFirstPanel(){
        general.SetActive(true);
        sound.SetActive(false);
        video.SetActive(false);
        other.SetActive(false);
        levelSelectionMenu.SetActive(false);
    }
    public void Credits()
    {
        SceneData.SceneToLoad = "Credits";
        SceneData.LoadScene();
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game Requested");
        Application.Quit();
    }
    public void General()
    {
        ActivateSettingsPanel(general);
    }
    
    public void Video()
    {
        ActivateSettingsPanel(video);
    }
    public void Sound()
    {
        ActivateSettingsPanel(sound);
    }
    public void Other()
    {
        ActivateSettingsPanel(other);
    }

    public void backToMainMenu()
    {
        mainMenu.SetActive(true);
        settingsMenu.SetActive(false);
        levelSelectionMenu.SetActive(false);
    }
}