using UnityEngine;

public class NewCameraFollow : MonoBehaviour
{
    public static NewCameraFollow Instance;

    public Transform target;
    public Vector3 offset = new Vector3(0, 0, -10);
    public float smoothValueY = 0.2f;
    public float smoothValueX = 0.2f;

    Vector3 velX = Vector3.zero;
    Vector3 velY = Vector3.zero;

    public float MinYValue = 0f;
    public float yMultiplier = 3f;
    float SavedMultiplier;

    private void Start()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;

        SavedMultiplier = yMultiplier;
    }

    private void LateUpdate()
    {
        Lerp();
    }

    void Lerp()
    {
        float targetX = target.position.x;
        float targetY = target.position.y;

        if (Controller.Instance.playerRb.linearVelocityY >= MinYValue)
        {
            yMultiplier = 1f;
        }
        else
        {
            yMultiplier = SavedMultiplier;
        }

        float newX = Mathf.SmoothDamp(transform.position.x, targetX, ref velX.x, smoothValueX);
        float newY = Mathf.SmoothDamp(transform.position.y, targetY, ref velY.y, smoothValueY / yMultiplier);

        transform.position = new Vector3(newX, newY, 0f) + offset;
    }
}
