using UnityEngine;

public class ChainsawScript : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float slowdownDistance = 1f;
    [SerializeField] private float waitTime = 1f;
    [SerializeField] private bool autoStart = true;
    [SerializeField] private bool canMove = true;
    [SerializeField] private bool startTowardsPointB = true; // Initial target point selection

    // Movement variables
    private Transform targetPoint;
    private float waitTimer = 0f;
    private bool isWaiting = false;
    private bool isActive = false;
    private Vector3 startPos;

    private void Start()
    {
        startPos = transform.position;
        // Set initial target point based on selected value
        targetPoint = startTowardsPointB ? pointB : pointA;
        
        // Auto start if configured
        if (autoStart && canMove)
        {
            isActive = true;
        }
    }

    private void Update()
    {
        // Don't move if canMove is false
        if (!canMove)
        {
            isActive = false;
            return;
        }
        
        if (!isActive || pointA == null || pointB == null)
            return;

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

        // Move saw using transform with calculated speed
        transform.position = Vector3.MoveTowards(currentPos, targetPos, currentSpeed * Time.deltaTime);
    }

    // Public method to activate saw movement externally
    public void ActivateMovement()
    {
        if (canMove)
        {
            isActive = true;
        }
    }

    // Public method to deactivate saw movement externally
    public void DeactivateMovement()
    {
        isActive = false;
    }

    // Toggle the ability to move
    public void SetCanMove(bool value)
    {
        canMove = value;
        if (!canMove)
        {
            isActive = false;
        }
        else if (autoStart)
        {
            isActive = true;
        }
    }

    // Set movement direction manually
    public void SetDirection(bool towardsPointB)
    {
        targetPoint = towardsPointB ? pointB : pointA;
    }

    public void ResetChainsaw()
    {
        isActive = true;
        transform.position = startPos;
        isWaiting = false;
    }

    // Optional: Visualize the path in the editor
    private void OnDrawGizmos()
    {
        // Don't draw gizmos if canMove is false
        if (!canMove)
            return;

        if (pointA != null && pointB != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(pointA.position, pointB.position);
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(pointA.position, 0.2f);
            Gizmos.DrawSphere(pointB.position, 0.2f);
            
            // Initial direction indicator
            if (Application.isPlaying == false)
            {
                Vector3 firstTarget = startTowardsPointB ? pointB.position : pointA.position;
                Vector3 startPos = transform.position;
                Gizmos.color = Color.green;
                Vector3 direction = (firstTarget - startPos).normalized;
                Gizmos.DrawRay(startPos, direction * 1.5f);
            }
            
            // Visualize slowdown distance
            // if (Application.isPlaying == false)
            // {
            //     Gizmos.color = new Color(1f, 0.5f, 0, 0.3f); // Semi-transparent orange
            //     Gizmos.DrawSphere(pointA.position, slowdownDistance);
            //     Gizmos.DrawSphere(pointB.position, slowdownDistance);
            // }
        }
    }
}