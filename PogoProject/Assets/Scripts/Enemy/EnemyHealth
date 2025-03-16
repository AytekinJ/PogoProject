using UnityEditor;
using UnityEngine;
using System;

public class EnemyHealth : MonoBehavior
{
    [serializedfield] int Health = 1
    
    public void GiveDamage(float damage)
    {
       checkHealth(damage);
       Health -= damage;
    }
    
    void checkHealth(float damageToAppend)
    {
       if (Health <= 0 || Health - damageToAppend <= 0)
       {
          Destroy(gameObject);
       }
       else
         return;
    }
}