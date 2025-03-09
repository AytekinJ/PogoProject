using UnityEngine;
using UnityEngine.SceneManagement;

public class HealthScript : MonoBehaviour
{
    public static GameObject Player;

    public static bool HasArmor = false;

    public static int HealthValue = 10;
    private static int StoredHealthValue = 10;

    public static Transform CurrentCheckpoint;
    public static Transform CurrentPlatformCheckpoint;
    public static GameObject WorldSpawnPoint;


    private void Start()
    {
        HealthValue = StoredHealthValue;
        Player = GameObject.FindGameObjectWithTag("Player");
        WorldSpawnPoint = GameObject.FindGameObjectWithTag("WorldSpawnPoint");
    }

    #region Transforms

    public static void SetCheckpoint(Transform TransformToSet)
    {
        CurrentCheckpoint = TransformToSet;
    }

    public static void SetPlatformCheckpoint(Transform TransformToSet)
    {
        CurrentPlatformCheckpoint = TransformToSet;
    }

    public static void Teleport(Transform CurrentPos, Transform DesiredPos)
    {
        if (CurrentPos == null || DesiredPos == null)
        {
            TeleportToSpawn();
            return;
        }

        CurrentPos.position = DesiredPos.position;
    }

    public static void TeleportToSpawn()
    {
        Player.transform.position = WorldSpawnPoint.transform.position;
    }

    #endregion

    #region Health

    public static void IncreaseHealth(int HealthInt)
    {
        HealthValue += HealthInt;
    }

    public static void DecreaseHealth(int HealthInt, string GameobjectTag)
    {
        if (HasArmor && GameobjectTag == "Thrones" && CurrentPlatformCheckpoint != null)
        {
            Debug.Log("1");
            Teleport(Player.transform, CurrentPlatformCheckpoint);
            RemoveArmor();
            return;
        }
        else if (HasArmor && GameobjectTag == "Thrones" && CurrentPlatformCheckpoint == null)
        {
            Debug.Log("2");
            Teleport(Player.transform, CurrentCheckpoint);
            RemoveArmor();
            return;
        }
        else if (HasArmor && GameobjectTag == "Enemy")
        {
            Debug.Log("3");
            RemoveArmor();
            return;
        }

        Debug.Log("4");

        if (HealthValue <= 0)
        {
            ReloadScene();
        }

        HealthValue -= HealthInt;
        if (HealthValue <= 0)
        {
            ReloadScene();
        }

        Teleport(Player.transform, CurrentCheckpoint);
    }


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
