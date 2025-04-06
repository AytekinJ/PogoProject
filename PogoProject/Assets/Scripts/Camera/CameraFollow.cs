using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public static CameraFollow Instance;

    public Transform target;
	public Vector3 offset = new Vector3(0, 0, -10);
	public float smoothValue = 0.2f;
    public Controller controllerScript;
    [SerializeField] float DownY = 0.5f;

    Vector3 movePosition;
    float RecentYvalue;

    private void Start()
    {
        Instance = this;
        movePosition = new Vector3(target.position.x, target.position.y) + offset;
        controllerScript = GameObject.FindGameObjectWithTag("Player").transform.GetComponent<Controller>();
    }

    Vector3 velocity = Vector3.zero;
    //-3.5 normal, -0.65 up
    private void Update() 
    {
        //Vector3 movePosition = new Vector3(target.position.x, target.position.y) + offset;
       

        if (controllerScript.CheckGrounded())
        {
            movePosition = new Vector3(target.position.x, target.position.y) + offset;
        }
        else if (!controllerScript.CheckGrounded())
        {
            movePosition = new Vector3(target.position.x, RecentYvalue) + offset;
        }

        transform.position = Vector3.SmoothDamp(transform.position, movePosition, ref velocity, smoothValue);

    }

    public void SetRecentY()
    {
        RecentYvalue = target.position.y - 0.167f + -DownY;
    }

    private void LateUpdate()
    {
        if (controllerScript.CheckGrounded())
            RecentYvalue = transform.position.y -0.167f + -DownY;
        else return;
    }
}
