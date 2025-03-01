using System;
using UnityEngine;

public class AttackScript : MonoBehaviour
{
    float Xinput, Yinput;

    public KeyCode AttackKey = KeyCode.X;
    public bool up, down, left, right;

    public float AttackCooldown = 0.5f;
    private float attacktime = 0f;

    public float attackRange = 1.5f;
    public LayerMask attackMask;

    private Controller playerController;

    void Start()
    {
        playerController = GetComponent<Controller>();
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
                CastAttackRay(attackDirection);
            }

            attacktime = Time.time + AttackCooldown;
        }
    }

    void CastAttackRay(Vector2 direction)
    {
        Vector2 attackPosition = (Vector2)transform.position + direction;

        RaycastHit2D hit = Physics2D.Raycast(attackPosition, direction, attackRange, attackMask);

        if (hit.collider != null)
        {
            Debug.Log(hit.collider.name);
        }
        else
        {
            Debug.Log("Yooook");
        }

        Debug.DrawRay(attackPosition, direction * attackRange, Color.red, 0.5f);
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
}
