using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidselBadiKarakterKontrolleri : MonoBehaviour
{
    #region Movement 4 Variables
    float pHorizontal;
    bool groundCheck;
    public float groundCheckRadius = .35f;
    float jumpPover = 10f;
    public GameObject groundCheckPosition;
    public LayerMask groundCheckLayer;
    #endregion

    float speed = 15f;
    Rigidbody2D rb;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void FixedUpdate()
    {
        Movement4();
    }

    private void Update()
    {
        CheckSurfaceForMovement4();
        Jump();
    }

    void Movement4()
    {
        pHorizontal = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(pHorizontal * speed, rb.linearVelocity.y);
    }

    void CheckSurfaceForMovement4()
    {
        groundCheck = Physics2D.OverlapCircle(groundCheckPosition.transform.position, groundCheckRadius, groundCheckLayer);

    }

    void Jump()
    {
        if (groundCheck == true)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPover);
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheckPosition.transform.position, groundCheckRadius);
    }
}
