using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

public class ScoreManager : MonoBehaviour
{
    private bool isFinished = false;
    private bool gameOver;
    public static ScoreManager main;

    [SerializeField] private LevelsManager levelManager;

    private LevelData levelData;

    private float scorePercentage;

    [Header("Timer Settings")]
    [SerializeField] private float timeScaleInterpolationSpeed = 1f;
    [SerializeField] private float time;

    [Header("Score Settings")]
    public TextMeshProUGUI timerText;
    public int starsInLevel;
    public int heartsInLevel;
    public int totalScore;
    public bool towerPogo;

    [Header("Rewards")]
    [SerializeField] private int rewardPerCoin;
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

    private Dictionary<int, Pickupable> starLookup = new Dictionary<int, Pickupable>();
    private List<int> collectedStarIDsThisRun = new List<int>();
    private string highScoreFilePath;

    private void Start()
    {
        main = this;
        
        timerText = GameObject.FindGameObjectWithTag("TimerUI").GetComponent<TextMeshProUGUI>();

        if (levelManager == null) { Debug.LogError("LevelsManager atanmamış!"); return; }
        if (DatabaseManager.main == null) { Debug.LogError("DatabaseManager sahnede bulunamadı veya aktif değil!"); return; }

        if (levelManager.currentLevel < 0 || levelManager.currentLevel >= levelManager.levels.Count) {
             Debug.LogError($"Geçersiz currentLevel index: {levelManager.currentLevel}"); return;
        }
        levelData = levelManager.levels[levelManager.currentLevel];
        if (levelData == null) { Debug.LogError("Mevcut level için LevelData alınamadı!"); return; }

        levelData.takenPickups.Clear();
        levelData.allPickups.Clear();
        starLookup.Clear();
        collectedStarIDsThisRun.Clear();
        gameOver = false;
        isFinished = false;
        totalScore = 0;

        LevelProgressData loadedProgress = DatabaseManager.main.GetLevelProgress(levelData.name);

        if (loadedProgress != null) {
            levelData.bestTime = loadedProgress.bestTime;
            levelData.completionPercentage = loadedProgress.completionPercentage;
            levelData.isCompleted = loadedProgress.isCompleted;

        } else {
             levelData.bestTime = float.PositiveInfinity;
             levelData.completionPercentage = 0f;
             levelData.isCompleted = false;

        }

        GameObject[] starObjects = GameObject.FindGameObjectsWithTag("Star");
        starsInLevel = starObjects.Length;

        for (int i = 0; i < starsInLevel; i++)
        {
            GameObject starGO = starObjects[i];
            Pickupable pickupable = starGO.GetComponent<Pickupable>();
            if (pickupable != null && pickupable.type == PickupableType.Star)
            {
                pickupable.id = i;
                pickupable.hasTaken = false;
                starLookup[i] = pickupable;
                levelData.allPickups.Add(pickupable);
            }
            else if (pickupable == null) {
                Debug.LogWarning($"'Star' etiketli nesne (index {i}) Pickupable içermiyor.");
            }
        }

        int initialStarCount = 0;
        if (loadedProgress != null && loadedProgress.takenStarIDs != null)
        {

            foreach (int starID in loadedProgress.takenStarIDs)
            {
                if (starLookup.TryGetValue(starID, out Pickupable star))
                {
                    star.hasTaken = true;
                    levelData.takenPickups.Add(star);
                    star.gameObject.SetActive(false);
                    initialStarCount++;
                }
                else {
                    Debug.LogWarning($"Yüklenen yıldız ID ({starID}) ile eşleşen yıldız bulunamadı.");
                }
            }
        }

        if (Score.player != null) {
             Score.player.ResetStars();
        } else {
            Debug.LogError("Awake içinde Score.player referansı null!");
        }

        GameObject timerObject = GameObject.FindWithTag("TimerUI");
        if (timerObject != null) {
            timerText = timerObject.GetComponent<TextMeshProUGUI>();
        } else {
            Debug.LogError("Etiketi 'TimerUI' olan timer UI nesnesi bulunamadı!");
        }

        GameObject[] hearts = GameObject.FindGameObjectsWithTag("Heart");
        heartsInLevel = hearts.Length;


        highScoreFilePath = Path.Combine(Application.persistentDataPath, levelData.name + "_highscore.dat");
        if (time > 0)
        {
             StartCoroutine(Timer());
        } else {
            Debug.LogWarning("Level süresi (time) ayarlanmamış, zamanlayıcı başlamıyor.");
        }
    }

    public void RegisterStarCollected(int id)
    {
        if (!collectedStarIDsThisRun.Contains(id))
        {
            collectedStarIDsThisRun.Add(id);

        }
    }

    public void EndGame()
    {
        if (gameOver) return;

        gameOver = true;
        isFinished = true;
        StopCoroutine(nameof(Timer)); 

        StartCoroutine(SlowMoEnd());

        if (Score.player != null && Score.player.gameObject != null)
        {
            Controller controller = Score.player.gameObject.GetComponent<Controller>();
            if (controller != null) controller.enabled = false;
            AttackScript attackScript = Score.player.gameObject.GetComponent<AttackScript>();
            if (attackScript != null) attackScript.enabled = false;
        }

        ControlScores();

        LevelProgressData progressToSave = DatabaseManager.main.GetLevelProgress(levelData.name);
        if (progressToSave == null) {
             progressToSave = new LevelProgressData(levelData.name);
        }

        float finishedTime = 0;
        if (timerText != null && float.TryParse(timerText.text, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out float remainingTimeValue))
        {
            finishedTime = time - remainingTimeValue;
            if (finishedTime < 0) finishedTime = 0;
        } else {
            finishedTime = float.PositiveInfinity;
            Debug.LogWarning("Bitirme süresi hesaplanamadı.");
        }

        if (finishedTime > 0 && finishedTime < progressToSave.bestTime)
        {
            progressToSave.bestTime = finishedTime;
            recordTimeText.text = $"YENİ REKOR! : {progressToSave.bestTime:F2} saniye";
        }
        else if (progressToSave.bestTime != float.PositiveInfinity)
        {
            recordTimeText.text = $"Rekor : {progressToSave.bestTime:F2} saniye";
        } else {
            recordTimeText.text = "Rekor Yok";
        }
        levelData.bestTime = progressToSave.bestTime;

        HashSet<int> allTakenStarIDs = new HashSet<int>(progressToSave.takenStarIDs);
        allTakenStarIDs.UnionWith(collectedStarIDsThisRun);
        progressToSave.takenStarIDs = allTakenStarIDs.ToList();

        levelData.takenPickups.Clear();
        foreach(int starID in progressToSave.takenStarIDs) {
            if (starLookup.TryGetValue(starID, out Pickupable star)) {
                levelData.takenPickups.Add(star);
            }
        }

        CalculateCompletionPercentage();
        progressToSave.completionPercentage = scorePercentage;
        progressToSave.isCompleted = true;

        levelData.completionPercentage = scorePercentage;
        levelData.isCompleted = true;

        DatabaseManager.main.SaveLevelProgress(progressToSave);

        ControlAndSaveHighScore(totalScore);

        endGameScreenPrefab.SetActive(true);
        percentageText.text = $"{scorePercentage:F2}%";
        if (finishedTime != float.PositiveInfinity) {
             timeText.text = $"Bitirme Süresi: {finishedTime:F2} saniye";
        } else {
             timeText.text = "Süre Hesaplanamadı";
        }
        pogoText.text = towerPogo ? "POGO İLE BİTTİ!" : "POGO YOK!";

        foreach (Transform child in starsParent.transform) { Destroy(child.gameObject); }
        int totalStarsCollectedEver = progressToSave.takenStarIDs.Count;
        for (int i = 0; i < starsInLevel; i++)
        {
            GameObject starUI = Instantiate(starPrefab, starsParent.transform);
            if (i < totalStarsCollectedEver)
            {
                Image starImage = starUI.GetComponent<Image>();
                if (starImage != null) {
                    Color c = starImage.color; c.a = 1f; starImage.color = c;
                }
            }
        }

    }

    public void ControlScores()
    {
        totalScore = 0;

        if (Score.player != null) {
            totalScore += Score.player.starsCollected * rewardPerCoin;
        }

        if (towerPogo) {
            totalScore += pogoReward;
        }

        if (timerText != null && int.TryParse(timerText.text, out int remainingSeconds) && remainingSeconds > 0) {
            totalScore += remainingSeconds * rewardPerRemainingSecond;
        }

    }

    private void CalculateCompletionPercentage()
    {
        if (starsInLevel == 0) {
            scorePercentage = 100f;
            return;
        }

        int totalCollectedEver = 0;
        LevelProgressData currentProgress = DatabaseManager.main.GetLevelProgress(levelData.name);
        HashSet<int> allTakenIDs = new HashSet<int>(currentProgress?.takenStarIDs ?? new List<int>());
        allTakenIDs.UnionWith(collectedStarIDsThisRun);
        totalCollectedEver = allTakenIDs.Count;

        scorePercentage = ((float)totalCollectedEver / starsInLevel) * 100f;


    }

    private int ControlAndSaveHighScore(int score)
    {
        highScoreFilePath = Path.Combine(Application.persistentDataPath, levelData.name + "_highscore.dat");

        int currentHighScore = 0;
        BinaryFormatter formatter = new BinaryFormatter();

        if (File.Exists(highScoreFilePath))
        {
            try {
                byte[] encryptedData = File.ReadAllBytes(highScoreFilePath);
                byte[] decryptedData = XorEncryptHighScore(encryptedData);
                using (MemoryStream ms = new MemoryStream(decryptedData)) {
                    currentHighScore = (int)formatter.Deserialize(ms);
                }
            } catch (Exception e) {
                Debug.LogError($"High score okuma hatası ({levelData.name}): {e.Message} - Dosya sıfırlanıyor.");
                currentHighScore = 0;
            }
        }

        if (score > currentHighScore)
        {
            WriteFileAsSerializedAndEncryptedHighScore(score, formatter);
            return score;
        }

        return currentHighScore;
    }

    private void WriteFileAsSerializedAndEncryptedHighScore(int score, BinaryFormatter formatter)
    {
        highScoreFilePath = Path.Combine(Application.persistentDataPath, levelData.name + "_highscore.dat");
        try {
            using (MemoryStream ms = new MemoryStream()) {
                formatter.Serialize(ms, score);
                byte[] rawData = ms.ToArray();
                byte[] encryptedData = XorEncryptHighScore(rawData);
                File.WriteAllBytes(highScoreFilePath, encryptedData);
      
            }
        } catch (Exception e) {
            Debug.LogError($"High score kaydetme hatası ({levelData.name}): {e.Message}");
        }
    }

    private byte[] XorEncryptHighScore(byte[] data)
    {
        byte xorKey = 0xBA;
        byte[] result = new byte[data.Length];
        for (int i = 0; i < result.Length; i++) {
            result[i] = (byte)(data[i] ^ xorKey);
        }
        return result;
    }

    private IEnumerator Timer()
    {
        float startTime = Time.time;
        float elapsedTime = 0f;

        while (!isFinished)
        {
            elapsedTime = Time.time - startTime;
            float remainingTime = time - elapsedTime;

            if (remainingTime <= 0) {
                remainingTime = 0;
                if (!gameOver) {
                   
                    EndGame();
                }
            }

            if (timerText != null) {
                timerText.text = Mathf.CeilToInt(remainingTime).ToString();
            }

            yield return null;
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
    public Pickupable FindStarWithID(int id)
    {
        if (starLookup.TryGetValue(id, out Pickupable star))
        {
            return star;
        }
        return null;
    }


    // başlangıçta eğer bu şekilde bir temizleme yapılmazsa, singleton objeler bellekte kalıyor ve oyun kapatılmadığı sürece
    // değişmiyorlar. Önceki verilerle birlikte üst üste bindikleri için oyunun çoğu mekaniği (singleton ile çalışanlar)
    // bozuluyor. Bu yüzden sahne yüklendiğinde, sahne değiştiğinde veya oyun sıfırlandığında bu temizleme yapılmalı.
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) {
            SceneLoader.ReloadCurrentScene_ClearAllocatedMemory();
        }
    }
}