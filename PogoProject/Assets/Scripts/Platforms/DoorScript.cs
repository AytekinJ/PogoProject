using UnityEngine;

public class DoorScript : MonoBehaviour
{
    public void DestroyDoor()
    {
        GetComponent<BoxCollider2D>().enabled = false;
        GetComponent<Animator>().SetTrigger("Hit");
        Destroy(gameObject, 0.9f);
    }
}
