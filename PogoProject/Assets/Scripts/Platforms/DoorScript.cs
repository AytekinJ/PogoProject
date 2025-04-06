using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    public float respawnDelay = 1f;
    //private bool canRespawn = true; kullanılmıyor
    Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
    }
    public void DestroyDoor(GameObject player)
    {
        // if(player.transform.position.x > transform.position.x)
        // {
        //     Vector3 localScale = transform.localScale;
        //     localScale.x *= -1;
        //     transform.localScale = localScale;
        // }
        
        GetComponent<BoxCollider2D>().enabled = false;
        animator.SetBool("isBreaking", true);
        StartCoroutine(RespawnDoor());
        
    }

    IEnumerator RespawnDoor()
    {
        yield return new WaitForSeconds(respawnDelay);
        animator.SetBool("isBreaking", false);
        yield return new WaitForSeconds(1f);
        GetComponent<BoxCollider2D>().enabled = true;
    }
}
