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
    [SerializeField] private bool startTowardsPointB = true;

    private Transform targetPoint;
    private float waitTimer = 0f;
    private bool isWaiting = false;
    private bool isActive = false;
    private Vector3 startPos;

    private void Start()
    {
        startPos = transform.position;
        targetPoint = startTowardsPointB ? pointB : pointA;
        
        if (autoStart && canMove)
        {
            isActive = true;
        }
    }

    private void Update()
    {
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
                targetPoint = (targetPoint == pointA) ? pointB : pointA;
            }
            return;
        }

        Vector3 currentPos = transform.position;
        Vector3 targetPos = targetPoint.position;
        
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

    public void ActivateMovement()
    {
        if (canMove)
        {
            isActive = true;
        }
    }

    public void DeactivateMovement()
    {
        isActive = false;
    }

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

    private void OnDrawGizmos()
    {
        if (!canMove)
            return;

        if (pointA != null && pointB != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(pointA.position, pointB.position);
            Gizmos.color = Color.yellow;
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
            
            // if (Application.isPlaying == false)
            // {
            //     Gizmos.color = new Color(1f, 0.5f, 0, 0.3f); // Semi-transparent orange
            //     Gizmos.DrawSphere(pointA.position, slowdownDistance);
            //     Gizmos.DrawSphere(pointB.position, slowdownDistance);
            // }
        }
    }
}
