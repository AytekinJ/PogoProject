using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class ButtonEventsForwarder : MonoBehaviour
{

    public void StartGame()
    {
        SceneManager.LoadSceneAsync(1);
    }

    public void Settings()
    {
        
    }

    public void Credits()
    {
        
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
