using System;
using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class Controller : MonoBehaviour
{

    [SerializeField] private GameSetting gameSetting;


    public static Controller Instance { get; private set; }


    public bool canChangeAnim = true;


    //public static Rigidbody2D rb;


    public Rigidbody2D playerRb;


    public float inputX;
    public float inputY;

    [Header("Görsel Referansları (Inspector'dan atanmalı)")]
    public GameObject normalGfx;
    public GameObject goldGfx;
    private bool isGoldActive = false; 

    [Header("Hareket Ayarları")]
    public float speed = 5f;
    public KeyCode JumpButton;
    //public KeyCode DpadUp;
    //public KeyCode DpadDown;
    //public KeyCode DpadLeft;
    //public KeyCode DpadRight;


    [Header("Zıplama Ayarları")]
    [SerializeField] Transform groundCheckPos;
    [SerializeField] float groundCheckRadius = 0.1f;
    [SerializeField] LayerMask groundCheckLayer;
    [SerializeField] float jumpForce = 7f;
    [SerializeField] float lowJumpMultiplier = 3f;
    [SerializeField] float fallMultiplier = 2.5f;
    public GameObject PogoSFX;

    [Header("Jump Buffer & Coyote Time")]
    [SerializeField] private float jumpBufferTime = 0.1f;
    private float jumpBufferCounter;
    [SerializeField] private float coyoteTime = 0.1f;
    private float coyoteTimeCounter;
    public bool hasJumpedDuringCoyote;
    private float jumpCooldownTime;
    private float jumpCooldownCounter;


    private Animator currentAnimator;
    private Animator goldGfxAnimator;
    private Animator normalGfxAnimator;

    [HideInInspector] public bool isFacingRight = true;
    private AttackScript attackScript; 

    private bool HasController;
    [SerializeField] private bool isPogoing;

    InputSystem_Actions PlayerControls;
    Vector2 moveDpad;

    void OnEnable()
    {
        PlayerControls.DpadMove.Enable();
    }

    private void OnDisable() {
        PlayerControls.DpadMove.Disable();
    }
    private void Awake()
    {
        PlayerControls = new InputSystem_Actions();

        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        gameSetting = GameSetting.Instance;
  
        playerRb = GetComponent<Rigidbody2D>();
        if (playerRb == null) Debug.LogError("Rigidbody2D bulunamadı!", this);
 

        if (normalGfx != null) {
            normalGfxAnimator = normalGfx.GetComponent<Animator>();
            if (normalGfxAnimator == null) Debug.LogError("Normal GFX üzerinde Animator bulunamadı!", normalGfx);
        } else {
            Debug.LogError("Normal GFX GameObject atanmamış!", this);
        }

        if (goldGfx != null) {
            goldGfxAnimator = goldGfx.GetComponent<Animator>();
             if (goldGfxAnimator == null) Debug.LogError("Gold GFX üzerinde Animator bulunamadı!", goldGfx);
        } else {
             Debug.LogError("Gold GFX GameObject atanmamış!", this);
        }

    
        SetActiveAnimator(false);

        attackScript = GetComponent<AttackScript>();
        if (attackScript == null) Debug.LogWarning("AttackScript bulunamadı.", this);

        jumpCooldownTime = coyoteTime + 0.05f;

       
        if (gameSetting == null)
        {
            Debug.LogError("GameSetting ScriptableObject atanmamış!", this);
    
        }
        else
        {
       
            LoadKeysFromSettings();
        }

        InvokeRepeating(nameof(CheckController), 1, 1);
    }

    void Start()
    {
    
        CheckController(); 
                      
        InvokeRepeating(nameof(LoadKeysFromSettings), 1,1);
    }

    void SetActiveAnimator(bool gold)
    {
        isGoldActive = gold;
        if (normalGfx != null) normalGfx.SetActive(!gold);
        if (goldGfx != null) goldGfx.SetActive(gold);

        currentAnimator = gold ? goldGfxAnimator : normalGfxAnimator;

        if (currentAnimator == null)
        {
            Debug.LogWarning($"Aktif Animator ({(gold ? "Gold" : "Normal")}) null! Animasyonlar çalışmayabilir.");
        }
    }

    void LoadKeysFromSettings()
    {
        if (gameSetting == null) return;
        JumpButton = gameSetting.JumpButton;

        //DpadUp = gameSetting.DpadUp;
        //DpadDown = gameSetting.DpadDown;
        //DpadLeft = gameSetting.DpadLeft;
        //DpadRight = gameSetting.DpadRight;
    }

    public void CheckController()
    {
        string[] controllers = Input.GetJoystickNames();
        bool controllerConnected = false;
        foreach (string controller in controllers)
        {
            if (!string.IsNullOrEmpty(controller)) { controllerConnected = true; break; }
        }

        if (controllerConnected != HasController)
        {
            HasController = controllerConnected;
            LoadKeysFromSettings(); 

            Debug.Log(HasController ? "Controller connected." : "No controller detected.");
        }
    }

    void Update()
    {

        if (playerRb == null) return;

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
            SetActiveAnimator(!isGoldActive);
        }

        if (jumpCooldownCounter > 0)
        {
            jumpCooldownCounter -= Time.deltaTime;
        }
    }

    void HandleInputs()
    {

        

        //if (HasController)
        {
            //bool dpadUsed = false;
    
            // if (Input.GetKey(gameSetting.up)) { inputY = 1f; /*dpadUsed = true;*/ }
            // else if (Input.GetKey(gameSetting.down)) { inputY = -1f; /*dpadUsed = true;*/ }
            // if (Input.GetKey(gameSetting.left)) { inputX = -1f; /*dpadUsed = true;*/ }
            // else if (Input.GetKey(gameSetting.right)) { inputX = 1f; /*dpadUsed = true;*/ }

            //if (!dpadUsed)
            //{
    
            //    inputX = Input.GetAxisRaw("Horizontal");
            //    inputY = Input.GetAxisRaw("Vertical");
       
            //    if (Mathf.Abs(inputX) < 0.1f) inputX = 0f;
            //    if (Mathf.Abs(inputY) < 0.1f) inputY = 0f;
            //}
        }
        //else
        //{
           // Klavye kontrolü
           
            
            
           
        //}
        if(gameSetting.playWithDpad)
        {
            PlayerControls.DpadMove.DpadUp.performed += ctx => inputY = 1;
            PlayerControls.DpadMove.DpadUp.canceled += ctx => inputY= 0;

            PlayerControls.DpadMove.DpadDown.performed += ctx => inputY = -1;
            PlayerControls.DpadMove.DpadDown.canceled += ctx => inputY = 0;

            PlayerControls.DpadMove.DpadRight.performed += ctx => inputX = 1;
            PlayerControls.DpadMove.DpadRight.canceled += ctx => inputX= 0;

            PlayerControls.DpadMove.DpadLeft.performed += ctx => inputX = -1;
            PlayerControls.DpadMove.DpadLeft.canceled += ctx => inputX= 0;
        }
        else
        {
            inputX = 0f;
            inputY = 0f;
            
            if (Input.GetKey(gameSetting.up)) { inputY = 1f; /*dpadUsed = true;*/ }
            else if (Input.GetKey(gameSetting.down)) { inputY = -1f; /*dpadUsed = true;*/ }
            if (Input.GetKey(gameSetting.left)) { inputX = -1f; /*dpadUsed = true;*/ }
            else if (Input.GetKey(gameSetting.right)) { inputX = 1f; /*dpadUsed = true;*/ }

            
        }

        if (Input.GetKeyDown(JumpButton)) { jumpBufferCounter = jumpBufferTime; }
        else { jumpBufferCounter -= Time.deltaTime; }
    }


    void Move()
    {
        float currentKnockback = 0f;
        if (HealthScript.Instance != null)
        {
             currentKnockback = HealthScript.Instance.XKnockBack;
        }
        playerRb.linearVelocity = new Vector2(inputX * speed + currentKnockback, playerRb.linearVelocity.y);
    }

    void AppendJump()
    {
        if (jumpBufferCounter > 0 && coyoteTimeCounter > 0 && !hasJumpedDuringCoyote && jumpCooldownCounter <= 0)
        {
            playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, jumpForce); 
            jumpBufferCounter = 0;
            hasJumpedDuringCoyote = true;
            jumpCooldownCounter = jumpCooldownTime;
            if (currentAnimator != null) currentAnimator.SetTrigger("JumpTrigger");
        }
    }

    void ApplyJumpPhysics()
    {
        if (playerRb.linearVelocity.y < 0) { playerRb.gravityScale = fallMultiplier; } 
        else if (playerRb.linearVelocity.y > 0 && !Input.GetKey(JumpButton)) { playerRb.gravityScale = lowJumpMultiplier; } 
        else if (playerRb.linearVelocity.y > 0 && isPogoing) { playerRb.gravityScale = lowJumpMultiplier; } 
        else { playerRb.gravityScale = 1f; }
    }

    void DoingPogo() { isPogoing = true; /* rb.gravityScale = lowJumpMultiplier; */ ApplyJumpPhysics(); } 
    void Landed() { if (!isPogoing) return; if (CheckGrounded()) isPogoing = false; }

    public void DoPOGO(float pogoMultiplier, bool isEnemy)
    {
        DoingPogo();
        PlayPogoSFX(isEnemy);

        CameraShake.StartShake(0.1f, 0.05f); 


    
        playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, 0); 
        playerRb.AddForce(Vector2.up * pogoMultiplier, ForceMode2D.Impulse); 


        /*
        if (playerRb.velocity.y < 0f)
        {
            playerRb.velocity = new Vector2(playerRb.velocity.x, pogoMultiplier);
        }
        else 
        {
             playerRb.velocity = new Vector2(playerRb.velocity.x, playerRb.velocity.y + (pogoMultiplier / 1.5f)); // /2 yerine /1.5f daha fazla güç
        }
        */
    }


    void PlayPogoSFX(bool isEnemy)
    {
        if (isEnemy || PogoSFX == null) return;
        var sfx = Instantiate(PogoSFX, transform.position, Quaternion.identity);
        AudioSource audioSource = sfx.GetComponent<AudioSource>();
        if (audioSource != null) audioSource.pitch = UnityEngine.Random.Range(1f, 1.3f);
        Destroy(sfx, 3f);
    }

    void UpdateCoyoteTime()
    {
        if (CheckGrounded()) { coyoteTimeCounter = coyoteTime; hasJumpedDuringCoyote = false; }
        else { coyoteTimeCounter -= Time.deltaTime; }
    }

    #region Animation
    void AnimatorVariables()
    {
        if (currentAnimator != null && canChangeAnim)
        {
            currentAnimator.SetFloat("Horizontal", Mathf.Abs(inputX));
            currentAnimator.SetFloat("Vertical", playerRb.linearVelocity.y);
            currentAnimator.SetFloat("VerticalInput", inputY);
            currentAnimator.SetBool("isGrounded", CheckGrounded());
        }
    }

    void Flip()
    {
        if (!canChangeAnim) return;
        if ((isFacingRight && inputX < 0f) || (!isFacingRight && inputX > 0f))
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    public void EnableAnimationChange()
    {
        canChangeAnim = true;
        Debug.Log("Animation Change Enabled");
    }

    public void DisableAnimationChange()
    {
         canChangeAnim = false;
         Debug.Log("Animation Change Disabled");
    }

    #endregion

    public bool CheckGrounded()
    {
        if (groundCheckPos == null) return false;
        return Physics2D.OverlapCircle(groundCheckPos.position, groundCheckRadius, groundCheckLayer);
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheckPos != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheckPos.position, groundCheckRadius);
        }
    }

    //private void OnMouseDown()
    //{
    //    Cursor.visible = false;
    //}
}