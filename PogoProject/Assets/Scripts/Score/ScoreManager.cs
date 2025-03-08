using System;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager main;
    public Score playerScore;
    public int starsInLevel;
    public int coinsInLevel;
    public int totalScore;
    public bool towerPogo;
    public bool isFinished = false;

    private void Awake()
    {
        main = this;
        playerScore = GameObject.FindGameObjectWithTag("Player").GetComponent<Score>();
    }

    public void ControlScores()
    {
        if (isFinished)
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

        float completionPercentage = (collectedItems / (float)totalCollectibles) * 100f;
        Debug.Log("Tamamlanma Yüzdesi: " + completionPercentage + "%");
    }
}
