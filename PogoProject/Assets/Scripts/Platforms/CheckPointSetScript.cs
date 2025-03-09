using UnityEngine;

public class CheckPointSetScript : MonoBehaviour
{
    [SerializeField] bool IsPlatformCheckPoint = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            DecideCheckPoint();
        }
        else
        {
            return;
        }
    }

    void DecideCheckPoint()
    {
        if (IsPlatformCheckPoint)
        {
            HealthScript.SetPlatformCheckpoint(gameObject.transform);
            Debug.Log(gameObject.name);
        }
        else if (!IsPlatformCheckPoint)
        {
            HealthScript.SetCheckpoint(gameObject.transform);
            Debug.Log(gameObject.name);
        }
    }
}
