using UnityEngine;
using System.Collections;

public class Controller : MonoBehaviour
{
    Rigidbody2D rb;
    private float inputX;

    public float speed = 10f;
    public KeyCode JumpButton = KeyCode.Space;

    [SerializeField] Transform groundCheckPos;
    [SerializeField] float groundCheckRadius = 0.25f;
    [SerializeField] LayerMask groundCheckLayer;
    [SerializeField] float jumpForce = 5f;

    [Header("Jump Buffer Settings")]
    [SerializeField] private float jumpBufferTime = 0.1f;
    private float jumpBufferCounter;

    [Header("Coyote Time Settings")]
    [SerializeField] private float coyoteTime = 0.15f;
    private float coyoteTimeCounter;
    public bool hasJumpedDuringCoyote;

    private float jumpCooldownTime;
    private float jumpCooldownCounter;


    [Header("Sprinting Settings")]
    [SerializeField] private KeyCode sprintButton = KeyCode.LeftShift;
    [SerializeField] private float sprintMultiplier = 1.5f;
    private bool isSprinting;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        jumpCooldownTime = coyoteTime + 0.05f;
    }

    void Update()
    {
        HandleInputs();
        Move();
        AppendJump();
        UpdateCoyoteTime();

        if (jumpCooldownCounter > 0)
        {
            jumpCooldownCounter -= Time.deltaTime;
        }
    }

    void HandleInputs()
    {
        inputX = Input.GetAxisRaw("Horizontal");

        if (Input.GetKeyDown(JumpButton))
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        isSprinting = Input.GetKey(sprintButton);
    }

    void Move()
    {
        float currentSpeed = isSprinting ? speed * sprintMultiplier : speed;
        rb.linearVelocity = new Vector2(inputX * currentSpeed, rb.linearVelocity.y);
    }

    void AppendJump()
    {
        if (jumpBufferCounter > 0 && coyoteTimeCounter > 0 && !hasJumpedDuringCoyote && jumpCooldownCounter <= 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpBufferCounter = 0;
            hasJumpedDuringCoyote = true;
            jumpCooldownCounter = jumpCooldownTime;
        }
    }

    public void DoPOGO(float pogoMultiplier)
    {
        if (rb.linearVelocityY < 0f)
        {
            rb.linearVelocityY = 0f;
            rb.linearVelocityY += pogoMultiplier;
        }
        else if (rb.linearVelocityY >= 0f)
        {
            rb.linearVelocityY += pogoMultiplier / 2;
        }
    }

    void UpdateCoyoteTime()
    {
        if (CheckGrounded())
        {
            coyoteTimeCounter = coyoteTime;
            hasJumpedDuringCoyote = false;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
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
