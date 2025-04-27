using UnityEngine;
using UnityEngine.SceneManagement;

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
        SceneData.isLevelSelection = true;
        SceneManager.LoadSceneAsync(SceneData.SceneToLoad, LoadSceneMode.Single);
    }
    public void NextLevel()
    {
        string currentLevel = LevelsManager.instance.currentLevel.ToString();
        if(int.Parse(currentLevel) == 7)
        {
            return; // şuanlık 7 dan büyük level olmadığı için işlem iptal
        }
        LevelsManager.instance.currentLevel++;
        SceneData.SceneToLoad = "Level "+(int.Parse(currentLevel)+2);
        SceneData.LoadScene();
        Time.timeScale=1f;
    }
}