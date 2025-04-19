using UnityEngine;

public class EndScreenButtons : MonoBehaviour
{
    public void RestartGame()
    {
        Time.timeScale = 1f; 
        SceneLoader.ReloadCurrentScene();
    }

    public void LevelSelection()
    {
        Time.timeScale = 1f;
        SceneData.SceneToLoad = "MainMenu";
        SceneData.LoadScene(); 

    }
}