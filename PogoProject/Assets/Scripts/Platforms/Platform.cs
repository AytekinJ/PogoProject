using System;
using TMPro;
using UnityEngine;

public class Platform : MonoBehaviour
{
    public bool movingPlatform = false;
    [SerializeField, HideInInspector] public Transform startPosition;
    [SerializeField, HideInInspector] public Transform endPosition;
    [SerializeField, HideInInspector] public float moveLength = 4f;
    [Space(2f)]

    public bool breakablePlatform = false;
    [SerializeField, HideInInspector] public float duration = 1f;
    [SerializeField, HideInInspector] public float delay = 3f;
    [HideInInspector] public bool isInteracted;

    public bool jumpSwitch = false;
    [SerializeField, HideInInspector] public GameObject matchedPlatform;
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        isInteracted = true;

    }

    private void OnCollisionExit2D(Collision2D other)
    {
        isInteracted = false;
    }

    private void OnDrawGizmos()
    {
        if (movingPlatform)
        {
            Gizmos.color = Color.yellow;
            Vector3 start = transform.position;
            Vector3 end1 = new Vector3(start.x + moveLength, start.y, start.z);
            Vector3 end2 = new Vector3(start.x - moveLength, start.y, start.z);

            Gizmos.DrawLine(start, end1);
            Gizmos.DrawLine(start, end2);
        }
    }
}
