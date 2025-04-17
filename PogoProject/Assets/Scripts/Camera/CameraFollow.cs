using UnityEngine;
using UnityEngine.UIElements;

public class CameraFollow : MonoBehaviour
{
    public static CameraFollow Instance;

    public Transform target;
    public Transform UnGroundedTransform;
    public Transform GroundedTransform;
    public Vector3 offset = new Vector3(0, 0, -10);
    public float smoothValue = 0.2f;
    public Controller controllerScript;
    [SerializeField] float DownY = 0.34f;
    public LayerMask GroundMask;

    Vector3 movePosition;
    Vector3 velocity = Vector3.zero;

    private void Start()
    {
        Instance = this;
        movePosition = new Vector3(target.position.x, target.position.y) + offset;
        controllerScript = GameObject.FindGameObjectWithTag("Player").transform.GetComponent<Controller>();

        SetUnGrounded();
        SetGrounded();
    }

    private void Update()
    {
        SetGrounded();
        SetUnGrounded();
        SetMovePosition();
        Lerp();

        // if(Mathf.Abs(target.position.y - gameObject.transform.position.y) > 2.5)
        // {
        //     SetCamFollowPublic(target.position);
        // }
    }

    void Lerp()
    {
        transform.position = Vector3.SmoothDamp(transform.position, movePosition, ref velocity, smoothValue);
    }

    void SetMovePosition()
    {
        if (controllerScript.CheckGrounded())
        {
            movePosition = GroundedTransform.position;
        }
        else
        {
            movePosition = new Vector3(target.position.x, GroundedTransform.position.y, UnGroundedTransform.position.z);
        }
    }

    void SetUnGrounded()
    {
        if (!controllerScript.CheckGrounded())
        {
            RaycastHit2D ray;
            ray = Physics2D.Raycast(target.transform.position, Vector2.down, Mathf.Infinity, GroundMask);
            UnGroundedTransform.position = new Vector3(ray.point.x, ray.point.y + 1f, offset.z);
        }
    }

    void SetGrounded()
    {
        if (controllerScript.CheckGrounded())
            GroundedTransform.position = target.position + offset;
    }

    public void SetCamFollowPublic(Vector3 targetTransorm)
    {
        GroundedTransform.position = targetTransorm + offset;
    }
    
}