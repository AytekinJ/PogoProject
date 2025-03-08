using UnityEngine;

public class DamageScript : MonoBehaviour
{
    [SerializeField] int DamageToGive = 1;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            HealthScript.DecreaseHealth(DamageToGive);
        }
        else
        {
            return;
        }
    }
}
