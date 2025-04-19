using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    private const float MAX_TOLERANCE = 0.01f;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] Enemy[] enemiesInScene;
    [SerializeField] static HashSet<Enemy> activeEnemies;
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
                activeEnemies.Add(obj);
                StartCoroutine(GoombaCR(obj)); 
           
                break;
            default: 
                break;
        }
    }
    
    #region Enemies
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


                RaycastHit2D hit = Physics2D.Raycast(eagle.transform.position, (targetPos - eagle.transform.position).normalized, Vector3.Distance(eagle.transform.position, targetPos), groundLayer);

                if (hit.collider != null)
                {
                    yield return new WaitForSeconds(eagle.afterDashDelay);
                    continue;
                }
                eagle.GetComponent<Animator>().SetBool("isAttacking",true);
                yield return new WaitForSeconds(eagle.dashDelay);
                yield return StartCoroutine(EagleDash(eagle, targetPos));
                eagle.GetComponent<Animator>().SetBool("isAttacking",false);
                yield return new WaitForSeconds(eagle.afterDashDelay);
            }
            yield return null;
        }
        yield return null;
    }


    private IEnumerator EagleDash(Enemy eagle, Vector2 target)
    {
        while (Vector2.Distance(eagle.transform.position, target) > MAX_TOLERANCE && !eagle.collided)
        {
            eagle.transform.position = Vector2.MoveTowards(eagle.transform.position, target, eagle.dashSpeed * Time.deltaTime);
            yield return null;
        }
        eagle.collided = false;
        eagle.locked = false;
    }
    
    

    private IEnumerator Cannon(Enemy cannon)
    {
        while (activeEnemies.Contains(cannon))
        {
            Vector2 target = (Vector2)cannon.transform.localPosition + (Vector2)cannon.transform.up * cannon.shootRange;
            if(cannon.gameObject.GetComponent<EnemyHealth>().isRespawning == false)
            {
                StartCoroutine(CannonBullet(Instantiate(cannon.bulletPrefab, cannon.transform.position, Quaternion.identity), target, cannon));
                cannon.transform.GetChild(0).GetComponent<Animator>().SetTrigger("Hit");
            }
            
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
    
    
    

    private IEnumerator GoombaCR(Enemy goomba)
    {
        bool direction = false; 

        while (activeEnemies.Contains(goomba))
        {
            yield return StartCoroutine(GoombaMove(goomba, direction));
            direction = !direction;
        }
    }

    private IEnumerator GoombaMove(Enemy goomba, bool direction)
    {
        while (!goomba.collided && activeEnemies.Contains(goomba))
        {
            if (direction)
            {
                goomba.transform.position += new Vector3(goomba.goombaSpeed * Time.deltaTime, 0, 0);
                goomba.GetComponent<SpriteRenderer>().flipX = true;
            }
            else
            {
                goomba.transform.position -= new Vector3(goomba.goombaSpeed * Time.deltaTime, 0, 0);
                goomba.GetComponent<SpriteRenderer>().flipX = false;
            }
            yield return null;
        }
        goomba.collided = false;
    }
    #endregion

    public static void deactivateEnemy(Enemy enemy)
    {
        activeEnemies.Remove(enemy);
    }

    
}
