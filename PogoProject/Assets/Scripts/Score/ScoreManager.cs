using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private bool isFinished = false;
    private bool gameOver;
    private Score playerScore;
    public static ScoreManager main;
    [Header("Timer Settings")]
    [SerializeField] private float timeScaleInterpolationSpeed = 1f;
    [SerializeField] private float time;
    [SerializeField] private string filePath;
    
    [Header("Score Settings")]
    public TextMeshProUGUI timerText;
    public int starsInLevel;
    public int coinsInLevel;
    public int totalScore;
    public bool towerPogo;
    
    
    [Header("Rewards")]
    [SerializeField] private int rewardPerCoin;
    [SerializeField] private int rewardPerStar;
    [SerializeField] private int pogoReward;
    [SerializeField] private int rewardPerRemainingSecond;

    private void Awake()
    {
        main = this;
        playerScore = GameObject.FindGameObjectWithTag("Player").GetComponent<Score>();
        timerText = GameObject.FindWithTag("TimerUI").GetComponent<TextMeshProUGUI>();
    }
    private void Start()
    {
        StartCoroutine(Timer());
    }

    public void ControlScores()
    {
        if (isFinished || gameOver)
            return;
        if (playerScore.starsCollected == starsInLevel)
        {
            totalScore += playerScore.starsCollected * rewardPerStar;
        }

        if (playerScore.coinsCollected == coinsInLevel)
        {
            totalScore += playerScore.coinsCollected * rewardPerCoin;
        }

        if (towerPogo)
        {
            totalScore += pogoReward;
        }
        
        totalScore += ((int)time-(int)Time.time) * rewardPerRemainingSecond;
            
        isFinished = true;
        Debug.Log(totalScore);
        CalculateCompletionPercentage();
        int highScore = ControlAndSaveHighScore(totalScore);
    }
    private void CalculateCompletionPercentage()
    {
        int totalCollectibles = starsInLevel + coinsInLevel;
        int collectedItems = playerScore.starsCollected + playerScore.coinsCollected;

        if (totalCollectibles == 0)
        {
            return;
        }
        if (playerScore.coinsCollected == coinsInLevel)
        {
            Debug.Log("Altın Görevi Tamamlandı !");
        }
        if (towerPogo)
        {
            Debug.Log("Kuleye pogo yapıldı");
        }
        float completionPercentage = (collectedItems / (float)totalCollectibles) * 100f;
        Debug.Log("Tamamlanma Yüzdesi: " + completionPercentage + "%");
    }

    #region Save And Encryption
    private int ControlAndSaveHighScore(int score)
    {
        int currentHighScore;
        BinaryFormatter formatter = new BinaryFormatter();

        if (!File.Exists(filePath))
        {
            currentHighScore = score;
            WriteFileAsSerializedAndEncrypted(score, formatter);
            return currentHighScore;
        }
        //say gex
        byte[] encryptedData = File.ReadAllBytes(filePath);
        byte[] decryptedData = XorEncrypt(encryptedData);
        using (MemoryStream ms = new MemoryStream(decryptedData))
        {
            currentHighScore = (int)formatter.Deserialize(ms);

        }
        //31
        if (score > currentHighScore)
        {
            WriteFileAsSerializedAndEncrypted(score, formatter);
        }
        return currentHighScore;
    }

    private void WriteFileAsSerializedAndEncrypted(int score, BinaryFormatter formatter)
    {
        using (MemoryStream ms = new MemoryStream())
        {
            formatter.Serialize(ms, score);
            byte[] rawData = ms.ToArray();
            byte[] encryptedData = XorEncrypt(rawData);
            File.WriteAllBytes(filePath, encryptedData);
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
            timerText.text = Mathf.FloorToInt(elapsedTime) + " / " + time;
            yield return null;
        }

        if (!isFinished)
        {
            float velocity = 0f;
            float targetTimeScale = 0f;

            while (Time.timeScale > 0.05f)
            {
                Time.timeScale = Mathf.SmoothDamp(Time.timeScale, targetTimeScale, ref velocity, timeScaleInterpolationSpeed);
                yield return null;
            }

            Time.timeScale = 0;
            playerScore.gameObject.GetComponent<Controller>().enabled = false;
            playerScore.gameObject.GetComponent<AttackScript>().enabled = false;
            Debug.Log("Zaman tükendiği için oyun kaybedildi.");
        }
    }

}
