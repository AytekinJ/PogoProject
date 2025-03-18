using UnityEngine;

public class HitParticleScript : MonoBehaviour
{
    public GameObject HitPrefab;
    public LayerMask layerXmaxPro;

    public void CastParticleRay(float RayLength, Vector2 direction)
    {
        RaycastHit2D ray = Physics2D.Raycast(transform.position, direction, RayLength, layerXmaxPro);

        if (ray.collider != null && ray.collider.tag != null)
        {
            var particle = Instantiate(HitPrefab, ray.point, Quaternion.identity);
            Destroy(particle, 1f);
        }
    }
}
