using UnityEngine;

public class GeneralCamera : MonoBehaviour
{
    public Vector3 offset = new Vector3(0, 0, -10);
    public float smoothValue = 15f;
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
            Debug.LogError("Main Camera not found!");
        }
    }


    public static void LockToTransform(Transform transform)
    {
        if (transform == null)
        {
            Debug.LogWarning("Attempted to lock camera to a null transform!");
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
        Vector3 movePosition = new Vector3(TransformToLock.position.x, TransformToLock.position.y, offset.z);
        transform.position = Vector3.SmoothDamp(transform.position, movePosition, ref velocity, smoothValue * Time.deltaTime);
    }
}