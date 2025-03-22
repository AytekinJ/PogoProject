using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class ButtonEventsForwarder : MonoBehaviour
{
    public UIDocument mainmenu;
    void Start()
    {
        var root = mainmenu.rootVisualElement;
        
        var startButton = root.Q<Button>("startGame");
        var quitButton = root.Q<Button>("quitButton");
        var settingsButton = root.Q<Button>("settingsButton");
        var creditsButton = root.Q<Button>("creditsButton");
        
        startButton.RegisterCallback<ClickEvent>(evt => StartGame());
        quitButton.RegisterCallback<ClickEvent>(evt => QuitGame());
        settingsButton.RegisterCallback<ClickEvent>(evt => Settings());
        creditsButton.RegisterCallback<ClickEvent>(evt => Credits());
    }

    void StartGame()
    {
        SceneManager.LoadSceneAsync(1);
    }

    void Settings()
    {
        
    }

    void Credits()
    {
        
    }

    void QuitGame()
    {
        Application.Quit();
    }
}
