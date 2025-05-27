using UnityEngine;

public class HitParticleScript : MonoBehaviour
{
    public static HitParticleScript Instance;
    public GameObject HitPrefab;
    public LayerMask layerXmaxPro;

    void Start()
    {
        Instance = this;
    }

    public void CastParticleBox(float RayLength, Vector2 direction, Vector2 boxSize)
    {
        RaycastHit2D ray = Physics2D.BoxCast(transform.position, boxSize, 0f, direction, RayLength, layerXmaxPro);

        if (ray.collider != null)
        {
            var particle = Instantiate(HitPrefab, ray.point, Quaternion.identity);
            Destroy(particle, 1f);
        }
    }
}