using UnityEngine;

public class Score : MonoBehaviour
{
    public static Score player { get; set; }
    public GameObject pickUpEffect;
    public int heartsCollected;
    public int starsCollected;

    public int maxStars;
    public int maxHearts;
    private void Awake()
    {
        maxStars = GameObject.FindGameObjectsWithTag("Star").Length;
        maxHearts = GameObject.FindGameObjectsWithTag("Heart").Length;
        player = this;

    }
    public void addStar()
    {
        if (starsCollected + 1 <= maxStars)
            starsCollected += 1;
    }
    public void addHeart()
    {
        if (heartsCollected+1 <= maxHearts)
            heartsCollected += 1;
    }
}
