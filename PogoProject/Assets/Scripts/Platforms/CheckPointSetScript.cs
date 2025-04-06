using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CheckPointSetScript : MonoBehaviour
{
    [SerializeField] bool IsPlatformCheckPoint = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            DecideCheckPoint();
            GetComponent<Light2D>().enabled = true;
            GetComponent<LightFlicker>().enabled = true;
            transform.GetChild(0).GetComponent<Animator>().SetTrigger("Activate");
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
