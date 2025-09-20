using UnityEngine;

public class NewMovingPlatform : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float slowdownDistance = 1f;
    [SerializeField] private float waitTime = 1f;
    [SerializeField] private bool autoStart = false;
    [SerializeField] private bool startOnPlayerContact = true;
    [SerializeField] private bool startTowardsPointB = true;

    private Transform targetPoint;
    private float waitTimer = 0f;
    private bool isWaiting = false;
    public bool isActive = false;
    private Vector3 lastPosition;
    
    private Transform playerToParent = null;
    private Transform playerToDetach = null;
    private Vector3 startPos;

    private void Start()
    {
        startPos = transform.position;
        targetPoint = startTowardsPointB ? pointB : pointA;
        lastPosition = transform.position;
        
        if (autoStart)
        {
            isActive = true;
        }
    }

    private void Update()
    {
        if (playerToParent != null)
        {
            playerToParent.SetParent(transform);
            playerToParent = null;
        }
        
        if (playerToDetach != null)
        {
            playerToDetach.SetParent(null);
            playerToDetach = null;
        }
        
        if (!isActive || pointA == null || pointB == null)
            return;

        lastPosition = transform.position;

        if (isWaiting)
        {
            waitTimer += Time.deltaTime;

            if (waitTimer >= waitTime)
            {
                isWaiting = false;
                waitTimer = 0f;
                targetPoint = (targetPoint == pointA) ? pointB : pointA;
            }
            return;
        }

        Vector3 currentPos = transform.position;
        Vector3 targetPos = targetPoint.position;
        
        Vector3 direction = (targetPos - currentPos).normalized;
        float distance = Vector3.Distance(currentPos, targetPos);

        if (distance < 0.1f)
        {
            isWaiting = true;
            return;
        }
        
        float currentSpeed = moveSpeed;
        if (distance < slowdownDistance)
        {
            currentSpeed = Mathf.Lerp(0.5f, moveSpeed, distance / slowdownDistance);
        }

        transform.position = Vector3.MoveTowards(currentPos, targetPos, currentSpeed * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (startOnPlayerContact && collision.gameObject.CompareTag("Player") && !isActive)
        {
            isActive = true;
        }
        if (collision.gameObject.CompareTag("Player"))
        {
            float playerBottom = collision.collider.bounds.min.y;
            float platformTop = GetComponent<Collider2D>().bounds.max.y - 0.1f;

            if (playerBottom >= platformTop)
            {
                playerToParent = collision.transform;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && collision.transform.parent == transform)
        {
            playerToDetach = collision.transform;
        }
    }

    public void ActivatePlatform()
    {
        isActive = true;
    }

    public void DeactivatePlatform()
    {
        isActive = false;
    }

    public void SetDirection(bool towardsPointB)
    {
        targetPoint = towardsPointB ? pointB : pointA;
    }

    public void ResetPlatform()
    {
        isActive = startOnPlayerContact == true ? false : true;
        transform.position = startPos;
        isWaiting = false;
    }

    private void OnDrawGizmos()
    {
        if (pointA != null && pointB != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(pointA.position, pointB.position);
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(pointA.position, 0.2f);
            Gizmos.DrawSphere(pointB.position, 0.2f);
            
            if (Application.isPlaying == false)
            {
                Vector3 firstTarget = startTowardsPointB ? pointB.position : pointA.position;
                Vector3 startPos = transform.position;
                Gizmos.color = Color.green;
                Vector3 direction = (firstTarget - startPos).normalized;
                Gizmos.DrawRay(startPos, direction * 1.5f);
            }
        }
    }
}
