using Unity.VisualScripting;
using UnityEngine;
public enum PickupableType
{
    Star,
    Health
}
public class Pickupable : MonoBehaviour
{
    [SerializeField] public PickupableType type;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (type == PickupableType.Star)
            {
                Score.player.addStar();
                Debug.Log("Star Added");
            }
            else if(type == PickupableType.Health)
            {
                HealthScript.IncreaseHealth(1);
            }
            Destroy(Instantiate(Score.player.pickUpEffect, transform.position, Quaternion.identity),1f);
            Destroy(gameObject);
        }
    }
}
