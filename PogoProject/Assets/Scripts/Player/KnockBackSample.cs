using UnityEngine;
using System.Collections;

public class KnockBackSample : MonoBehaviour
{
    [SerializeField] float X;
    [SerializeField] float Y;

    public float CoolDownTime = .2f;

    bool CanKnockBack = true;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && CanKnockBack)
            HealthScript.PlayerKnockBack(X, Y, transform);
    }

    IEnumerator Cooldown()
    {
        CanKnockBack = false;
        yield return new WaitForSeconds(CoolDownTime);
        CanKnockBack = true;
    }
}
