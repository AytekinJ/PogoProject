using UnityEngine;

public enum PickupableType { Star, Health }

public class Pickupable : MonoBehaviour
{
    [SerializeField] public PickupableType type;
    [SerializeField] public int id;
    [SerializeField] public bool hasTaken = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasTaken || !collision.gameObject.CompareTag("Player"))
        {
            return;
        }

        if (type == PickupableType.Star)
        {
            hasTaken = true;

            if (Score.player != null) { Score.player.addStar(); } else { Debug.LogError("Score.player null!");}

            if (ScoreManager.main != null) { ScoreManager.main.RegisterStarCollected(this.id); } else { Debug.LogError("ScoreManager.main null!");}

            if(Score.player != null && Score.player.pickUpEffect != null) {
                 Destroy(Instantiate(Score.player.pickUpEffect, transform.position, Quaternion.identity), 1f);
            }
            Destroy(gameObject);
        }
        else if (type == PickupableType.Health)
        {
            if (Score.player != null) { Score.player.addHeart(); }

            if(Score.player != null && Score.player.pickUpEffect != null) {
                 Destroy(Instantiate(Score.player.pickUpEffect, transform.position, Quaternion.identity), 1f);
            }
            Destroy(gameObject);
        }
    }
}