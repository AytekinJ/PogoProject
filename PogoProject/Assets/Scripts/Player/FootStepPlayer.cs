using UnityEngine;

public class FootStepPlayer : MonoBehaviour
{
    public GameObject[] FootstepSnow;
    public GameObject[] FootStepStone;
    public LayerMask GroundLayer;
    public float Distance = 2f;

    public void CheckSFXGround()
    {
        RaycastHit2D ray;
        ray = Physics2D.Raycast(transform.position, Vector2.down, Distance, GroundLayer);

        if (ray.collider == null)
            return;

        if (ray.collider.transform.CompareTag("GroundStone"))
            PlayStoneSFX();
        else if (ray.collider.transform.CompareTag("GroundSnow"))
            PlaySnowSFX();
    }

    public void PlayStoneSFX()
    {
        if (FootStepStone.Length == 0) return;
        int randomIndex = Random.Range(0, FootStepStone.Length);
        var sfx = Instantiate(FootStepStone[randomIndex], transform.position, Quaternion.identity, gameObject.transform);
        sfx.GetComponent<AudioSource>().pitch = Random.Range(0.85f, 1.2f);
        Destroy(sfx, 5f);
    }

    public void PlaySnowSFX()
    {
        if (FootstepSnow.Length == 0) return;
        int randomIndex = Random.Range(0, FootstepSnow.Length);
        var sfx = Instantiate(FootstepSnow[randomIndex], transform.position, Quaternion.identity, gameObject.transform);
        sfx.GetComponent<AudioSource>().pitch = Random.Range(0.85f, 1.2f);
        Destroy(sfx, 5f);
    }

    public void PlayGroundHitSFX()
    {
        var sfx = Instantiate(FootStepStone[0], transform.position, Quaternion.identity, gameObject.transform);
        sfx.GetComponent<AudioSource>().pitch = Random.Range(0.6f, 0.8f);
    }
}