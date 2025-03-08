using UnityEngine;

public class Score : MonoBehaviour
{
    public static Score player { get; set; }
    public GameObject pickUpEffect;
    public int starsCollected;
    public int coinsCollected;

    public int maxStars;
    public int maxCoins;
    private void Awake()
    {
        player = this;
    }
    public void addCoin()
    {
        if (coinsCollected + 1 <= maxCoins)
            coinsCollected += 1;
    }
    public void addStar()
    {
        if (starsCollected+1 <= maxStars)
            starsCollected += 1;
    }
}
