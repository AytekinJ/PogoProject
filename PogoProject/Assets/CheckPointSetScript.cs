using UnityEngine;

public class CheckPointSetScript : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            HealthScript.SetCheckpoint(transform);
        }
        else
        {
            return;
        }
    }
}
