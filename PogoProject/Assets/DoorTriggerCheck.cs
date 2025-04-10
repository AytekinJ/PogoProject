using UnityEngine;

public class DoorTriggerCheck : MonoBehaviour
{
    DoorScript doorScript;
    void Start()
    {
        doorScript = GetComponentInParent<DoorScript>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        doorScript.IsPlayerInside = true;
        doorScript.StopAllCoroutines();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        doorScript.IsPlayerInside = false;
        doorScript.StartCoroutine(doorScript.RespawnDoor());
    }


}
