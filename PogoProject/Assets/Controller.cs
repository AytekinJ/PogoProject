using UnityEngine;

public class Controller : MonoBehaviour
{
    Rigidbody2D rb;
    private float inputX;

    public float speed = 10f;
    public bool gotJumpInput;
    public KeyCode JumpButton = KeyCode.Space;

    [SerializeField] Transform groundCheckPos;
    [SerializeField] float groundCheckRadius = 0.25f;
    [SerializeField] LayerMask groundCheckLayer;
    [SerializeField] float jumpSpeed = 5f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        HandleInputs();
        Move();
        AppendJump();
    }

    void HandleInputs()
    {
        inputX = Input.GetAxisRaw("Horizontal");
        gotJumpInput = Input.GetKey(JumpButton);
    }

    void Move()
    {
        rb.linearVelocity = new Vector2(inputX * speed, rb.linearVelocityY);
    }

    void AppendJump()
    {
        if (gotJumpInput && CheckGrounded())
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpSpeed);
        }
    }

    public bool CheckGrounded()
    {
        return Physics2D.OverlapCircle(groundCheckPos.transform.position, groundCheckRadius, groundCheckLayer);
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheckPos.transform.position, groundCheckRadius);
    }
}
