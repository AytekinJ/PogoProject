using UnityEngine;

public class HitParticleScript : MonoBehaviour
{
    public GameObject HitPrefab;
    public LayerMask layerXmaxPro;
    
    public Vector2 boxSize = new Vector2(1f, 1f);
    public float angle = 0f;

    public void CastParticleBox(float RayLength, Vector2 direction)
    {
        RaycastHit2D ray = Physics2D.BoxCast(transform.position, boxSize, angle, direction, RayLength, layerXmaxPro);

        if (ray.collider != null)
        {
            var particle = Instantiate(HitPrefab, ray.point, Quaternion.identity);
            Destroy(particle, 1f);
        }
    }
}