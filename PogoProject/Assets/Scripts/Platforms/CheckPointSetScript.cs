using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class CheckPointSetScript : MonoBehaviour
{
    [SerializeField] bool IsPlatformCheckPoint = false;
    public GameObject CheckPointSFX;
    public GameObject CheckPointIdleSFX;

    bool isActivated;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            DecideCheckPoint();
            GetComponent<Light2D>().enabled = true;
            GetComponent<LightFlicker>().enabled = true;
            transform.GetChild(0).GetComponent<Animator>().SetTrigger("Activate");
            PlaySFX();
        }
        else
        {
            return;
        }
    }

    void PlaySFX()
    {
        if (isActivated) return;
        isActivated = true;
        var sfx = Instantiate(CheckPointSFX, transform.position, Quaternion.identity);
        AudioSource audioSource = sfx.GetComponent<AudioSource>();
        if (audioSource != null) audioSource.pitch = Random.Range(0.9f, 1.1f);
        //StartCoroutine(PlayIdleSFX());
        Destroy(sfx, 3f);
    }

    //IEnumerator PlayIdleSFX()
    //{
    //    yield return new WaitForSeconds(2f);
    //    Instantiate(CheckPointIdleSFX, transform.position, Quaternion.identity);
    //}

    void DecideCheckPoint()
    {
        if (IsPlatformCheckPoint)
        {
            HealthScript.Instance.SetPlatformCheckpoint(gameObject.transform);
            Debug.Log(gameObject.name);
        }
        else if (!IsPlatformCheckPoint)
        {
            HealthScript.Instance.SetCheckpoint(gameObject.transform);
            Debug.Log(gameObject.name);
        }
    }
    
}
