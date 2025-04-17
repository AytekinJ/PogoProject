using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelect : MonoBehaviour
{
    [SerializeField] private LevelsManager levelsManager;

    void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            int index = i;
            transform.GetChild(i).GetComponent<Button>().onClick.AddListener(() => OnLevelButtonClick(index));
        }
        LoadLevelData();
    }

    private void LoadLevelData(){
        levelsManager.LoadLevelsUI(gameObject);
    }

    public void OnLevelButtonClick(int levelIndex)
    {
        if (levelIndex < 0 || levelIndex >= levelsManager.levels.Count)
        {
            Debug.LogError("Invalid level index: " + levelIndex);
            return;
        }

        levelsManager.currentLevel = levelIndex;
        Debug.Log("Loading Level Scene : " + levelsManager.levels[levelIndex].sceneNumber);
        SceneData.SceneToLoad = levelsManager.levels[levelIndex].sceneName;
        SceneManager.LoadScene("LoadingScene", LoadSceneMode.Single);
        System.GC.Collect();
        Resources.UnloadUnusedAssets();
    }
}
