using System;
using System.Collections;
using UnityEngine;

public class AttackScript : MonoBehaviour
{
    public int Damage = 1;
    public float POGOMultiplier = 10f;
    #region silersiniz
    public GameObject normalGfx;
    public GameObject goldGfx;
    
    #endregion

    float Xinput, Yinput;

    public KeyCode AttackKey = KeyCode.X;
    public bool up, down, left, right;

    public float AttackCooldown = 0.5f;
    private float attacktime = 0f;

    public float attackRange = 0.75f;
    public Vector2 boxSize = new Vector2(0.75f, 0.75f);
    public LayerMask attackMask;

    private Controller playerController;
    HitParticleScript particleScript;
    Animator animator;
    private Vector2 attackDirection;
    [SerializeField] private Animator normal;
    [SerializeField] private Animator gold;

    [SerializeField] float CamShakeDuration = 0.1f;
    [SerializeField] float CamShakeMagnitude = 0.05f;

    [SerializeField] GameObject SwordSwaySFX;

    void Start()
    {
        playerController = GetComponent<Controller>();
        animator = GetComponent<Animator>();
        particleScript = GetComponent<HitParticleScript>();
    }

    void Update()
    {
        GetInputs();
        CalculateDirection();
        AppendAttack();
        SetLastAttackPos();
    }

    void PlaySFX()
    {
        var sfx = Instantiate(SwordSwaySFX, transform.position, Quaternion.identity, gameObject.transform);
        sfx.GetComponent<AudioSource>().pitch = UnityEngine.Random.Range(0.9f, 1.1f);
        Destroy(sfx, 3f);
    }

    void AppendAttack()
    {
        if (Input.GetKeyDown(AttackKey) && Time.time >= attacktime)
        {
            attackDirection = GetAttackDirection();

            if (attackDirection == Vector2.down && playerController.CheckGrounded())
            {
                return;
            }

            PlaySFX();

            if (attackDirection != Vector2.zero)
            {
                CastAttackBox(attackDirection);
                particleScript.CastParticleBox(attackRange + 0.1f, attackDirection);
                //animasyonlar için gerekliydi, yazdım (ayt)
                
                Controller.canChangeAnim = false;
                normal.SetTrigger("AttackTrigger");
                gold.SetTrigger("AttackTrigger");
            }
            
            attacktime = Time.time + AttackCooldown;
        }
    }

    void SetLastAttackPos()
    {
        
    }

    void CastAttackBox(Vector2 direction)
    {
        Vector2 attackPosition = (Vector2)transform.position + direction * attackRange;
        RaycastHit2D hit = Physics2D.BoxCast(attackPosition, boxSize, 0f, direction, 0f, attackMask);

        if (hit.collider != null)
        {
            Debug.Log(hit.collider.name);

            if (hit.collider.gameObject.CompareTag("Enemy"))
            {
                EnemyHealth enemyHealth = hit.collider.gameObject.GetComponent<EnemyHealth>();
                CameraShake.StartShake(0.1f, 0.05f);
                enemyHealth.GiveDamage(Damage);
            }

            if (direction == Vector2.down && !playerController.CheckGrounded())
            {
                OnAirJump();
                if (hit.collider.gameObject.CompareTag("Tower"))
                {
                    CameraShake.StartShake(CamShakeDuration, CamShakeMagnitude);
                    ScoreManager.main.towerPogo = true;
                    ScoreManager.main.ControlScores();
                }
            }
        }
        else
        {
            Debug.Log("YOOOK");
        }

        DebugDrawBox(attackPosition, boxSize, Color.red, 0.5f);
    }

    void OnAirJump()
    {
        playerController.DoPOGO(POGOMultiplier);
        // animator.SetBool("isJumping", true);
    }

    void DebugDrawBox(Vector2 center, Vector2 size, Color color, float duration)
    {
        Vector2 halfSize = size / 2;
        Vector2 topLeft = center + new Vector2(-halfSize.x, halfSize.y);
        Vector2 topRight = center + new Vector2(halfSize.x, halfSize.y);
        Vector2 bottomLeft = center + new Vector2(-halfSize.x, -halfSize.y);
        Vector2 bottomRight = center + new Vector2(halfSize.x, -halfSize.y);

        Debug.DrawLine(topLeft, topRight, color, duration);
        Debug.DrawLine(topRight, bottomRight, color, duration);
        Debug.DrawLine(bottomRight, bottomLeft, color, duration);
        Debug.DrawLine(bottomLeft, topLeft, color, duration);
    }

    void CalculateDirection()
    {
        if (Yinput > 0f)
        {
            up = true; down = false;
            left = false; right = false;
        }
        else if (Yinput < 0f)
        {
            up = false; down = true;
            left = false; right = false;
        }
        else if (Xinput > 0f)
        {
            left = false; right = true;
            up = false; down = false;
        }
        else if (Xinput < 0f)
        {
            left = true; right = false;
            up = false; down = false;
        }
        else if (Xinput == 0f && Yinput == 0f)
        {
            if (playerController.isFacingRight)
            {
                left = false; right = true;
                up = false; down = false;
            }
            else
            {
                left = true; right = false;
                up = false; down = false;
            }
        }
    }

    void GetInputs()
    {
        Xinput = playerController.inputX;
        Yinput = playerController.inputY;
    }

    Vector3 GetAttackDirection()
    {
        if (up) return Vector2.up;
        if (down) return Vector2.down;
        if (left) return Vector2.left;
        if (right) return Vector2.right;
        return Vector2.zero;
    }
    //animasyonlar için gerekliydi, yazdım (ayt)
    // IEnumerator AttackAnimation()
    // {
    //     animator.SetBool("isAttacking", true);
    //     yield return new WaitForSeconds(0.1f);
    //     animator.SetBool("isAttacking", false);
    // }
}
