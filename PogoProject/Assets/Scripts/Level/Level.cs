using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Scriptable Objects/LevelData")]
[System.Serializable]
public class LevelData : ScriptableObject
{
    [SerializeField] public string sceneName;
    [SerializeField] public int sceneNumber;
    [SerializeField] public Sprite levelIcon;
    [SerializeField] public List<Pickupable> takenPickups = new List<Pickupable>();
    [SerializeField] public List<Pickupable> allPickups = new List<Pickupable>();
    [SerializeField] public float bestTime = Mathf.Infinity;
    [SerializeField] public float completionPercentage = 0f;
    [SerializeField] public bool isCompleted = false;
}


[System.Serializable]
public class LevelProgressData
{
    public string levelIdentifier;
    public float bestTime = float.PositiveInfinity;
    public float completionPercentage = 0f;
    public bool isCompleted = false;
    public List<int> takenStarIDs = new List<int>();

    public LevelProgressData(string identifier)
    {
        levelIdentifier = identifier;
        bestTime = float.PositiveInfinity;
        completionPercentage = 0f;
        isCompleted = false;
        takenStarIDs = new List<int>();
    }

    public LevelProgressData() {}
}
