using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager main;
    public TextMeshProUGUI timerText;
    public Score playerScore;
    public int starsInLevel;
    public int coinsInLevel;
    public int totalScore;
    public bool towerPogo;
    public bool isFinished = false;
    public bool gameOver;
    public float time;
    public float timeScaleInterpolationSpeed = 1f;

    private void Awake()
    {
        main = this;
        playerScore = GameObject.FindGameObjectWithTag("Player").GetComponent<Score>();
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
            totalScore++;
        }

        if (playerScore.coinsCollected == coinsInLevel)
        {
            totalScore++;
        }

        if (towerPogo)
        {
            totalScore++;
        }

        isFinished = true;
        Debug.Log(totalScore);
        CalculateCompletionPercentage();
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
