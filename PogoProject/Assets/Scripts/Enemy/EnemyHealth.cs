using UnityEditor;
using UnityEngine;
using System;
using Unity.VisualScripting;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
   [SerializeField] private int Health = 1;
   [Tooltip("Sadece Eagle Enemy ile çalışır. Aksi takdirde otomatik False olur.")]
   [SerializeField] private bool respawn;
   Enemy enemyScript;
   BoxCollider2D boxCollider;
   CircleCollider2D circleCollider;
    private void Start() 
    {
       enemyScript = GetComponent<Enemy>();
       boxCollider = GetComponent<BoxCollider2D>();
       circleCollider = GetComponent<CircleCollider2D>();
    }
    public void GiveDamage(int damage)
    {
       checkHealth(damage);
       Health -= damage;
    }
    
    void checkHealth(float damageToAppend)
    {
       if (Health <= 0 || Health - damageToAppend <= 0)
       {
         if(!respawn)
         {
            EnemyManager.deactivateEnemy(GetComponent<Enemy>());
            Destroy(gameObject);
         }
         else
         {
            StartCoroutine(EnemyRespawn());
         }
          
       }
       else
         return;
    }

    IEnumerator EnemyRespawn()
    {
      if(enemyScript.enemydata.enemyType == EnemyType.Eagle)
      {
         boxCollider.enabled = false;
         circleCollider.enabled = false;
         GetComponent<Animator>().SetBool("isVanishing", true);
         yield return new WaitForSeconds(1f);
         GetComponent<Animator>().SetBool("isVanishing", false);
         yield return new WaitForSeconds(0.7f);
         boxCollider.enabled = true;
         circleCollider.enabled = true;
         Health = 1;
      }
      
    }
}