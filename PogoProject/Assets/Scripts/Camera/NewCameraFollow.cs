using UnityEngine;

public class NewCameraFollow : MonoBehaviour
{
    public static NewCameraFollow Instance;

    public Transform target;
    public Vector3 offset = new Vector3(0, 0, -10);
    public float smoothValueY = 0.2f;
    public float smoothValueX = 0.2f;
    float velocity = 0;
    Vector3 vel = Vector3.zero;

    public float MinYValue = 0f;
    public float yMultiplier = 3f;

    //public Controller controllerScript;

    #region old
    //public Transform UnGroundedTransform;
    //public Transform GroundedTransform;
    #endregion
    #region old
    //public Controller controllerScript;
    //[SerializeField] float DownY = 0.34f;
    //public LayerMask GroundMask;
    #endregion
    #region old
    //public Transform movePosition;
    #endregion
    #region old
    //private Vector3 defaultOffset;
    //float inputYHoldTime;
    #endregion

    private void Start()
    {
        Instance = this;
        //controllerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<Controller>();
        #region old
        //movePosition.position = new Vector3(target.position.x, target.position.y, offset.z);
        //controllerScript = GameObject.FindGameObjectWithTag("Player").transform.GetComponent<Controller>();
        //defaultOffset = offset;
        //SetUnGrounded();
        //SetGrounded();
        #endregion
    }

    private void Update()
    {
        #region old
        //SetGrounded();
        //SetUnGrounded();
        //SetMovePosition();
        #endregion
        #region old
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
        #endregion
        Lerp();
    }

    void Lerp()
    {
        
        Vector3 xLerp;
        Vector3 yLerp;

        if (Controller.Instance.playerRb.linearVelocityY >= MinYValue)
        {
            xLerp = Vector3.SmoothDamp(transform.position, target.position, ref vel, smoothValueX);
            yLerp = Vector3.SmoothDamp(transform.position, target.position, ref vel, smoothValueY);
            transform.position = new Vector3(xLerp.x, yLerp.y, 0) + offset;
        }
        else if (Controller.Instance.playerRb.linearVelocityY < MinYValue)
        {
            xLerp = Vector3.SmoothDamp(transform.position, target.position, ref vel, smoothValueX);
            yLerp = Vector3.SmoothDamp(transform.position, target.position, ref vel, smoothValueY * yMultiplier);
            transform.position = new Vector3(xLerp.x, yLerp.y, 0) + offset;
        }

        

        
    }

    #region old
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
    #endregion
}
