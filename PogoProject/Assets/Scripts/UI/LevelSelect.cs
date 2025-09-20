using UnityEngine;
using UnityEngine.UI;

public class LevelSelect : MonoBehaviour
{
    [SerializeField] private LevelsManager levelsManager;

    void Start()
    {
        if (levelsManager == null)
        {
            Debug.LogError("LevelSelect: LevelsManager referansı atanmamış!", this);
            return;
        }

     
        for (int i = 0; i < transform.childCount; i++)
        {
      
            Button button = transform.GetChild(i).GetComponent<Button>();
            if (button != null)
            {
                int index = i;
                button.onClick.AddListener(() => OnLevelButtonClick(index));
            }
        }
        LoadLevelData();
    }

    private void LoadLevelData()
    {
        if (levelsManager != null)
        {
            levelsManager.LoadLevelsUI(gameObject);
        }
    }

    public void OnLevelButtonClick(int levelIndex)
    {
        if (levelsManager == null) return;

        if (levelIndex < 0 || levelIndex >= levelsManager.levels.Count)
        {
            Debug.LogError("Invalid level index: " + levelIndex);
            return;
        }

        LevelData selectedLevel = levelsManager.levels[levelIndex];
        if (selectedLevel == null)
        {
            Debug.LogError($"LevelData {levelIndex} null!");
            return;
        }

      
        string sceneNameToLoad = selectedLevel.sceneName;
        if (string.IsNullOrEmpty(sceneNameToLoad))
        {
            Debug.LogError($"LevelData '{selectedLevel.name}' için sceneName boş veya atanmamış!");
            return;
        }

        levelsManager.currentLevel = levelIndex;
        Debug.Log("Loading Level Scene (via SceneLoader): " + sceneNameToLoad);


        SceneData.SceneToLoad = sceneNameToLoad;
        SceneData.LoadScene();

    }
}
