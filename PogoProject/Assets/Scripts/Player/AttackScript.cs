using System;
using System.Collections;
using UnityEngine;

public class AttackScript : MonoBehaviour
{
    public float POGOMultiplier = 10f;

    float Xinput, Yinput;

    public KeyCode AttackKey = KeyCode.X;
    public bool up, down, left, right;

    public float AttackCooldown = 0.5f;
    private float attacktime = 0f;

    public float attackRange = 0.75f;
    public Vector2 boxSize = new Vector2(0.75f, 0.75f);
    public LayerMask attackMask;

    private Controller playerController;
    Animator animator;

    void Start()
    {
        playerController = GetComponent<Controller>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        GetInputs();
        CalculateDirection();
        AppendAttack();
    }

    void AppendAttack()
    {
        if (Input.GetKeyDown(AttackKey) && Time.time >= attacktime)
        {
            Vector3 attackDirection = GetAttackDirection();

            if (attackDirection == Vector3.down && playerController.CheckGrounded())
            {
                return;
            }

            if (attackDirection != Vector3.zero)
            {
                CastAttackBox(attackDirection);
                //animasyonlar için gerekliydi, yazdım (ayt)
                StartCoroutine(AttackAnimation());
            }

            attacktime = Time.time + AttackCooldown;
        }
    }

    void CastAttackBox(Vector2 direction)
    {
        Vector2 attackPosition = (Vector2)transform.position + direction * attackRange;
        RaycastHit2D hit = Physics2D.BoxCast(attackPosition, boxSize, 0f, direction, 0f, attackMask);

        if (hit.collider != null)
        {
            Debug.Log(hit.collider.name);

            if (direction == Vector2.down && !playerController.CheckGrounded())
            {
                OnAirJump();
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
    }

    void GetInputs()
    {
        Xinput = Input.GetAxisRaw("Horizontal");
        Yinput = Input.GetAxisRaw("Vertical");
    }

    Vector3 GetAttackDirection()
    {
        if (up) return Vector3.up;
        if (down) return Vector3.down;
        if (left) return Vector3.left;
        if (right) return Vector3.right;
        return Vector3.zero;
    }
    //animasyonlar için gerekliydi, yazdım (ayt)
    IEnumerator AttackAnimation()
    {
        animator.SetBool("isAttacking", true);
        yield return new WaitForSeconds(0.1f);
        animator.SetBool("isAttacking", false);
    }
}
