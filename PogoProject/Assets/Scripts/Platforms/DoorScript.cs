using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    public float respawnDelay = 1f;
    private bool isRespawning = false;
    private bool IsPlayerInside = false;

    Animator animator;
    [SerializeField] BoxCollider2D boxCollider2D;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void DestroyDoor(GameObject player)
    {
        if (!isRespawning)
        {
            isRespawning = true;
            boxCollider2D.enabled = false;
            animator.SetBool("isBreaking", true);
            StartCoroutine(RespawnDoor());
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IsPlayerInside = true;
        StopAllCoroutines();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        IsPlayerInside = false;
        StartCoroutine(RespawnDoor());
    }

    IEnumerator RespawnDoor()
    {
        while (IsPlayerInside)
        {
            yield return null;
        }

        yield return new WaitForSeconds(respawnDelay);
        animator.SetBool("isBreaking", false);
        yield return new WaitForSeconds(1f);
        boxCollider2D.enabled = true;
        isRespawning = false;
    }
}
