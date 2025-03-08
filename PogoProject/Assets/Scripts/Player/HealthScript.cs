using UnityEngine;
using UnityEngine.SceneManagement;

public class HealthScript : MonoBehaviour
{
    public static bool HasArmor = false;

    public static int HealthValue = 10;

    private static int StoredHealthValue = 10;

    public static Transform CurrentCheckpoint;

    public static GameObject Player;

    private void Start()
    {
        HealthValue = StoredHealthValue;
        Player = GameObject.FindGameObjectWithTag("Player");
    }

    #region Transforms

    public static void SetCheckpoint(Transform TransformToSet)
    {
        CurrentCheckpoint = TransformToSet;
    }

    #endregion

    #region Health

    public static void IncreaseHealth(int HealthInt)
    {
        HealthValue += HealthInt;
    }

    public static void DecreaseHealth(int HealthInt)
    {
        if (HealthValue <= 0)
        {
            ReloadScene();
        }

        HealthValue -= HealthInt;
        if (HealthValue <= 0)
        {
            ReloadScene();
        }

        Player.transform.position = CurrentCheckpoint.position;
    }

    // Set health to a specific value
    public static void SetHealth(int HealthInt)
    {
        HealthValue = HealthInt;
    }

    private static void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    #endregion

    #region Armor

    public static void RemoveArmor()
    {
        HasArmor = false;
    }

    public static void AddArmor()
    {
        HasArmor = true;
    }

    #endregion
}
