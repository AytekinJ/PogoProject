using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    private const float MAX_TOLERANCE = 0.01f;
    [SerializeField] Enemy[] enemiesInScene;
    [SerializeField] HashSet<Enemy> activeEnemies;
    private void Awake()
    {
        activeEnemies = new HashSet<Enemy>();
        enemiesInScene = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
    }

    private void Start()
    {
        foreach (Enemy obj in enemiesInScene)
        {
            ActivateEnemy(obj);
        }
    }
    private void ActivateEnemy(Enemy obj)
    {
        switch (obj.enemydata.enemyType)
        {
            case EnemyType.Eagle:
                activeEnemies.Add(obj);
                StartCoroutine(Eagle(obj));
                break;
            case EnemyType.Cannon:
                activeEnemies.Add(obj);
                StartCoroutine(Cannon(obj));
                break;
            case EnemyType.Goomba:
                break;
            default: Debug.LogError("Invalid type of enemy : "+obj.enemydata.enemyType);
                break;
        }
    }
    private IEnumerator Eagle(Enemy eagle)
    {
        Collider2D target = null;
        Vector3 targetPos;
        while (activeEnemies.Contains(eagle))
        {
            if (!eagle.locked)
                target = Physics2D.OverlapCircle(eagle.transform.position, eagle.enemydata.range, eagle.playerlayer);
            else
                yield return null;

            if (target != null)
            {
                eagle.locked = true;
                targetPos = target.transform.position;
                LineRenderer line = eagle.GetComponent<LineRenderer>();
                if (line == null)
                {
                    line = eagle.gameObject.AddComponent<LineRenderer>();
                    line.startWidth = 0.1f;
                    line.endWidth = 0.1f;
                    line.material = new Material(Shader.Find("Sprites/Default"));
                    line.startColor = Color.red;
                    line.endColor = Color.yellow;
                }
                line.positionCount = 2;
                line.SetPosition(0, eagle.transform.position);
                line.SetPosition(1, targetPos);
                yield return new WaitForSeconds(eagle.dashDelay);
                yield return StartCoroutine(EagleDash(eagle, targetPos, line));
                yield return new WaitForSeconds(eagle.afterDashDelay);
                line.positionCount = 0;
            }
            yield return null;
        }
        yield return null;
    }

    private IEnumerator EagleDash(Enemy eagle, Vector2 target, LineRenderer line)
    {
        while (Vector2.Distance(eagle.transform.position, target) > MAX_TOLERANCE)
        {
            eagle.transform.position = Vector2.MoveTowards(eagle.transform.position, target, eagle.dashSpeed * Time.deltaTime);
            line.SetPosition(0, eagle.transform.position);
            yield return null;
        }
        eagle.locked = false;
    }

    private IEnumerator Cannon(Enemy cannon)
    {
        while (activeEnemies.Contains(cannon))
        {
            float radians = cannon.shootAngle * Mathf.Deg2Rad;
            Vector2 direction = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians)).normalized;
            Vector2 target = (Vector2)cannon.transform.position + direction * cannon.shootRange;
            StartCoroutine(CannonBullet(Instantiate(cannon.bulletPrefab, cannon.transform.position, Quaternion.identity), target, cannon));
            yield return new WaitForSeconds(cannon.shootDelay);
        }
    }

    private IEnumerator CannonBullet(GameObject bullet, Vector2 target, Enemy cannon)
    {
        while (Vector2.Distance(bullet.transform.position, target) > MAX_TOLERANCE)
        {
            bullet.transform.position = Vector2.MoveTowards(bullet.transform.position, target, cannon.bulletSpeed * Time.deltaTime);
            yield return null;
        }
        Destroy(bullet);
    }
}
