using UnityEditor;
using UnityEngine;
using System;
using Unity.VisualScripting;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    public GameObject[] SFXPrefab;
    [SerializeField] private int Health = 1;
    [Tooltip("Sadece Eagle Enemy ile çalışır. Aksi takdirde otomatik False olur.")]
    [SerializeField] private bool respawn;
    [HideInInspector] public bool isRespawning;
    Enemy enemyScript;
    BoxCollider2D boxCollider;
    CircleCollider2D circleCollider;
    Vector3 startPos;

    private void Start()
    {
        startPos = transform.position;
        enemyScript = GetComponent<Enemy>();
        boxCollider = GetComponent<BoxCollider2D>();
        circleCollider = GetComponent<CircleCollider2D>();
    }
    public void GiveDamage(int damage)
    {
        PlaySFX();
        checkHealth(damage);
        Health -= damage;
        isRespawning = true;
    }

    void PlaySFX()
    {
        if (SFXPrefab.Length == 0) return;
        int randomIndex = UnityEngine.Random.Range(0, SFXPrefab.Length);
        var sfx = Instantiate(SFXPrefab[randomIndex], transform.position, Quaternion.identity, null);
        sfx.GetComponent<AudioSource>().pitch = UnityEngine.Random.Range(0.9f, 1.1f);
        Destroy(sfx, 5f);
    }

    void checkHealth(float damageToAppend)
    {
        if (Health <= 0 || Health - damageToAppend <= 0)
        {
            if (!respawn)
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
        if (enemyScript.enemydata.enemyType == EnemyType.Eagle)
        {
            boxCollider.enabled = false;
            circleCollider.enabled = false;
            GetComponent<Animator>().SetBool("isVanishing", true);
            yield return new WaitForSeconds(1f);
            GetComponent<Animator>().SetBool("isVanishing", false);
            yield return new WaitForSeconds(0.7f);
            isRespawning = false;
            boxCollider.enabled = true;
            circleCollider.enabled = true;
            Health = 1;
        }
        else if (enemyScript.enemydata.enemyType == EnemyType.Cannon)
        {
            boxCollider.enabled = false;
            circleCollider.enabled = false;
            transform.GetChild(0).GetComponent<Animator>().SetBool("isVanishing", true);
            yield return new WaitForSeconds(2f);
            transform.GetChild(0).GetComponent<Animator>().SetBool("isVanishing", false);
            yield return new WaitForSeconds(0.7f);
            isRespawning = false;
            boxCollider.enabled = true;
            circleCollider.enabled = true;
            Health = 1;
        }
    }

    public void ResetEnemy()
    {
        transform.position = startPos;
    }


}