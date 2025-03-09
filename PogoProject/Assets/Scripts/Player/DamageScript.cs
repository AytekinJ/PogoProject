using UnityEngine;

public class DamageScript : MonoBehaviour
{
    [SerializeField] int PointValue = 1;
    [SerializeField] bool IncreaseHealth = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            DecideDamage();
        }
        else
        {
            return;
        }
    }

    void DecideDamage()
    {
        if (IncreaseHealth)
        {
            HealthScript.IncreaseHealth(PointValue);
        }
        else if (!IncreaseHealth)
        {
            HealthScript.DecreaseHealth(PointValue, gameObject.tag);
            Debug.Log(gameObject.tag);
        }
    }
}
