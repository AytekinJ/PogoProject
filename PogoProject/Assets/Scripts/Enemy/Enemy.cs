using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Eagle Var.")]
    public bool locked;
    public float dashDelay = 0.3f;
    public float dashSpeed = 10f;

    public EnemyData enemydata;
    public EnemyType type;
    public LayerMask playerlayer;
    public int level, hp;
    public float range;
    void Start()
    {
        enemydata.DefineSpecifies(type, hp, level, range);
    }

    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, range, playerlayer);

        foreach (var hit in hitColliders)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(hit.transform.position, new Vector3(2f,2f,2f));
            Gizmos.DrawLine(transform.position, hit.transform.position);
        }

        #if UNITY_EDITOR
            Handles.color = Color.white;
            Handles.Label(transform.position + Vector3.up, $"Range: {range}");
            Handles.Label(transform.position + Vector3.up*2, $"Locked : {locked}");
#endif
    }
    
}

