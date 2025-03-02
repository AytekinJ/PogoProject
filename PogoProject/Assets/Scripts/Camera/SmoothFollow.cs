using UnityEngine;

public class SmoothFollow : MonoBehaviour
{
    Camera cam;

    public Transform objectToFollow;

    [SerializeField] float speed = 10f;

    public float targetFov = 6.5f;

    private void Awake()
    {
        cam = Camera.main;
    }

    void Update()
    {
        transform.position = Vector3.LerpUnclamped(transform.position, new Vector3(objectToFollow.position.x, objectToFollow.position.y, -10), speed * Time.deltaTime);
        cam.orthographicSize = Mathf.LerpUnclamped(cam.orthographicSize, targetFov, speed * Time.deltaTime);
    }
}
