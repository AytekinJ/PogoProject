using UnityEngine;
using UnityEngine.SceneManagement;

public class HealthScript : MonoBehaviour
{
    public GameObject dmgSFX;

    public static GameObject DamageSFX;

    public static GameObject Player;

    public static bool HasArmor = false;

    public static int HealthValue = 10;
    private static int StoredHealthValue = 10;

    public static float XKnockBack;

    public static Transform CurrentCheckpoint;
    public static Transform CurrentPlatformCheckpoint;
    public static GameObject WorldSpawnPoint;

    public static CameraFadeScript cameraFadeScript;
    ResetPlatforms resetScript;

    private void Start()
    {
        HealthValue = StoredHealthValue;
        HasArmor = false;
        Player = GameObject.FindGameObjectWithTag("Player");
        WorldSpawnPoint = GameObject.FindGameObjectWithTag("WorldSpawnPoint");
        resetScript = GetComponent<ResetPlatforms>();

        DamageSFX = dmgSFX;
        cameraFadeScript = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraFadeScript>();
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
            ReloadScene();
            //TeleportToSpawn();
            return;
        }
        cameraFadeScript.StartFade(0.2f, true, true);
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

    static void PlaySFX()
    {
        var sfx = Instantiate(DamageSFX, Player.transform.position, Quaternion.identity, Player.transform);
        sfx.GetComponent<AudioSource>().pitch = UnityEngine.Random.Range(0.9f, 1.2f);
        Destroy(sfx, 3f);
    }

    public static void DecreaseHealth(int HealthInt, string GameobjectTag)
    {
        PlaySFX();
        ResetPlatforms.ResetAllPlatforms();
        CameraShake.StartShake(0.1f, 0.05f);
        cameraFadeScript.StartDamageFlash(0.1f);
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

    #region KnockBack

    public static void PlayerKnockBack(float x, float y, Transform position)
    {
        if (Player == null || Controller.rb == null) return;

        float direction = position.transform.position.x - Player.transform.position.x;

        if (direction > 0f)
        {
            XKnockBack = -x;
        }
        else
        {
            XKnockBack = x;
        }

        Controller.rb.AddForce(new Vector2(0, y), ForceMode2D.Impulse);
    }


    public static void ReduceXKnockBack()
    {
        XKnockBack = Mathf.LerpUnclamped(XKnockBack, 0, 10f * Time.deltaTime);
    }

    #endregion
}
