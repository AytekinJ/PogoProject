using System;
using UnityEngine;
using System.Collections;

public class Controller : MonoBehaviour
{
    public GameSetting gameSetting;

    public static Rigidbody2D rb;
    public float inputX;
    public float inputY;

    #region silersiniz
    public GameObject normalGfx;
    public GameObject goldGfx;
    bool change;
    
    #endregion

    public float speed = 5f;
    public KeyCode JumpButton;
    public KeyCode DpadUp;
    public KeyCode DpadDown;
    public KeyCode DpadLeft;
    public KeyCode DpadRight;
    public static bool canChangeAnim = true;

    [SerializeField] Transform groundCheckPos;
    [SerializeField] float groundCheckRadius = 0.1f;
    [SerializeField] LayerMask groundCheckLayer;
    [SerializeField] float jumpForce = 7f;
    [SerializeField] float lowJumpMultiplier = 3f;
    [SerializeField] float fallMultiplier = 2.5f;

    public GameObject PogoSFX;

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

    [HideInInspector] public bool isFacingRight = true;
    AttackScript attackScript;

    bool HasController;
    [SerializeField] bool isPogoing;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        jumpCooldownTime = coyoteTime + 0.05f;
        goldGfx.SetActive(false);
        goldGfxAnimator = goldGfx.GetComponent<Animator>();
        normalGfxAnimator = normalGfx.GetComponent<Animator>();
        attackScript = GetComponent<AttackScript>();
        InvokeRepeating(nameof(CheckController), 1, 1);

        JumpButton = gameSetting.JumpButton;
        DpadUp = gameSetting.DpadUp;
        DpadDown = gameSetting.DpadDown;
        DpadLeft = gameSetting.DpadLeft;
        DpadRight = gameSetting.DpadRight;
    }

    #region ControllerCheck
    void Start()
    {
        CheckController();
    }

    void CheckController()
    {
        string[] controllers = Input.GetJoystickNames();
        bool controllerConnected = false;

        foreach (string controller in controllers)
        {
            if (!string.IsNullOrEmpty(controller))
            {
                controllerConnected = true;
                break;
            }
        }

        if (controllerConnected != HasController)
        {
            HasController = controllerConnected;
            if (HasController)
            {
                JumpButton = KeyCode.JoystickButton0;
                attackScript.AttackKey = KeyCode.JoystickButton2;
                Debug.Log("Controller connected.");
            }
            else
            {
                JumpButton = KeyCode.Space;
                attackScript.AttackKey = KeyCode.X;
                Debug.Log("No controller detected.");
            }
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
        Landed();

        if (Input.GetKeyDown(KeyCode.G))
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
        inputX = 0f;
        inputY = 0f;

        if (HasController)
        {
            bool dpadUsed = false;
            if (Input.GetKey(DpadUp))
            {
                inputY = 1f;
                dpadUsed = true;
            }
            else if (Input.GetKey(DpadDown))
            {
                inputY = -1f;
                dpadUsed = true;
            }

            if (Input.GetKey(DpadLeft))
            {
                inputX = -1f;
                dpadUsed = true;
            }
            else if (Input.GetKey(DpadRight))
            {
                inputX = 1f;
                dpadUsed = true;
            }

            if (!dpadUsed)
            {
                inputX = Mathf.Floor(Input.GetAxisRaw("Horizontal"));
                inputY = Mathf.Floor(Input.GetAxisRaw("Vertical"));
            }
        }
        else
        {
            inputX = Mathf.Floor(Input.GetAxisRaw("Horizontal"));
            inputY = Mathf.Floor(Input.GetAxisRaw("Vertical"));
        }

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
        else if (rb.linearVelocity.y > 0 && isPogoing)
        {
            rb.gravityScale = lowJumpMultiplier;
        }
        else
        {
            rb.gravityScale = 1f;
        }
    }

    void DoingPogo()
    {
        isPogoing = true;
        rb.gravityScale = lowJumpMultiplier;
    }

    void Landed()
    {
        if (!isPogoing)
            return;
        if (CheckGrounded())
        isPogoing = false;
    }

    public void DoPOGO(float pogoMultiplier, bool isEnemy)
    {
        DoingPogo();
        PlaySFX(isEnemy);

        CameraShake.StartShake(0.1f, 0.05f);
        if (rb.linearVelocity.y < 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, pogoMultiplier);
        }
        else if (rb.linearVelocity.y >= .5f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y + (pogoMultiplier / 2));
        }
    }

    void PlaySFX(bool isEnemy)
    {
        if (isEnemy)
            return;
        var sfx = Instantiate(PogoSFX, transform.position, Quaternion.identity, gameObject.transform);
        sfx.GetComponent<AudioSource>().pitch = UnityEngine.Random.Range(1f, 1.3f);
        Destroy(sfx, 3f);
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
