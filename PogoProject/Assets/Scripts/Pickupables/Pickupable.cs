using UnityEngine;

public enum PickupableType { Star, Health }

public class Pickupable : MonoBehaviour
{
    [SerializeField] public PickupableType type;
    [SerializeField] public int id;
    [SerializeField] public bool hasTaken = false;
    public GameObject PickupSFX;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasTaken || !collision.gameObject.CompareTag("Player"))
        {
            return;
        }

        if (type == PickupableType.Star)
        {
            PlaySFX();
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
            PlaySFX();
            if (Score.player != null) { Score.player.addHeart(); }

            if(Score.player != null && Score.player.pickUpEffect != null) {
                 Destroy(Instantiate(Score.player.pickUpEffect, transform.position, Quaternion.identity), 1f);
            }
            Destroy(gameObject);
        }
    }

    void PlaySFX()
    {
        var sfx = Instantiate(PickupSFX, transform.position, Quaternion.identity);
        AudioSource audioSource = sfx.GetComponent<AudioSource>();
        if (audioSource != null) audioSource.pitch = UnityEngine.Random.Range(0.6f, 0.8f);
        Destroy(sfx, 3f);
    }
}