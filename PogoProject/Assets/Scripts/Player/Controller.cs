using UnityEngine;
using System.Collections;

public class Controller : MonoBehaviour
{
    Rigidbody2D rb;
    private float inputX;

    public float speed = 5f;
    public KeyCode JumpButton = KeyCode.Space;

    [SerializeField] Transform groundCheckPos;
    [SerializeField] float groundCheckRadius = 0.1f;
    [SerializeField] LayerMask groundCheckLayer;
    [SerializeField] float jumpForce = 7f;
    [SerializeField] float lowJumpMultiplier = 3f;
    [SerializeField] float fallMultiplier = 2.5f;
    private bool isJumping;

    [Header("Jump Buffer Settings")]
    [SerializeField] private float jumpBufferTime = 0.1f;
    private float jumpBufferCounter;

    [Header("Coyote Time Settings")]
    [SerializeField] private float coyoteTime = 0.1f;
    private float coyoteTimeCounter;
    public bool hasJumpedDuringCoyote;

    private float jumpCooldownTime;
    private float jumpCooldownCounter;
    Animator animator;

    [Header("Sprinting Settings")]
    [SerializeField] private KeyCode sprintButton = KeyCode.LeftShift;
    [SerializeField] private float sprintMultiplier = 1.5f;
    private bool isSprinting;
    bool isFacingRight = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        jumpCooldownTime = coyoteTime + 0.05f;
    }

    void Update()
    {
        HandleInputs();
        Move();
        AppendJump();
        ApplyJumpPhysics();
        UpdateCoyoteTime();
        AnimatorVariables();
        Flip();

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

        if (CheckGrounded())
        {
            isSprinting = Input.GetKey(sprintButton);
        }
        else
        {
            isSprinting = false;
        }
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
            isJumping = true;
            jumpBufferCounter = 0;
            hasJumpedDuringCoyote = true;
            jumpCooldownCounter = jumpCooldownTime;
        }
    }

    void ApplyJumpPhysics()
    {
        if (rb.linearVelocity.y < 0)
        {
            rb.gravityScale = fallMultiplier;
        }
        else if (rb.linearVelocity.y > 0 && !Input.GetKey(JumpButton))
        {
            rb.gravityScale = lowJumpMultiplier;
        }
        else
        {
            rb.gravityScale = 1f;
        }
    }

    public void DoPOGO(float pogoMultiplier)
    {
        if (rb.linearVelocity.y < 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, pogoMultiplier);
        }
        else if (rb.linearVelocity.y >= 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y + (pogoMultiplier / 2));
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

    #region Animation
    void AnimatorVariables()
    {
        animator.SetFloat("Horizontal", Mathf.Abs(inputX));
        animator.SetFloat("VerticalInput", Input.GetAxisRaw("Vertical"));
        animator.SetFloat("Vertical", rb.linearVelocity.y);
        animator.SetBool("isGrounded", CheckGrounded());

        if (Input.GetKeyDown(JumpButton))
        {
            StopAllCoroutines();
            StartCoroutine(Jumping());
        }
    }

    void Flip()
    {
        if (isFacingRight && inputX < 0f || !isFacingRight && inputX > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }
    #endregion

    public bool CheckGrounded()
    {
        return Physics2D.OverlapCircle(groundCheckPos.transform.position, groundCheckRadius, groundCheckLayer);
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheckPos.transform.position, groundCheckRadius);
    }

    IEnumerator Jumping()
    {
        animator.SetBool("isJumping", true);
        yield return new WaitForSeconds(0.1f);
        animator.SetBool("isJumping", false);
    }
}
