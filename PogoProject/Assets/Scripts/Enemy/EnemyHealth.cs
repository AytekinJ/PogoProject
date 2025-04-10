using UnityEditor;
using UnityEngine;
using System;
using Unity.VisualScripting;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int Health = 1;
    [SerializeField] private bool respawnEagleEnemy;

    public GameObject[] DamageSFX;

    public void GiveDamage(int damage)
    {
       checkHealth(damage);
        PlaySFX();
       Health -= damage;
    }

    public void PlaySFX()
    {
        if (DamageSFX.Length == 0) return;
        int randomIndex = UnityEngine.Random.Range(0, DamageSFX.Length);
        var sfx = Instantiate(DamageSFX[randomIndex], transform.position, Quaternion.identity, gameObject.transform);
        sfx.GetComponent<AudioSource>().pitch = UnityEngine.Random.Range(0.7f, 0.8f);
        Destroy(sfx, 3f);
    }

    void checkHealth(float damageToAppend)
    {
       if (Health <= 0 || Health - damageToAppend <= 0)
       {
         if(!respawnEagleEnemy)
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
      GetComponent<BoxCollider2D>().enabled = false;
      GetComponent<CircleCollider2D>().enabled = false;
      GetComponent<Animator>().SetBool("isVanishing", true);
      yield return new WaitForSeconds(1f);
      GetComponent<Animator>().SetBool("isVanishing", false);
      yield return new WaitForSeconds(0.7f);
      GetComponent<BoxCollider2D>().enabled = true;
      GetComponent<CircleCollider2D>().enabled = true;
      Health = 1;
    }
}