using Unity.VisualScripting;
using UnityEngine;
public enum PickupableType
{
    Coin,
    Star
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
            else
            {
                Score.player.addCoin();
                Debug.Log("Coin Added");
            }
            Destroy(gameObject);
        }
    }
}
