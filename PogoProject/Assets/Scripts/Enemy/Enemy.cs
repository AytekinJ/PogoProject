using System;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Eagle Var.")]
    public bool locked;
    public bool move;
    public float dashDelay = 0.3f;
    public float afterDashDelay = 0.5f;
    public float dashSpeed = 10f;
    public float range;
    [Space(5f)]
    [Header("Cannon Var.")]
    public bool active;
    public float shootAngle;
    public float shootDelay;
    public float shootRange;
    public float bulletSpeed;
    public GameObject bulletPrefab;
    [Space(5f)]
    [Header("Goomba")]
    public bool activeGoomba;
    public float goombaSpeed;
    public bool collided;
    [Space(5f)]
    
    [Header("Other")]
    public EnemyData enemydata;
    public EnemyType type;
    public LayerMask playerlayer;
    public LayerMask groundlayer;
    public int level, hp;
    void Awake()
    {
        enemydata.DefineSpecifies(type, hp, level, range);
    }

    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        if (type == EnemyType.Eagle)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, range);
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, range, playerlayer);

            foreach (var hit in hitColliders)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireCube(hit.transform.position, new Vector3(2f, 2f, 2f));
                Gizmos.DrawLine(transform.position, hit.transform.position);
            }

#if UNITY_EDITOR
            Handles.color = Color.white;
            Handles.Label(transform.position + Vector3.up, $"Range: {range}");
            Handles.Label(transform.position + Vector3.up * 2, $"Locked : {locked}");
#endif
        }

        if (type == EnemyType.Cannon)
        {
            Gizmos.color = Color.red;

            float angleInRadians = shootAngle * Mathf.Deg2Rad;
            Vector2 direction = new Vector2(Mathf.Cos(angleInRadians), Mathf.Sin(angleInRadians));
            Vector2 endPoint = (Vector2)transform.position + direction * shootRange;

            Gizmos.DrawLine(transform.position, endPoint);

#if UNITY_EDITOR
            Handles.color = Color.white;
            Handles.Label(transform.position + Vector3.up, $"Shoot Range: {shootRange}");
            Handles.Label(transform.position + Vector3.up * 2, $"Shoot Angle: {shootAngle}°");
#endif
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        collided = true;
        
    }
}

