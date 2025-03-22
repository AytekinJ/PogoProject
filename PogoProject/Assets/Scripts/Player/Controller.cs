using System;
using UnityEngine;
using System.Collections;

public class Controller : MonoBehaviour
{
    public static Rigidbody2D rb;
    private float inputX;

    #region silersiniz
    public GameObject normalGfx;
    public GameObject goldGfx;
    bool change;
    
    #endregion

    public float speed = 5f;
    public KeyCode JumpButton = KeyCode.Space;
    public static bool canChangeAnim = true;

    [SerializeField] Transform groundCheckPos;
    [SerializeField] float groundCheckRadius = 0.1f;
    [SerializeField] LayerMask groundCheckLayer;
    [SerializeField] float jumpForce = 7f;
    [SerializeField] float lowJumpMultiplier = 3f;
    [SerializeField] float fallMultiplier = 2.5f;

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
    private Animator goldGfxAnimator;
    private Animator normalGfxAnimator;

    //[Header("Sprinting Settings")]
    //[SerializeField] private KeyCode sprintButton = KeyCode.LeftShift;
    //[SerializeField] private float sprintMultiplier = 1.5f;
    //private bool isSprinting;
    [HideInInspector] public bool isFacingRight = true;
    AttackScript attackScript;

    bool HasController;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        jumpCooldownTime = coyoteTime + 0.05f;
        goldGfx.SetActive(false);
        goldGfxAnimator = goldGfx.GetComponent<Animator>();
        normalGfxAnimator = normalGfx.GetComponent<Animator>();
        attackScript = GetComponent<AttackScript>();
    }

    #region ControllerCheck
    void Start()
    {
        CheckController();
    }

    void CheckController()
    {
        string[] controllers = Input.GetJoystickNames();

        if (controllers.Length > 0)
        {
            foreach (string controller in controllers)
            {
                if (!string.IsNullOrEmpty(controller))
                {
                    HasController = true;
                    Debug.Log("Controller connected: " + controller);
                }
            }
        }
        else
        {
            Debug.Log("No controller detected.");
        }

        if (HasController)
        {
            JumpButton = KeyCode.JoystickButton14;
            attackScript.AttackKey = KeyCode.JoystickButton11;
        }
        
    }
    #endregion




    void Update()
    {
        HandleInputs();
        Move();
        AppendJump();
        ApplyJumpPhysics();
        UpdateCoyoteTime();
        AnimatorVariables();
        Flip();

        if(Input.GetKeyDown(KeyCode.G))
        {
            normalGfx.SetActive(change);
            goldGfx.SetActive(!change);
            change = !change;
        }
        
        

        if (jumpCooldownCounter > 0)
        {
            jumpCooldownCounter -= Time.deltaTime;
        }
    }

    void HandleInputs()
    {
        
        inputX = Mathf.Floor(Input.GetAxisRaw("Horizontal"));
        

        if (Input.GetKeyDown(JumpButton))
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }
    }

    void Move()
    {
        HealthScript.ReduceXKnockBack();
        rb.linearVelocity = new Vector2(inputX * speed + HealthScript.XKnockBack, rb.linearVelocity.y);
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
        CameraShake.StartShake(0.1f, 0.05f);
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
        #region Normal
        if (canChangeAnim)
        {
            normalGfxAnimator.SetFloat("Horizontal", Mathf.Abs(inputX));
            normalGfxAnimator.SetFloat("Vertical", rb.linearVelocity.y);
            normalGfxAnimator.SetFloat("VerticalInput", Input.GetAxisRaw("Vertical"));
            normalGfxAnimator.SetBool("isGrounded", CheckGrounded());
        }

        if (Input.GetKeyDown(JumpButton))
        {
            normalGfxAnimator.SetTrigger("JumpTrigger");
        }
        #endregion

        #region Gold
        if (canChangeAnim)
        {
            goldGfxAnimator.SetFloat("Horizontal", Mathf.Abs(inputX));
            goldGfxAnimator.SetFloat("Vertical", rb.linearVelocity.y);
            goldGfxAnimator.SetFloat("VerticalInput", Input.GetAxisRaw("Vertical"));
            goldGfxAnimator.SetBool("isGrounded", CheckGrounded());
        }

        if (Input.GetKeyDown(JumpButton))
        {
            goldGfxAnimator.SetTrigger("JumpTrigger");
        }
        #endregion

    }

    void Flip()
    {
        if (!canChangeAnim)
            return;
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
}
