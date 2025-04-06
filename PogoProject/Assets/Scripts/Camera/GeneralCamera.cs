using UnityEngine;

public class GeneralCamera : MonoBehaviour
{
    public Vector3 offset = new Vector3(0, 0, -10);
    public float smoothValue = 3f;
    Vector3 velocity = Vector3.zero;

    public static Transform TransformToLock;
    public static bool IsLocked = false;
    public static CameraFollow cameraFollowScript;
    public float distanceOffset = 5;

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
        //cameraFollowScript.enabled = false;
        TransformToLock = transform;
        IsLocked = true;
    }


    public static void UnlockCamera()
    {
        //cameraFollowScript.enabled = true;
        IsLocked = false;
    }


    void Update()
    {
        if (!IsLocked) return;
        Lerpcam();
    }


    void Lerpcam()
    {
        //Vector3 movePosition = new Vector3(TransformToLock.position.x, TransformToLock.position.y, offset.z);
        //transform.position = Vector3.Lerp(transform.position, movePosition, smoothValue * Time.deltaTime);

        Vector2 lockPos = new Vector2(TransformToLock.position.x, TransformToLock.position.y);
        Vector2 playerPos = new Vector2(cameraFollowScript.target.position.x, cameraFollowScript.target.position.y);
        float distance = Vector2.Distance(playerPos, lockPos);

        float Magnitude = Mathf.InverseLerp(0f, TransformToLock.GetComponent<CamPoint>().Distance, distance);
        //Debug.Log(Magnitude);

        //if (Vector2.Distance(lockPos, playerPos) > 3f)
        //{
            Vector2 direction = (lockPos - playerPos).normalized;
            Vector3 movePosition = new Vector3(TransformToLock.position.x + (direction.x * distanceOffset * 10) * Magnitude, TransformToLock.position.y + (direction.y * distanceOffset * 10) * Magnitude, offset.z);
            transform.position = Vector3.Lerp(transform.position, movePosition, smoothValue * Time.deltaTime);
        //    return;
        //}

        //Vector3 finalPosition = new Vector3(TransformToLock.position.x, TransformToLock.position.y, offset.z);
        //transform.position = Vector3.Lerp(transform.position, finalPosition, smoothValue * Time.deltaTime);
    }
}