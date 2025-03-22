using UnityEditor;
using UnityEngine;
using System;
using Unity.VisualScripting;

public class EnemyHealth : MonoBehaviour
{
   [SerializeField] private int Health = 1;
    
    public void GiveDamage(int damage)
    {
       checkHealth(damage);
       Health -= damage;
    }
    
    void checkHealth(float damageToAppend)
    {
       if (Health <= 0 || Health - damageToAppend <= 0)
       {
          EnemyManager.deactivateEnemy(GetComponent<Enemy>());
          Destroy(gameObject);
       }
       else
         return;
    }
}