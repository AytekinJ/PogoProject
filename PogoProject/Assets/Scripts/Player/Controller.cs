using System;
using UnityEngine;
using System.Collections;

public class Controller : MonoBehaviour
{
    // GameSetting referansı Inspector'dan atanmalı
    [SerializeField] private GameSetting gameSetting;

    // --- Singleton Deseni (AttackScript'in erişmesi için eklendi) ---
    public static Controller Instance { get; private set; }
    // ----------------------------------------------------------------

    // --- Static Değil! ---
    public bool canChangeAnim = true;
    // ---------------------

    public static Rigidbody2D rb; // Bu hala static kalabilir mi? Genellikle instance olması daha iyi.
                                  // Eğer static kalacaksa, her sahne yüklemesinde doğru Rigidbody'ye atandığından emin olunmalı.
                                  // Instance yapalım:
    private Rigidbody2D playerRb;
    // ---------------------

    public float inputX;
    public float inputY;

    [Header("Görsel Referansları (Inspector'dan atanmalı)")]
    public GameObject normalGfx;
    public GameObject goldGfx;
    private bool isGoldActive = false; // Aktif görseli takip etmek için

    [Header("Hareket Ayarları")]
    public float speed = 5f;
    public KeyCode JumpButton;
    public KeyCode DpadUp;
    public KeyCode DpadDown;
    public KeyCode DpadLeft;
    public KeyCode DpadRight;


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

    // Animator referansları
    private Animator currentAnimator; // O an aktif olan animator (normal veya gold)
    private Animator goldGfxAnimator;
    private Animator normalGfxAnimator;

    [HideInInspector] public bool isFacingRight = true;
    private AttackScript attackScript; // Referans

    private bool HasController;
    [SerializeField] private bool isPogoing;

    private void Awake()
    {
        // --- Singleton ---
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        // -----------------

        // Rigidbody referansını al (static olmayan)
        playerRb = GetComponent<Rigidbody2D>();
        if (playerRb == null) Debug.LogError("Rigidbody2D bulunamadı!", this);
        // Eski static atamayı kaldır: rb = GetComponent<Rigidbody2D>();

        // Animator referanslarını al ve kontrol et
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

        // Başlangıçta aktif olan animator'ü belirle
        SetActiveAnimator(false); // Başlangıçta normal aktif olsun

        attackScript = GetComponent<AttackScript>(); // AttackScript referansını al
        if (attackScript == null) Debug.LogWarning("AttackScript bulunamadı.", this);

        jumpCooldownTime = coyoteTime + 0.05f;

        // GameSetting kontrolü
        if (gameSetting == null)
        {
            Debug.LogError("GameSetting ScriptableObject atanmamış!", this);
            // Varsayılan tuşlar atanabilir veya hata verilebilir
        }
        else
        {
            // Tuşları GameSetting'den yükle
            LoadKeysFromSettings();
        }

        InvokeRepeating(nameof(CheckController), 1, 1);
    }

    void Start()
    {
        // İlk controller kontrolünü yap
        CheckController(); // Awake'de de çağrılıyor, Start'ta tekrar gerekebilir mi?
                           // Sahne yüklemesinden sonra emin olmak için Start'ta olabilir.
        InvokeRepeating(nameof(LoadKeysFromSettings), 1,1);
    }

    void SetActiveAnimator(bool gold)
    {
        isGoldActive = gold;
        if (normalGfx != null) normalGfx.SetActive(!gold);
        if (goldGfx != null) goldGfx.SetActive(gold);

        currentAnimator = gold ? goldGfxAnimator : normalGfxAnimator;

        // Eğer animator null ise uyarı ver
        if (currentAnimator == null)
        {
            Debug.LogWarning($"Aktif Animator ({(gold ? "Gold" : "Normal")}) null! Animasyonlar çalışmayabilir.");
        }
    }

    void LoadKeysFromSettings()
    {
        if (gameSetting == null) return;
        JumpButton = gameSetting.JumpButton;
        DpadUp = gameSetting.DpadUp;
        DpadDown = gameSetting.DpadDown;
        DpadLeft = gameSetting.DpadLeft;
        DpadRight = gameSetting.DpadRight;
        // Attack tuşu AttackScript'te atanmalı
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
            LoadKeysFromSettings(); // Controller durumu değişince tuşları tekrar yükle (varsa)

            Debug.Log(HasController ? "Controller connected." : "No controller detected.");
        }
    }

    void Update()
    {
        // Rigidbody null ise çık
        if (playerRb == null) return;

        HandleInputs();
        Move();
        AppendJump();
        ApplyJumpPhysics();
        UpdateCoyoteTime();
        AnimatorVariables(); // Aktif animator'ü kullanır
        Flip();
        Landed();

        // Test için G tuşu (Görsel değiştirme)
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
        // Input alma mantığı aynı kalabilir...
        inputX = 0f;
        inputY = 0f;

        if (HasController)
        {
            bool dpadUsed = false;
            // Dpad kontrolü...
            if (Input.GetKey(DpadUp)) { inputY = 1f; dpadUsed = true; }
            else if (Input.GetKey(DpadDown)) { inputY = -1f; dpadUsed = true; }
            if (Input.GetKey(DpadLeft)) { inputX = -1f; dpadUsed = true; }
            else if (Input.GetKey(DpadRight)) { inputX = 1f; dpadUsed = true; }

            if (!dpadUsed)
            {
                // Analog stick kontrolü
                inputX = Input.GetAxisRaw("Horizontal"); // GetAxisRaw genellikle daha iyi
                inputY = Input.GetAxisRaw("Vertical");
                // Küçük analog hareketlerini yok saymak için deadzone eklenebilir
                if (Mathf.Abs(inputX) < 0.1f) inputX = 0f;
                if (Mathf.Abs(inputY) < 0.1f) inputY = 0f;
            }
        }
        else
        {
            // Klavye kontrolü
            inputX = Input.GetAxisRaw("Horizontal"); // GetAxisRaw kullan
            inputY = Input.GetAxisRaw("Vertical"); // GetAxisRaw kullan
        }


        if (Input.GetKeyDown(JumpButton)) { jumpBufferCounter = jumpBufferTime; }
        else { jumpBufferCounter -= Time.deltaTime; }
    }


    void Move()
    {
        float currentKnockback = 0f;
        // HealthScript Instance üzerinden knockback al
        if (HealthScript.Instance != null)
        {
             currentKnockback = HealthScript.Instance.XKnockBack;
        }
        // Static olmayan playerRb kullan
        playerRb.linearVelocity = new Vector2(inputX * speed + currentKnockback, playerRb.linearVelocity.y);
    }

    void AppendJump()
    {
        if (jumpBufferCounter > 0 && coyoteTimeCounter > 0 && !hasJumpedDuringCoyote && jumpCooldownCounter <= 0)
        {
            playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, jumpForce); // Static olmayan playerRb
            jumpBufferCounter = 0;
            hasJumpedDuringCoyote = true;
            jumpCooldownCounter = jumpCooldownTime;
            // Zıplama trigger'ını aktif animator'de tetikle
            if (currentAnimator != null) currentAnimator.SetTrigger("JumpTrigger");
        }
    }

    void ApplyJumpPhysics()
    {
        if (playerRb.linearVelocity.y < 0) { playerRb.gravityScale = fallMultiplier; } // Static olmayan playerRb
        else if (playerRb.linearVelocity.y > 0 && !Input.GetKey(JumpButton)) { playerRb.gravityScale = lowJumpMultiplier; } // Static olmayan playerRb
        else if (playerRb.linearVelocity.y > 0 && isPogoing) { playerRb.gravityScale = lowJumpMultiplier; } // Static olmayan playerRb
        else { playerRb.gravityScale = 1f; }
    }

    void DoingPogo() { isPogoing = true; /* rb.gravityScale = lowJumpMultiplier; */ ApplyJumpPhysics(); } // ApplyJumpPhysics çağrısı daha doğru
    void Landed() { if (!isPogoing) return; if (CheckGrounded()) isPogoing = false; }

    public void DoPOGO(float pogoMultiplier, bool isEnemy)
    {
        DoingPogo();
        PlayPogoSFX(isEnemy);

        CameraShake.StartShake(0.1f, 0.05f); // Static kalabilir

        // Pogo gücünü uygula (Y hızını doğrudan ayarlamak yerine kuvvet uygulamak daha iyi olabilir)
        // Örnek: Var olan Y hızını sıfırla ve yukarı doğru kuvvet uygula
        playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, 0); // Önce Y hızını sıfırla
        playerRb.AddForce(Vector2.up * pogoMultiplier, ForceMode2D.Impulse); // Sonra kuvvet uygula


        // Eski Y hızına dayalı mantık (daha az tercih edilir):
        /*
        if (playerRb.velocity.y < 0f)
        {
            playerRb.velocity = new Vector2(playerRb.velocity.x, pogoMultiplier);
        }
        else // Zaten yukarı gidiyorsa ek güç ver
        {
            // Çok yüksek hızları engellemek için bir limit eklemek iyi olabilir
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
        Destroy(sfx, 3f); // Sesi Parent yapmaya gerek yok
    }

    void UpdateCoyoteTime()
    {
        if (CheckGrounded()) { coyoteTimeCounter = coyoteTime; hasJumpedDuringCoyote = false; }
        else { coyoteTimeCounter -= Time.deltaTime; }
    }

    #region Animation
    void AnimatorVariables()
    {
        // Sadece aktif animator'ü güncelle
        if (currentAnimator != null && canChangeAnim)
        {
            currentAnimator.SetFloat("Horizontal", Mathf.Abs(inputX));
            currentAnimator.SetFloat("Vertical", playerRb.linearVelocity.y); // Static olmayan playerRb
            currentAnimator.SetFloat("VerticalInput", inputY); // inputY kullanmak daha doğru
            currentAnimator.SetBool("isGrounded", CheckGrounded());
        }
        // Jump trigger AppendJump içinde tetikleniyor
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

    // Bu metod AttackScript tarafından Animation Event ile çağrılmalı
    public void EnableAnimationChange()
    {
        canChangeAnim = true;
        Debug.Log("Animation Change Enabled");
    }

    // Bu metod AttackScript tarafından çağrılmalı
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

    void OnDrawGizmosSelected() // OnDrawGizmos yerine Selected kullanmak daha az kalabalık yapar
    {
        if (groundCheckPos != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheckPos.position, groundCheckRadius);
        }
    }
}