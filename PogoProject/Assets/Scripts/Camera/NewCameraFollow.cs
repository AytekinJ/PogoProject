using UnityEngine;

public class NewCameraFollow : MonoBehaviour
{
    public static NewCameraFollow Instance;

    public Transform target;
    //public Transform UnGroundedTransform;
    //public Transform GroundedTransform;
    public Vector3 offset = new Vector3(0, 0, -10);
    public float smoothValueY = 0.2f;
    public float smoothValueX = 0.2f;
    //public Controller controllerScript;
    //[SerializeField] float DownY = 0.34f;
    //public LayerMask GroundMask;

    //public Transform movePosition;
    float velocity = 0;
    Vector3 vel = Vector3.zero;
    //private Vector3 defaultOffset;
    //float inputYHoldTime;
    private void Start()
    {
        Instance = this;
        //movePosition.position = new Vector3(target.position.x, target.position.y, offset.z);
        //controllerScript = GameObject.FindGameObjectWithTag("Player").transform.GetComponent<Controller>();
        //defaultOffset = offset;
        //SetUnGrounded();
        //SetGrounded();
    }

    private void Update()
    {
        //SetGrounded();
        //SetUnGrounded();
        //SetMovePosition();
        Lerp();

        // if(Mathf.Abs(target.position.y - gameObject.transform.position.y) > 2.5)
        // {
        //     SetCamFollowPublic(target.position);
        // }

        //if (controllerScript.inputX == 0 && controllerScript.inputY != 0 && controllerScript.CheckGrounded())
        //{
        //    inputYHoldTime += Time.deltaTime;

        //    if (inputYHoldTime > 1)
        //    {
        //        offset = new Vector3(defaultOffset.x, defaultOffset.y + 1 * controllerScript.inputY, defaultOffset.z);
        //    }
        //}
        //else
        //{
        //    offset = defaultOffset;
        //    inputYHoldTime = 0;
        //}
    }

    void Lerp()
    {
        //float lerpingY;
        //float lerpingX;

        Vector3 xLerp;
        Vector3 yLerp;

        xLerp = Vector3.SmoothDamp(transform.position, target.position, ref vel, smoothValueX);
        yLerp = Vector3.SmoothDamp(transform.position, target.position, ref vel, smoothValueY);

        //lerpingX = Mathf.SmoothDamp(transform.position.x, target.position.x, ref velocity, smoothValueX);
        //lerpingY = Mathf.SmoothDamp(transform.position.y, target.position.y, ref velocity, smoothValueY);

        transform.position = new Vector3(xLerp.x, yLerp.y, 0) + offset;
    }

    //void SetMovePosition()
    //{
    //    if (controllerScript.CheckGrounded())
    //    {
    //        movePosition = GroundedTransform.position;
    //    }
    //    else
    //    {
    //        movePosition = new Vector3(target.position.x, GroundedTransform.position.y, UnGroundedTransform.position.z);
    //    }
    //}

    //void SetUnGrounded()
    //{
    //    if (!controllerScript.CheckGrounded())
    //    {
    //        RaycastHit2D ray;
    //        ray = Physics2D.Raycast(target.transform.position, Vector2.down, Mathf.Infinity, GroundMask);
    //        UnGroundedTransform.position = new Vector3(ray.point.x, ray.point.y + 1f, offset.z);
    //    }
    //}

    //void SetGrounded()
    //{
    //    if (controllerScript.CheckGrounded())
    //        GroundedTransform.position = target.position + offset;
    //}

    //public void SetCamFollowPublic(Vector3 targetTransorm)
    //{
    //    GroundedTransform.position = targetTransorm + offset;
    //}
}
