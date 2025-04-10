using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    public float respawnDelay = 1f;
    public bool isRespawning = false;
    public bool IsPlayerInside = false;

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

    

    public IEnumerator RespawnDoor()
    {
        while (IsPlayerInside)
        {
            yield return null;
        }

        yield return new WaitForSeconds(respawnDelay);
        animator.SetBool("isBreaking", false);
        boxCollider2D.enabled = true;
        isRespawning = false;
    }
}
