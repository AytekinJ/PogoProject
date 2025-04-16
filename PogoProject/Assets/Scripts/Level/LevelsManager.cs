using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
[CreateAssetMenu(fileName = "LevelsManager", menuName = "Scriptable Objects/LevelsManager")]
public class LevelsManager : ScriptableObject
{
    [SerializeField] public List<LevelData> levels = new List<LevelData>();
    [SerializeField] public int currentLevel = 0;

    public void LoadLevelsUI(GameObject parentUI){
        foreach (Transform child in parentUI.transform)
        {
            Image levelImage = child.GetComponent<Image>();
            TextMeshProUGUI levelInfo = child.GetComponentInChildren<TextMeshProUGUI>();
            levelImage.sprite = levels[currentLevel].levelIcon;
            levelInfo.text = "Level " + (currentLevel + 1) + "\n" +
                "Best Time: " + levels[currentLevel].bestTime.ToString("F2") + "s\n" +
                "Completion: " + levels[currentLevel].completionPercentage.ToString("F0") + "%\n" +
                (levels[currentLevel].isCompleted ? "Completed" : "Not Completed");
        }
    }
}
