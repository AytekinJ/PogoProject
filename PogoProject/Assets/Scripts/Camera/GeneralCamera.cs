using UnityEngine;

public class GeneralCamera : MonoBehaviour
{
    public static GeneralCamera Instance;
    public Vector3 offset = new Vector3(0, 0, -10);
    public float smoothValue = 3f;
    Vector3 velocity = Vector3.zero;

    public static Transform TransformToLock;
    public static bool IsLocked = false;
    public static CameraFollow cameraFollowScript;
    public float distanceOffset = 5;

    bool LockX;
    bool LockY;

    private void Start()
    {
        Instance = this;
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            cameraFollowScript = mainCamera.GetComponent<CameraFollow>();
        }
        else
        {
            Debug.LogError("Main Camera not found");
        }
    }

    public void LockToTransform(Transform transform, bool lockX, bool lockY)
    {
        LockX = lockX;
        LockY = lockY;
        if (transform == null)
        {
            return;
        }
        TransformToLock = transform;
        IsLocked = true;
    }

    public void UnlockCamera()
    {
        IsLocked = false;
        LockX = false;
        LockY = false;
    }

    void Update()
    {
        if (!IsLocked) return;
        Lerpcam();
    }

    void Lerpcam()
{
    if (TransformToLock == null)
    {
        UnlockCamera(); 
        return;
    }

    Vector2 lockPos = new Vector2(TransformToLock.position.x, TransformToLock.position.y);


    if (cameraFollowScript == null || cameraFollowScript.target == null)
    {
         
         UnlockCamera();
         return;
    }

    Vector2 playerPos = new Vector2(cameraFollowScript.target.position.x, cameraFollowScript.target.position.y);
    float distance = Vector2.Distance(playerPos, lockPos);

    CamPoint camPoint = TransformToLock.GetComponent<CamPoint>();
    if (camPoint == null) {
        UnlockCamera();
        return;
    }

    float Magnitude = Mathf.InverseLerp(0f, camPoint.Distance, distance);
    Vector2 direction = (lockPos - playerPos).normalized;

    float targetX = TransformToLock.position.x + (direction.x * distanceOffset) * Magnitude;
    float targetY = TransformToLock.position.y + (direction.y * distanceOffset) * Magnitude;

    if (LockX) targetX = TransformToLock.position.x;
    if (LockY) targetY = TransformToLock.position.y;

    Vector3 movePosition = new Vector3(targetX, targetY, offset.z);
    transform.position = Vector3.Lerp(transform.position, movePosition, smoothValue * Time.deltaTime);
}
}