using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Platform : MonoBehaviour
{
    public bool movingPlatform = false;
    [SerializeField, HideInInspector] public bool usePositions;
    [SerializeField, HideInInspector] public bool useLength;
    [SerializeField, HideInInspector] public Transform startPosition;
    [SerializeField, HideInInspector] public Transform endPosition;
    [SerializeField, HideInInspector] public float moveLength = 4f;
    [Space(2f)]

    public bool breakablePlatform = false;
    [SerializeField, HideInInspector] public float duration = 1f;
    [SerializeField, HideInInspector] public float delay = 3f;
    [HideInInspector] public bool isInteracted;
    [HideInInspector] public GameObject interactingObject;

    public bool jumpSwitch = false;
    [SerializeField, HideInInspector] public GameObject matchedPlatform;

    public Vector2 center;
    public bool active = true;

    private bool started = false;
    
    private void Awake()
    {
        center = transform.position;
        started = true;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        isInteracted = true;
        interactingObject = other.gameObject;
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        isInteracted = false;
        interactingObject.transform.parent = null;
        interactingObject = null;
        
    }

    private void OnValidate()
    {
        if (useLength)
        {
            usePositions = false;
        }

        if (usePositions)
        {
            useLength = false;
        }
    }

    private void OnDrawGizmos()
    {
        if (movingPlatform && !started)
        {
            Gizmos.color = Color.yellow;
            Vector3 start = transform.position;
            Vector3 end1 = new Vector3(start.x + moveLength, start.y, start.z);
            Vector3 end2 = new Vector3(start.x - moveLength, start.y, start.z);

            Gizmos.DrawLine(start, end1);
            Gizmos.DrawLine(start, end2);
        }
        else if (movingPlatform)
        {
            Gizmos.color = Color.yellow;
            Vector3 start = center;
            Vector3 end1 = new Vector3(start.x + moveLength, start.y, start.z);
            Vector3 end2 = new Vector3(start.x - moveLength, start.y, start.z);

            Gizmos.DrawLine(start, end1);
            Gizmos.DrawLine(start, end2);
        }
    }
}
