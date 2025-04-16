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
        for (int i = 0; i < parentUI.transform.childCount; i++)
        {
            Transform child = parentUI.transform.GetChild(i);
            Image levelImage = child.transform.GetChild(1).GetComponent<Image>();
            TextMeshProUGUI levelInfo = child.GetComponentInChildren<TextMeshProUGUI>();
            levelImage.sprite = levels[i].levelIcon;
            levelInfo.text = "Level " + (i + 1) + "\n" +
            "Best Time: " + levels[i].bestTime.ToString("F2") + "s\n" +
            "Completion: " + levels[i].completionPercentage.ToString("F0") + "%\n" +
            (levels[i].isCompleted ? "Completed" : "Not Completed");
        }
    }
}
