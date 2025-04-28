using UnityEngine;
using UnityEngine.SceneManagement;

public class EndScreenButtons : MonoBehaviour
{

    public LevelsManager levelsManager;

    //void Awake()
    //{
    //    if (LevelsManager.instance == null)
    //    {
    //        LevelsManager.instance = Resources.Load<LevelsManager>("LevelsManager");
    //    }
    //}

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
        Debug.Log(levelsManager);
        Debug.Log(levelsManager.currentLevel);
        Debug.Log(levelsManager.currentLevel.ToString());
        string currentLevel = levelsManager.currentLevel.ToString();
        if(int.Parse(currentLevel) == 8)
        {
            return; // şuanlık 8 dan büyük level olmadığı için işlem iptal
        }
        else if (int.Parse(currentLevel) == SceneManager.sceneCountInBuildSettings - 4) //SonPlayable Shanede miyiz? toplam 9 sahne var ve oynanabilecek 6 sahne var ve 0 dan başlıyor, o yüzden max sahneden 4 çıkardım
        {
            //eğer öyleyse credits i yükle
            //LevelsManager.instance.currentLevel++;
            SceneData.SceneToLoad = "Credits";
            SceneData.LoadScene();
            Time.timeScale = 1f;
            return;
        }
        levelsManager.currentLevel++;
        SceneData.SceneToLoad = "Level "+(int.Parse(currentLevel)+2);
        SceneData.LoadScene();
        Time.timeScale=1f;
    }
}