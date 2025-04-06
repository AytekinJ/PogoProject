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
    [SerializeField] private GameObject PlayerObject;
    [SerializeField] private SpriteRenderer sr;
    void Awake()
    {
        enemydata.DefineSpecifies(type, range);
        //optimizasyon
        //aferin böyle böyle öğrencen (arda)
        PlayerObject = PlayerObject == null ? GameObject.FindGameObjectWithTag("Player") : PlayerObject;
        sr = sr == null ? GetComponent<SpriteRenderer>() : sr;
    }

    void Update()
    {
            if(type == EnemyType.Eagle)
            {
                if (PlayerObject.transform.position.x < transform.position.x)
                {
                    sr.flipX = true;
                }
                else
                {
                    sr.flipX = false;
                }
            }
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
            Vector2 endPoint = (Vector2)transform.localPosition + (Vector2)transform.up * shootRange;

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

    private void OnCollisionExit2D(Collision2D other)
    {
        collided = false;
    }

    private void OnDestroy()
    {
        EnemyManager.deactivateEnemy(this);
    }
}

