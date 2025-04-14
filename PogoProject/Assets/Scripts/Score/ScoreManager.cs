using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Mono.Cecil.Cil;
using TMPro;
using Unity.Burst;
using Unity.Collections;
using UnityEngine.UI;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private bool isFinished = false;
    private bool gameOver;
    public static ScoreManager main;

    private float scorePercentage;
    
    [Header("Timer Settings")]
    [SerializeField] private float timeScaleInterpolationSpeed = 1f;
    [SerializeField] private float time;
    [SerializeField] private string filePath;
    
    [Header("Score Settings")]
    public TextMeshProUGUI timerText;
    public int starsInLevel;
    public int heartsInLevel;
    public int totalScore;
    public bool towerPogo;
    
    [Header("Rewards")]
    [SerializeField] private int rewardPerCoin;
    [SerializeField] private int rewardPerStar;
    [SerializeField] private int pogoReward;
    [SerializeField] private int rewardPerRemainingSecond;

    [Space(5f)]
    [Header("Endgame Screen Settings")]
    [SerializeField] private GameObject endGameScreenPrefab;
    [SerializeField] private GameObject starsParent;
    [SerializeField] private GameObject starPrefab;
    [SerializeField] private TextMeshProUGUI percentageText;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI recordTimeText;
    [SerializeField] private TextMeshProUGUI pogoText;

    private void Awake()
    {
        main = this;
        
        // Find timer UI text
        GameObject timerObject = GameObject.FindWithTag("TimerUI");
        if (timerObject != null) {
            timerText = timerObject.GetComponent<TextMeshProUGUI>();
        } else {
            Debug.LogError("Timer UI object with tag 'TimerUI' not found!");
        }
        
        // Count stars in level
        GameObject[] stars = GameObject.FindGameObjectsWithTag("Star");
        starsInLevel = stars.Length;
        Debug.Log($"Found {starsInLevel} stars in level");
        
        // Count hearts in level
        GameObject[] hearts = GameObject.FindGameObjectsWithTag("Heart");
        heartsInLevel = hearts.Length;
        Debug.Log($"Found {heartsInLevel} hearts in level");
    }
    
    private void Start()
    {
        StartCoroutine(Timer());
    }

    public void ControlScores()
    {
        if (isFinished || gameOver)
            return;
            
        
        // Calculate score for collected stars (rewardPerCoin for each star)
        totalScore += Score.player.starsCollected * rewardPerCoin;
        
        // Add bonus for tower pogo if achieved
        if (towerPogo)
        {
            totalScore += pogoReward;
        }
        
        // Add bonus for remaining time
        int remainingSeconds = (int)time - (int)Time.time;
        if (remainingSeconds > 0) {
            totalScore += remainingSeconds * rewardPerRemainingSecond;
        }
            
        isFinished = true;
        Debug.Log($"Final Score: {totalScore}");
        
        // Calculate completion percentage
        CalculateCompletionPercentage();
        
        // Save high score
        int highScore = ControlAndSaveHighScore(totalScore);
        Debug.Log($"High Score: {highScore}");
    }
    
    private void CalculateCompletionPercentage()
    {
        int totalCollectibles = starsInLevel;
        int collectedItems = Score.player.starsCollected;

        // Check for division by zero
        if (totalCollectibles == 0)
        {
            Debug.Log("No collectibles in level!");
            return;
        }
        
        // Check if all stars collected
        if (Score.player.starsCollected == starsInLevel && starsInLevel > 0)
        {
            Debug.Log("All stars collected! Star goal complete!");
        }
    
        
        // Check tower pogo
        if (towerPogo)
        {
            Debug.Log("Tower pogo completed!");
        }
        
        // Calculate completion percentage
        scorePercentage = ((float)collectedItems / totalCollectibles) * 100f;
        Debug.Log($"Collected {collectedItems}/{totalCollectibles} items");
        Debug.Log($"Completion Percentage: {scorePercentage:F2}%");

        EndGame();
    }

    #region Save And Encryption
    private int ControlAndSaveHighScore(int score)
    {
        // Make sure filepath is set
        if (string.IsNullOrEmpty(filePath)) {
            filePath = Application.persistentDataPath + "/highscore.dat";
            Debug.Log($"Setting default filepath to: {filePath}");
        }
    
        int currentHighScore;
        BinaryFormatter formatter = new BinaryFormatter();

        if (!File.Exists(filePath))
        {
            currentHighScore = score;
            WriteFileAsSerializedAndEncrypted(score, formatter);
            return currentHighScore;
        }
        
        try {
            byte[] encryptedData = File.ReadAllBytes(filePath);
            byte[] decryptedData = XorEncrypt(encryptedData);
            using (MemoryStream ms = new MemoryStream(decryptedData))
            {
                currentHighScore = (int)formatter.Deserialize(ms);
            }
            
            if (score > currentHighScore)
            {
                WriteFileAsSerializedAndEncrypted(score, formatter);
                return score; // Return the new high score
            }
            
            return currentHighScore;
        }
        catch (Exception e) {
            Debug.LogError($"Error reading high score: {e.Message}");
            // If file is corrupted, create a new one
            WriteFileAsSerializedAndEncrypted(score, formatter);
            return score;
        }
    }

    private void WriteFileAsSerializedAndEncrypted(int score, BinaryFormatter formatter)
    {
        try {
            using (MemoryStream ms = new MemoryStream())
            {
                formatter.Serialize(ms, score);
                byte[] rawData = ms.ToArray();
                byte[] encryptedData = XorEncrypt(rawData);
                File.WriteAllBytes(filePath, encryptedData);
                Debug.Log($"High score saved: {score}");
            }
        }
        catch (Exception e) {
            Debug.LogError($"Error saving high score: {e.Message}");
        }
    }

    private byte[] XorEncrypt(byte[] data)
    {
        byte xorKey = 0xBA;
        byte[] result = new byte[data.Length];
        for (int i = 0; i < result.Length; i++)
        {
            result[i] = (byte)(data[i] ^ xorKey);
        }
        return result;
    }
    #endregion
    
    private IEnumerator Timer()
    {
        float startTime = Time.time;
        float elapsedTime = 0f;

        while (elapsedTime < time && !isFinished)
        {
            elapsedTime = Time.time - startTime;
            
            if (timerText != null) {
                // Format remaining time in a more intuitive way
                float remainingTime = time - elapsedTime;
                timerText.text = Mathf.CeilToInt(remainingTime).ToString();
            }
            
            yield return null;
        }

        if (!isFinished)
        {
            StartCoroutine(SlowMoEnd());
        }
    }

    private IEnumerator SlowMoEnd()
    {
        gameOver = true;
        float velocity = 0f;
        float targetTimeScale = 0f;

        while (Time.timeScale > 0.05f)
        {
            Time.timeScale = Mathf.SmoothDamp(Time.timeScale, targetTimeScale, ref velocity, timeScaleInterpolationSpeed);
            yield return null;
        }

        Time.timeScale = 0;

        // Disable player controls on game over
        if (Score.player != null && Score.player.gameObject != null)
        {
            Controller controller = Score.player.gameObject.GetComponent<Controller>();
            if (controller != null) controller.enabled = false;

            AttackScript attackScript = Score.player.gameObject.GetComponent<AttackScript>();
            if (attackScript != null) attackScript.enabled = false;
        }

        Debug.Log("Time's up! Game over.");
    }

    [BurstCompile]
    public struct ScoreData
    {
        public int totalScore;
        public int starsInLevel;
        public bool towerPogo;
        public float scorePercentage;
    }
    public ScoreData GetScoreData()
    {
        return new ScoreData {
                totalScore = totalScore,
                starsInLevel = starsInLevel,
                towerPogo = towerPogo ,
                scorePercentage = scorePercentage
             };
    }


    public void EndGame()
{
    StartCoroutine(SlowMoEnd());
    gameOver = true;
    ControlScores();

    if (Score.player != null && Score.player.gameObject != null)
    {
        Controller controller = Score.player.gameObject.GetComponent<Controller>();
        if (controller != null) controller.enabled = false;

        AttackScript attackScript = Score.player.gameObject.GetComponent<AttackScript>();
        if (attackScript != null) attackScript.enabled = false;
    }

    endGameScreenPrefab.SetActive(true);
    percentageText.text = $"{scorePercentage:F2}%";
    timeText.text = $"finished in {time - int.Parse(timerText.text)} seconds";
    pogoText.text = towerPogo ? "Finished With POGO!" : "No POGO!";

    for (int i = 0; i < starsInLevel; i++)
    {
        GameObject star = Instantiate(starPrefab, starsParent.transform);
        if (i < Score.player.starsCollected)
        {
            Image starImage = star.GetComponent<Image>();
            Color c = starImage.color;
            c.a = 1f;  // Direkt alpha'yı ayarlıyoruz
            starImage.color = c;
        }
    }
    float finishedTime = time - int.Parse(timerText.text);
    if (PlayerPrefs.HasKey("FinishedTime"))
    {
        finishedTime = PlayerPrefs.GetFloat("FinishedTime", 0f);
    }
    else
    {
        float fastest = PlayerPrefs.GetFloat("FastestTime", float.MaxValue);
        if (finishedTime < fastest)
        {
        PlayerPrefs.SetFloat("FastestTime", finishedTime);
        PlayerPrefs.Save();
        recordTimeText.text = $"NEW RECORD! : {finishedTime} seconds";
        }
        else
        {
            recordTimeText.text = $"Record : {fastest} seconds";
        }
    }
    Debug.Log("Game Over triggered manually.");
}
}