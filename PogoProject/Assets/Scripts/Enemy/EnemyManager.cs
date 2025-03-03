using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] Enemy[] enemiesInScene;
    private void Awake()
    {
        enemiesInScene = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
    }


    private IEnumerator Eagle(Enemy eagle)
    {
        yield return null;
    }
}
