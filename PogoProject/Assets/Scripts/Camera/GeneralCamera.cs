using UnityEngine;

public class GeneralCamera : MonoBehaviour
{
    public Vector3 offset = new Vector3(0, 0, -10);
    public float smoothValue = 3f;
    Vector3 velocity = Vector3.zero;

    public static Transform TransformToLock;
    public static bool IsLocked = false;
    public static CameraFollow cameraFollowScript;

    private void Start()
    {
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

    public static void LockToTransform(Transform transform)
    {
        if (transform == null)
        {
            return;
        }
        TransformToLock = transform;
        IsLocked = true;
    }

    public static void UnlockCamera()
    {
        IsLocked = false;
    }

    void Update()
    {
        if (!IsLocked || TransformToLock == null || cameraFollowScript == null) return;
        Lerpcam();
    }

    void Lerpcam()
    {
        Vector2 lockPos = new Vector2(TransformToLock.position.x, TransformToLock.position.y);
        Vector2 playerPos = new Vector2(cameraFollowScript.Target.transform.position.x, cameraFollowScript.player.transform.position.y);

        if (Vector2.Distance(lockPos, playerPos) > 3f)
        {
            Vector2 direction = (lockPos - playerPos).normalized;
            Vector3 movePosition = new Vector3(TransformToLock.position.x + (direction.x * 3), TransformToLock.position.y + (direction.y * 3), offset.z);
            transform.position = Vector3.Lerp(transform.position, movePosition, smoothValue * Time.deltaTime);
            return;
        }

        Vector3 finalPosition = new Vector3(TransformToLock.position.x, TransformToLock.position.y, offset.z);
        transform.position = Vector3.Lerp(transform.position, finalPosition, smoothValue * Time.deltaTime);
    }
}