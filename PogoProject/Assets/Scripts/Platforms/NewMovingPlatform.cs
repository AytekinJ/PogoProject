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
    [SerializeField] private bool startTowardsPointB = true; // Yeni: İlk hedef noktası seçimi

    private Transform targetPoint;
    private float waitTimer = 0f;
    private bool isWaiting = false;
    private bool isActive = false;
    private Vector3 lastPosition;
    
    // Track player for delayed parenting
    private Transform playerToParent = null;
    private Transform playerToDetach = null;

    private void Start()
    {
        // İlk hedef noktasını seçilen değere göre ayarla
        targetPoint = startTowardsPointB ? pointB : pointA;
        lastPosition = transform.position;
        
        // Auto start if configured
        if (autoStart)
        {
            isActive = true;
        }
    }

    private void Update()
    {
        // Handle delayed parenting
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

        // Store last position before moving
        lastPosition = transform.position;

        if (isWaiting)
        {
            waitTimer += Time.deltaTime;

            if (waitTimer >= waitTime)
            {
                isWaiting = false;
                waitTimer = 0f;
                // Switch target point
                targetPoint = (targetPoint == pointA) ? pointB : pointA;
            }
            return;
        }

        // Get current position and target position
        Vector3 currentPos = transform.position;
        Vector3 targetPos = targetPoint.position;
        
        // Calculate direction and distance
        Vector3 direction = (targetPos - currentPos).normalized;
        float distance = Vector3.Distance(currentPos, targetPos);

        // Check if we need to stop
        if (distance < 0.1f)
        {
            // We've reached the target point, start waiting
            isWaiting = true;
            return;
        }
        
        // Calculate speed based on distance (slow down when approaching)
        float currentSpeed = moveSpeed;
        if (distance < slowdownDistance)
        {
            // Gradually reduce speed as we approach the target
            currentSpeed = Mathf.Lerp(0.5f, moveSpeed, distance / slowdownDistance);
        }

        // Move platform using transform
        transform.position = Vector3.MoveTowards(currentPos, targetPos, currentSpeed * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if it's the player
        if (startOnPlayerContact && collision.gameObject.CompareTag("Player") && !isActive)
        {
            isActive = true;
        }

        // Schedule the player to be parented in the next frame
        if (collision.gameObject.CompareTag("Player"))
        {
            // Check if player is on top of the platform
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
        // Schedule the player to be detached in the next frame
        if (collision.gameObject.CompareTag("Player") && collision.transform.parent == transform)
        {
            playerToDetach = collision.transform;
        }
    }

    // Public method to activate platform externally
    public void ActivatePlatform()
    {
        isActive = true;
    }

    // Public method to deactivate platform externally
    public void DeactivatePlatform()
    {
        isActive = false;
    }

    // Hareket yönünü manuel olarak değiştirmek için
    public void SetDirection(bool towardsPointB)
    {
        targetPoint = towardsPointB ? pointB : pointA;
    }

    // Optional: Visualize the path in the editor
    private void OnDrawGizmos()
    {
        if (pointA != null && pointB != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(pointA.position, pointB.position);
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(pointA.position, 0.2f);
            Gizmos.DrawSphere(pointB.position, 0.2f);
            
            // İlk yön göstergesi
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
