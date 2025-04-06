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

    public void StartGame()
    {
        SceneManager.LoadSceneAsync(1);
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

        if (panelToShow != null)
        {
            panelToShow.SetActive(true);
        }
    }

    public void Credits()
    {
        Debug.Log("Credits Button Pressed");
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
    }
}