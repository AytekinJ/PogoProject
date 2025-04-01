using UnityEngine;

public class FootStepPlayer : MonoBehaviour
{
    public GameObject FootstepSnow;
    public GameObject FootStepStone;

    public void PlaySnowSFX()
    {
        var sfx = Instantiate(FootstepSnow, transform.position, Quaternion.identity, gameObject.transform);
        sfx.GetComponent<AudioSource>().pitch = Random.Range(0.85f, 1.2f);
        Destroy(sfx, 5f);
    }

}