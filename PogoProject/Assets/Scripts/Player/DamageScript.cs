using UnityEngine;

public class DamageScript : MonoBehaviour
{
    [SerializeField] int PointValue = 1;
    [SerializeField] bool IncreaseHealth = false;
    [SerializeField] HealthScript healthScript;

    void Awake()
    {
        healthScript = GameObject.FindGameObjectWithTag("Player").GetComponent<HealthScript>();
    }
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
            healthScript.IncreaseHealth(PointValue);
        }
        else if (!IncreaseHealth)
        {
            healthScript.DecreaseHealth(PointValue, gameObject.tag);
            //Debug.Log(gameObject.tag);
        }
    }
}
