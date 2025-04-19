using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement; // Kaldırıldı (Reload için SceneLoader kullanılıyor)

public class HealthScript : MonoBehaviour
{
    // --- Singleton Deseni ---
    public static HealthScript Instance { get; private set; }
    // ------------------------

    [Header("Durum Değişkenleri (Instance)")]
    public bool HasArmor = false;
    public int HealthValue = 10;
    public float XKnockBack = 0f; // Knockback durumu oyuncuya ait olmalı
    public Transform CurrentCheckpoint = null;
    public Transform CurrentPlatformCheckpoint = null;

    [Header("Referanslar (Inspector'dan atanmalı veya Start'ta bulunmalı)")]
    [SerializeField] private int startingHealth = 10; // Başlangıç canı
    [SerializeField] private GameObject damageSFXPrefab; // Hasar sesi prefabı
    [SerializeField] private Transform worldSpawnPoint; // Dünyanın başlangıç noktası
    [SerializeField] private CameraFadeScript cameraFadeScript; // Kamera fade script'i

    // Diğer script referansları (gerekirse)
    private ResetPlatforms resetScript; // Player üzerinde mi?

    void Awake()
    {
        // --- Singleton Uygulaması ---
        if (Instance == null)
        {
            Instance = this;
            // Opsiyonel: Eğer oyuncu ve HealthScript sahneler arası taşınacaksa:
            // DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Debug.LogWarning("Birden fazla HealthScript bulundu. Bu örnek yok ediliyor.", this);
            Destroy(gameObject);
            return;
        }
        // ----------------------------

        // Başlangıç değerlerini ayarla
        HealthValue = startingHealth;
        HasArmor = false;
        XKnockBack = 0f;
        CurrentCheckpoint = null; // Sahne başında checkpoint olmaz
        CurrentPlatformCheckpoint = null;

        resetScript = GetComponent<ResetPlatforms>(); // Eğer Player üzerindeyse
    }

    void Start()
    {
        // Referansları Start içinde bulmak daha güvenli olabilir
        if (worldSpawnPoint == null)
        {
            GameObject spawnGO = GameObject.FindGameObjectWithTag("WorldSpawnPoint");
            if (spawnGO != null) worldSpawnPoint = spawnGO.transform;
            else Debug.LogError("WorldSpawnPoint etiketiyle nesne bulunamadı!", this);
        }
        if (cameraFadeScript == null)
        {
            Camera mainCam = Camera.main; // Camera.main performans için cachelenebilir
            if (mainCam != null) cameraFadeScript = mainCam.GetComponent<CameraFadeScript>();
            if (cameraFadeScript == null) Debug.LogError("Ana Kamerada CameraFadeScript bulunamadı!", this);
        }
    }

    #region Checkpoint ve Teleport (Instance Metodları)

    public void SetCheckpoint(Transform TransformToSet)
    {
        Debug.Log($"Yeni Checkpoint Ayarlandı: {TransformToSet.name}", TransformToSet);
        CurrentCheckpoint = TransformToSet;
        CurrentPlatformCheckpoint = null; // Normal checkpoint platform checkpoint'ini sıfırlar
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
            // Belki sahneyi yeniden başlatmak daha iyi bir fallback olabilir?
            // SceneLoader.ReloadCurrentScene();
            return;
        }
        StartCoroutine(TeleportCoroutine(worldSpawnPoint.position));
    }

    private IEnumerator TeleportCoroutine(Vector3 targetPosition)
    {
        if (cameraFadeScript != null)
        {
            // Fade out başlat (unfadeAfter = false, çünkü pozisyon değişecek)
            cameraFadeScript.StartFade(0.2f, true, false);
            yield return new WaitForSecondsRealtime(0.2f); // Zaman ölçeğinden bağımsız bekle
        }

        // Pozisyonu değiştir
        transform.position = targetPosition;
        // Fizik motorunu güncellemek için kısa bir bekleme iyi olabilir
        yield return new WaitForFixedUpdate();
        // Velocity'yi sıfırlamak isteyebilirsin
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = Vector2.zero;
        XKnockBack = 0f; // Knockback'i sıfırla

        if (cameraFadeScript != null)
        {
            // Fade in başlat
            cameraFadeScript.StartFade(0.2f,true,true); // UnFadeCoroutine public olmalı veya yeni bir public metod yazılmalı
        }
    }


    #endregion

    #region Health (Instance Metodları)

    public void IncreaseHealth(int HealthInt)
    {
        HealthValue += HealthInt;
        // İsteğe bağlı: Max can kontrolü
        // HealthValue = Mathf.Min(HealthValue, maxHealth);
        Debug.Log($"Can Arttı: {HealthValue}", this);
        // UI Güncellemesi (Event veya doğrudan çağrı ile)
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
        if (HealthValue <= 0) return; // Zaten ölü

        PlayDamageSFX();

        ResetPlatforms.ResetAllPlatforms(); // Static olmayan metod çağrısı
        ResetPlatforms.ResetAllChainsaws();
        
        CameraShake.StartShake(0.1f, 0.05f); // Bu static kalabilir
        if (cameraFadeScript != null) cameraFadeScript.StartDamageFlash(0.1f);

        // Zırh Kontrolü
        if (HasArmor)
        {
            Debug.Log("Zırh hasarı engelledi.", this);
            RemoveArmor(); // Zırhı kaldır
            // Zırh varken özel teleport mantığı (Platform checkpoint vs.)
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
                    TeleportToSpawn();
                }
            }
            // Zırh kaldırıldıktan sonra can azaltma işlemi yapılmaz.
            return;
        }

        // Can Azaltma
        HealthValue -= HealthInt;
        Debug.Log($"Can Azaldı: {HealthValue}", this);
        UpdateHealthUI(); // UI Güncelle

        // Ölüm Kontrolü
        if (HealthValue <= 0)
        {
            Debug.Log("Oyuncu Öldü! Sahne yeniden yükleniyor.", this);
            // Ölüm animasyonu/efekti için bekleme eklenebilir
            // gameObject.SetActive(false); // Oyuncuyu devre dışı bırak
            // yield return new WaitForSecondsRealtime(1.0f); // Kısa bekleme
            Die();
            return; // Ölüm sonrası teleport olmasın
        }

        // Ölmediyse Checkpoint'e Işınla
        if (CurrentCheckpoint != null)
        {
            Teleport(CurrentCheckpoint);
        }
        else
        {
            TeleportToSpawn();
        }
    }

    private void Die()
    {
         // Ölüm işlemleri burada yapılabilir (animasyon, ses vb.)
        Time.timeScale = 1f; // Zamanı normale döndür (eğer durdurulmuşsa)
        SceneLoader.ReloadCurrentScene(); // Sahneyi yeniden yükle
    }

    // Bu metod muhtemelen gereksiz, IncreaseHealth yeterli
    // public void SetHealth(int HealthInt)
    // {
    //     HealthValue = HealthInt;
    //     UpdateHealthUI();
    // }

    private void UpdateHealthUI()
    {
        // Sağlık UI'ını güncellemek için bir event sistemi veya doğrudan çağrı kullanın.
        // Örnek: UIManager.Instance.UpdateHealth(HealthValue, HasArmor);
        // Veya HealthUIScript'e doğrudan referans:
        HealthUIScript healthUI = FindAnyObjectByType<HealthUIScript>(); // Performanslı değil, cachelemek daha iyi
        if (healthUI != null)
        {
            healthUI.UpdateUI(HealthValue, HasArmor); // HealthUIScript'te public UpdateUI metodu olmalı
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
            UpdateHealthUI(); // UI Güncelle
            // Zırh kaldırma efekti/sesi
        }
    }

    public void AddArmor()
    {
        if (!HasArmor)
        {
            Debug.Log("Zırh Giyildi.", this);
            HasArmor = true;
            UpdateHealthUI(); // UI Güncelle
            // Zırh giyme efekti/sesi
        }
    }

    #endregion

    #region KnockBack (Instance Metodları)

    public void PlayerKnockBack(float xPower, float yPower, Transform damageSource)
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb == null) return;

        // Yönü belirle (hasar kaynağına göre ters yönde)
        float direction = Mathf.Sign(transform.position.x - damageSource.position.x);
        XKnockBack = direction * xPower; // XKnockBack durumunu ayarla

        // Y ekseninde anlık kuvvet uygula
        // Var olan Y hızını sıfırlayıp sadece yeni kuvveti eklemek daha kontrollü olabilir
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
        rb.AddForce(new Vector2(0, yPower), ForceMode2D.Impulse);

        StartCoroutine(ReduceKnockbackOverTime()); // Knockback'i zamanla azalt
    }

    // Knockback'i zamanla azaltmak için Coroutine
    private IEnumerator ReduceKnockbackOverTime()
    {
        float timer = 0f;
        float reduceDuration = 0.5f; // Knockback'in azalma süresi (ayarlanabilir)
        float initialKnockback = XKnockBack;

        while (timer < reduceDuration && Mathf.Abs(XKnockBack) > 0.01f)
        {
            // Yatay hızı doğrudan ayarlamak yerine XKnockBack'i azaltıp Move metodunda kullanmak daha iyi
            // Rigidbody hızını doğrudan ayarlarken dikkatli olun, fizik motoruyla çakışabilir.
            // Controller script'i XKnockBack'i zaten kullanıyor.
            XKnockBack = Mathf.Lerp(initialKnockback, 0, timer / reduceDuration);
            timer += Time.deltaTime;
            yield return null;
        }
        XKnockBack = 0f; // Tamamen sıfırla
    }


    // Bu metod Controller script'i içinde zaten var gibi görünüyor.
    // Controller içindeki ReduceXKnockBack kullanılmalı veya buradaki mantık oraya taşınmalı.
    
    public void ReduceXKnockBack()
    {
        // Controller scripti zaten rb.velocity.x içinde XKnockBack'i kullanıyor.
        // Bu yüzden burada sadece XKnockBack değerini azaltmak yeterli olmalı.
        // Controller'daki Move metodu güncellenmiş XKnockBack'i kullanacaktır.
        XKnockBack = Mathf.Lerp(XKnockBack, 0, 10f * Time.deltaTime); // 10f değeri ayarlanabilir
    }
    

    #endregion

    // Eski Hatalı Yeniden Yükleme Metodu Kaldırıldı
    // private static IEnumerator ReloadScene() { ... }
}