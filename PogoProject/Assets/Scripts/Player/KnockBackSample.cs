using UnityEngine;

public class KnockBackSample : MonoBehaviour
{
    [SerializeField] float X;
    [SerializeField] float Y;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            HealthScript.Instance.PlayerKnockBack(X, Y, transform);
    }
}
