using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HealthScript : MonoBehaviour
{
    public static HealthScript Instance { get; private set; }

    [Header("Durum Değişkenleri (Instance)")]
    public bool HasArmor = false;
    public int HealthValue = 10;
    public float XKnockBack = 0f;
    public Transform CurrentCheckpoint = null;
    public Transform CurrentPlatformCheckpoint = null;

    [Header("Referanslar (Inspector'dan atanmalı veya Start'ta bulunmalı)")]
    [SerializeField] private int startingHealth = 10;
    [SerializeField] private GameObject damageSFXPrefab;
    [SerializeField] private Transform worldSpawnPoint;
    [SerializeField] private CameraFadeScript cameraFadeScript;

    bool canBeHurt = true;

    private ResetPlatforms resetScript;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Debug.LogWarning("Birden fazla HealthScript bulundu. Bu örnek yok ediliyor.", this);
            Destroy(gameObject);
            return;
        }

        HealthValue = startingHealth;
        HasArmor = false;
        XKnockBack = 0f;
        CurrentCheckpoint = null;
        CurrentPlatformCheckpoint = null;

        resetScript = GetComponent<ResetPlatforms>();
    }

    void Start()
    {
        if (worldSpawnPoint == null)
        {
            GameObject spawnGO = GameObject.FindGameObjectWithTag("WorldSpawnPoint");
            if (spawnGO != null) worldSpawnPoint = spawnGO.transform;
            else Debug.LogError("WorldSpawnPoint etiketiyle nesne bulunamadı!", this);
        }
        if (cameraFadeScript == null)
        {
            Camera mainCam = Camera.main;
            if (mainCam != null) cameraFadeScript = mainCam.GetComponent<CameraFadeScript>();
            if (cameraFadeScript == null) Debug.LogError("Ana Kamerada CameraFadeScript bulunamadı!", this);
        }
    }

    #region Checkpoint ve Teleport (Instance Metodları)

    public void SetCheckpoint(Transform TransformToSet)
    {
        Debug.Log($"Yeni Checkpoint Ayarlandı: {TransformToSet.name}", TransformToSet);
        CurrentCheckpoint = TransformToSet;
        CurrentPlatformCheckpoint = null;
    }

    public void SetPlatformCheckpoint(Transform TransformToSet)
    {
        Debug.Log($"Yeni Platform Checkpoint Ayarlandı: {TransformToSet.name}", TransformToSet);
        CurrentPlatformCheckpoint = TransformToSet;
    }

    public void Teleport(Transform desiredPosTransform)
    {
        if (desiredPosTransform == null)
        {
            Debug.LogWarning("Teleport hedefi null, spawn noktasına gidiliyor.");
            TeleportToSpawn();
            return;
        }
        StartCoroutine(TeleportCoroutine(desiredPosTransform.position));
    }

    public void TeleportToSpawn()
    {
        if (worldSpawnPoint == null)
        {
            Debug.LogError("WorldSpawnPoint ayarlanmamış, teleport yapılamıyor!", this);
            return;
        }
        StartCoroutine(TeleportCoroutine(worldSpawnPoint.position));
    }

    private IEnumerator TeleportCoroutine(Vector3 targetPosition)
    {
        HitParticleScript.Instance.enabled = false;
        if (cameraFadeScript != null)
        {
            cameraFadeScript.StartFade(0.2f, true, false);
            yield return new WaitForSecondsRealtime(0.2f);
        }

        gameObject.transform.SetParent(null);
        transform.position = targetPosition;
        yield return new WaitForFixedUpdate();
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = Vector2.zero;
        XKnockBack = 0f;

        if (cameraFadeScript != null)
        {
            cameraFadeScript.StartFade(0.2f,true,true);
        }
        HitParticleScript.Instance.enabled = true;
    }

    private void Update() {
        if(Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.H))
        {
            IncreaseHealth(5);
        }
    }

    #endregion

    #region Health (Instance Metodları)

    public void IncreaseHealth(int HealthInt)
    {
        HealthValue += HealthInt;
        Debug.Log($"Can Arttı: {HealthValue}", this);
        UpdateHealthUI();
    }

    private void PlayDamageSFX()
    {
        if (damageSFXPrefab != null)
        {
            var sfx = Instantiate(damageSFXPrefab, transform.position, Quaternion.identity);
            AudioSource audioSource = sfx.GetComponent<AudioSource>();
            if (audioSource != null)
            {
                audioSource.pitch = Random.Range(0.9f, 1.2f);
            }
            Destroy(sfx, 3f);
        }
    }

    public void DecreaseHealth(int HealthInt, string damagingObjectTag)
    {
        if (!canBeHurt)
            return;
        StartCoroutine(Invinciblity());

        if (HealthValue <= 0) return;

        PlayDamageSFX();
        StartCoroutine(LatePlatformReset());
        ResetPlatforms.ResetAllEnemies();
        
        
        CameraShake.StartShake(0.1f, 0.05f);
        if (cameraFadeScript != null) cameraFadeScript.StartDamageFlash(0.3f);

        if (HasArmor)
        {
            Debug.Log("Zırh hasarı engelledi.", this);
            RemoveArmor();
            if (damagingObjectTag == "Thrones")
            {
                if (CurrentPlatformCheckpoint != null)
                {
                    Teleport(CurrentPlatformCheckpoint);
                }
                else if (CurrentCheckpoint != null)
                {
                    Teleport(CurrentCheckpoint);
                }
                else
                {
                    Die();
                }
            }
            return;
        }

        HealthValue -= HealthInt;
        Debug.Log($"Can Azaldı: {HealthValue}", this);
        UpdateHealthUI();

        if (HealthValue <= 0)
        {
            Debug.Log("Oyuncu Öldü! Sahne yeniden yükleniyor.", this);
            Die();
            return;
        }

        if (CurrentCheckpoint != null)
        {
            Teleport(CurrentCheckpoint);
        }
        else
        {
            Die();
        }
    }

    private void Die()
    {
        Time.timeScale = 1f;
        SceneLoader.ReloadCurrentScene();
    }

    // public void SetHealth(int HealthInt)
    // {
    //     HealthValue = HealthInt;
    //     UpdateHealthUI();
    // }

    private void UpdateHealthUI()
    {
        HealthUIScript healthUI = FindAnyObjectByType<HealthUIScript>();
        if (healthUI != null)
        {
            healthUI.UpdateUI(HealthValue);
        }
    }

    #endregion

    #region Armor (Instance Metodları)

    public void RemoveArmor()
    {
        if (HasArmor)
        {
            Debug.Log("Zırh Kaldırıldı.", this);
            HasArmor = false;
            UpdateHealthUI();
        }
    }

    public void AddArmor()
    {
        if (!HasArmor)
        {
            Debug.Log("Zırh Giyildi.", this);
            HasArmor = true;
            UpdateHealthUI();
        }
    }

    #endregion

    #region KnockBack (Instance Metodları)

    public void PlayerKnockBack(float xPower, float yPower, Transform damageSource)
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb == null) return;

        float direction = Mathf.Sign(transform.position.x - damageSource.position.x);
        XKnockBack = direction * xPower;

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
        rb.AddForce(new Vector2(0, yPower), ForceMode2D.Impulse);

        StartCoroutine(ReduceKnockbackOverTime());
    }

    private IEnumerator ReduceKnockbackOverTime()
    {
        float timer = 0f;
        float reduceDuration = 0.5f;
        float initialKnockback = XKnockBack;

        while (timer < reduceDuration && Mathf.Abs(XKnockBack) > 0.01f)
        {
            XKnockBack = Mathf.Lerp(initialKnockback, 0, timer / reduceDuration);
            timer += Time.deltaTime;
            yield return null;
        }
        XKnockBack = 0f;
    }

    
    public void ReduceXKnockBack()
    {
        XKnockBack = Mathf.Lerp(XKnockBack, 0, 10f * Time.deltaTime);
    }


    #endregion

    IEnumerator Invinciblity()
    {
        canBeHurt = false;
        yield return new WaitForSeconds(0.2f);
        canBeHurt = true;
    }

    IEnumerator LatePlatformReset()
    {
        yield return new WaitForSeconds(0.2f);
        ResetPlatforms.ResetAllPlatforms();
        ResetPlatforms.ResetAllChainsaws();
    }
}
