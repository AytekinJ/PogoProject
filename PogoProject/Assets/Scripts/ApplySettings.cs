using System;
using System.Collections.Generic;
using TMPro; // TextMeshPro için gerekli
using UnityEngine;
using UnityEngine.UI; // UI elemanlarý (Button, Slider, Toggle) için gerekli

//-----------------------------------------------------------------------------
// ÖNEMLÝ NOTLAR:
// 1. Bu kodun çalýþmasý için, her bir tuþ atama butonuna (Key Binding Button)
//    aþaðýda tanýmlanan `KeyBindingButton` script'ini eklemeniz ve
//    Inspector'dan `actionToBind` alanýný doðru eyleme ayarlamanýz GEREKÝR.
// 2. `GameSetting` script'inizin ve içindeki `FPS` enum'unun tanýmlý olduðunu,
//    ve `settings` üzerinden eriþilebilir bir Singleton veya
//    ScriptableObject yapýsýnda olduðunu varsayýyoruz.
// 3. Çözünürlük (Resolution) dropdown'ýnýn metinlerinin "1920x1080" gibi
//    "GeniþlikxYükseklik" formatýnda olduðunu varsayýyoruz.
// 4. FPS dropdown'ýndaki metinlerin `FPS` enum isimleriyle (veya "Unlimited" gibi
//    özel bir metinle) eþleþtiðini varsayýyoruz.
//-----------------------------------------------------------------------------

/// <summary>
/// Her bir tuþ atama butonuna eklenmesi gereken yardýmcý script.
/// Hangi eylemin bu buton tarafýndan ayarlandýðýný belirtir.
/// </summary>
/// 

public enum BindingAction
{
    MoveUp, MoveDown, MoveLeft, MoveRight,
    Attack,
    AimUp, AimDown, AimLeft, AimRight,
    DpadUp, DpadDown, DpadLeft, DpadRight
    // Gerekiyorsa baþka eylemler ekleyin
}

public class KeyBindingButton : MonoBehaviour
{
    [SerializeField] GameSetting settings;
    [Tooltip("Bu butonun hangi eylemin tuþunu ayarladýðýný belirtir.")]
    public BindingAction actionToBind;

    // Butonun içindeki TextMeshProUGUI referansýný otomatik bulmak istemiyorsanýz
    // veya farklý bir yerdeyse, buraya sürükleyip býrakabilirsiniz.
    // [SerializeField] private TextMeshProUGUI keyTextComponent;

    // TextMeshProUGUI bileþenini almak için bir yardýmcý property
    public TextMeshProUGUI KeyTextComponent
    {
        get
        {
            // if (keyTextComponent == null) // Eðer manuel referans kullanýlýyorsa
            // {
            //     keyTextComponent = GetComponentInChildren<TextMeshProUGUI>();
            // }
            // return keyTextComponent;
            return GetComponentInChildren<TextMeshProUGUI>(); // Direkt alt objeden bul
        }
    }
}

// --- GameSetting Sýnýfý ve FPS Enum'ý için Örnek Taným (SÝZÝN PROJENÝZDE ZATEN OLMALI) ---
/*
public enum FPS
{
    Unlimited = 0, // Veya -1 gibi özel bir deðer
    Thirty = 30,
    Sixty = 60,
    OneTwenty = 120,
    OneFourtyFour = 144
    // Diðer FPS deðerleri
}

public class GameSetting // : ScriptableObject veya : MonoBehaviour (Singleton)
{
    // Singleton örneði (Projennizde uygun þekilde implemente edilmeli)
    private static GameSetting _instance;
    public static GameSetting Instance
    {
        get
        {
            if (_instance == null)
            {
                // Projenize uygun bir þekilde instance'ý bulma veya oluþturma mantýðý
                Debug.LogError("GameSetting Instance bulunamadý!");
            }
            return _instance;
        }
    }
    // Awake içinde instance'ý ayarlama (Eðer MonoBehaviour ise)
    // void Awake() { if (_instance == null) _instance = this; else Destroy(gameObject); DontDestroyOnLoad(gameObject); }

    // ---- Ayar Alanlarý ----
    // Tuþ Atamalarý (Varsayýlan deðerler atanabilir)
    public KeyCode up = KeyCode.W;
    public KeyCode down = KeyCode.S;
    public KeyCode left = KeyCode.A;
    public KeyCode right = KeyCode.D;
    public KeyCode attack = KeyCode.Mouse0;
    public KeyCode upAim = KeyCode.UpArrow;
    public KeyCode downAim = KeyCode.DownArrow;
    public KeyCode leftAim = KeyCode.LeftArrow;
    public KeyCode rightAim = KeyCode.RightArrow;
    public KeyCode DpadUp = KeyCode.JoystickButton5; // Örnek, kontrolcüye göre deðiþebilir
    public KeyCode DpadDown = KeyCode.JoystickButton7;
    public KeyCode DpadLeft = KeyCode.JoystickButton8;
    public KeyCode DpadRight = KeyCode.JoystickButton6;

    // Ses Seviyeleri
    public int masterVolume = 80;
    public int musicVolume = 80;
    public int sfxVolume = 80;

    // Video Ayarlarý
    public int rWidth = 1920;
    public int rHeight = 1080;
    public FPS fps = FPS.Unlimited;
    public bool postprocessing = true;

    // Ayarlarý Kaydetme/Yükleme fonksiyonlarý burada veya baþka bir manager'da olabilir
    // public void SaveSettings() { ... }
    // public void LoadSettings() { ... }
}
*/
// --- Ana Ayar Uygulama Script'i ---

public class ApplySettings : MonoBehaviour
{
    [Header("Baðlantýlar")]
    [SerializeField] KeyBinderInitializer keys; // Buton listesini içeren script (varsayým)
    [SerializeField] GameSetting settings; // GameSetting referansý (opsiyonel, Instance kullanýlýyorsa gerekmeyebilir)

    [Header("Ses Ayarlarý UI")]
    [SerializeField] Slider masterSlider;
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider sfxSlider;

    [Header("Video Ayarlarý UI")]
    [SerializeField] TMP_Dropdown resolution;
    [SerializeField] TMP_Dropdown fps;
    [SerializeField] Toggle postprocess;

    /// <summary>
    /// UI üzerindeki tuþ atamalarýný GameSetting nesnesine uygular.
    /// Buton sýrasýna deðil, butonlara eklenmiþ KeyBindingButton component'ýna göre çalýþýr.
    /// </summary>
    public void ApplyKeyBindings()
    {
        // --- Gerekli Referans Kontrolleri ---
        if (keys == null || keys.buttons == null)
        {
            Debug.LogError("ApplySettings: KeyBinderInitializer veya buton listesi atanmamýþ!");
            return;
        }
        // 'settings' referansýný kullanýyoruz (senin kodundaki gibi)
        if (settings == null)
        {
            Debug.LogError("ApplySettings: GameSetting referansý (settings) atanmamýþ!");
            return;
        }

        // --- Buton Metinlerinden KeyCode Listesi Oluþturma ---
        List<KeyCode> parsedKeyCodes = new List<KeyCode>();
        Debug.Log("Tuþ atamalarý için KeyCode'lar okunuyor...");

        foreach (Button button in keys.buttons)
        {
            // Butonun altýndaki TextMeshPro bileþenini bul
            TextMeshProUGUI keyText = button.GetComponentInChildren<TextMeshProUGUI>();

            if (keyText == null)
            {
                Debug.LogError($"Buton '{button.name}' içinde TextMeshProUGUI bulunamadý! Bu tuþ için atama yapýlamýyor. Varsayýlan KeyCode.None kullanýlýyor.");
                // Hata durumunda listeye geçersiz veya varsayýlan bir deðer ekle
                parsedKeyCodes.Add(KeyCode.None);
                continue; // Sonraki butona geç
            }

            string keyString = keyText.text;
            KeyCode keyCodeResult;

            // Metni KeyCode'a dönüþtürmeyi dene (TryParse ile güvenli)
            // true parametresi büyük/küçük harf duyarsýz dönüþüm saðlar
            if (System.Enum.TryParse<KeyCode>(keyString, true, out keyCodeResult))
            {
                // Baþarýlý olursa listeye ekle
                parsedKeyCodes.Add(keyCodeResult);
            }
            else
            {
                // Baþarýsýz olursa hata logla ve varsayýlan/geçersiz deðer ekle
                Debug.LogError($"Geçersiz KeyCode metni '{keyString}' (Buton: {button.name}). Varsayýlan KeyCode.None kullanýlýyor.");
                parsedKeyCodes.Add(KeyCode.None);
            }
        }

        // --- Ýndeks Tabanlý Atama (KIRILGAN YAPI!) ---
        // Beklenen buton sayýsýný kontrol et (hatanýn erken fark edilmesi için)
        // Orijinal kodunda 13 atama vardý.
        const int expectedButtonCount = 13;
        if (parsedKeyCodes.Count < expectedButtonCount)
        {
            Debug.LogError($"Tuþ atama buton sayýsý beklenenden az ({parsedKeyCodes.Count}/{expectedButtonCount})! Ýndeks tabanlý atamalar yapýlamýyor.");
            return; // Hatalý atama yapmamak için çýk
        }

        Debug.Log($"Okunan {parsedKeyCodes.Count} KeyCode, GameSetting'e indeks tabanlý atanýyor...");

        // DÝKKAT: Aþaðýdaki atamalar tamamen butonlarýn 'keys.buttons' listesindeki
        // SIRASINA baðýmlýdýr! (0: up, 1: down, 2: left, ...)
        settings.up = parsedKeyCodes[0];
        settings.down = parsedKeyCodes[1];
        settings.left = parsedKeyCodes[2];
        settings.right = parsedKeyCodes[3];
        settings.attack = parsedKeyCodes[4];
        settings.upAim = parsedKeyCodes[5];
        settings.downAim = parsedKeyCodes[6];
        settings.leftAim = parsedKeyCodes[7];
        settings.rightAim = parsedKeyCodes[8];
        settings.DpadUp = parsedKeyCodes[9];   // Joystick/Dpad Up
        settings.DpadDown = parsedKeyCodes[10]; // Joystick/Dpad Down
        settings.DpadLeft = parsedKeyCodes[11]; // Joystick/Dpad Left
        settings.DpadRight = parsedKeyCodes[12];// Joystick/Dpad Right

        Debug.Log("Tuþ atamalarý (indeks tabanlý) tamamlandý.");
    }

    /// <summary>
    /// Slider'lardaki ses seviyelerini GameSetting nesnesine uygular.
    /// </summary>
    public void ApplySoundLevels()
    {
        if (settings == null) { Debug.LogError("GameSetting Instance bulunamadý!"); return; }

        // Slider deðeri (0.0-1.0 aralýðýnda) * 100 ile 0-100 aralýðýnda int'e çevrilir.
        settings.masterVolume = Mathf.RoundToInt(masterSlider.value * 100f);
        settings.musicVolume = Mathf.RoundToInt(musicSlider.value * 100f);
        settings.sfxVolume = Mathf.RoundToInt(sfxSlider.value * 100f);
        // Debug.Log($"Ses seviyeleri uygulandý: Master={settings.masterVolume}, Music={settings.musicVolume}, SFX={settings.sfxVolume}");
    }

    /// <summary>
    /// Dropdown ve Toggle'daki video ayarlarýný GameSetting nesnesine uygular.
    /// </summary>
    public void ApplyVideo()
    {
        if (settings == null) { Debug.LogError("GameSetting Instance bulunamadý!"); return; }

        // --- Çözünürlük Ayarý ---
        string selectedResolutionText = resolution.options[resolution.value].text;
        string[] dimensions = selectedResolutionText.Split('x'); // "1920x1080" -> ["1920", "1080"]

        int width, height;
        if (dimensions.Length == 2 &&
            int.TryParse(dimensions[0].Trim(), out width) &&
            int.TryParse(dimensions[1].Trim(), out height))
        {
            settings.rWidth = width;
            settings.rHeight = height;
            // Debug.Log($"Çözünürlük ayarlandý: {width}x{height}");
        }
        else
        {
            Debug.LogError($"Geçersiz çözünürlük metni: '{selectedResolutionText}'. Varsayýlan kullanýlýyor (1920x1080).");
            // Varsayýlan veya bilinen bir çözünürlüðe geri dönülebilir
            settings.rWidth = 1920;
            settings.rHeight = 1080;
        }

        // --- FPS Ayarý ---
        string selectedFpsText = fps.options[fps.value].text;
        FPS fpsResult;

        // "Unlimited" gibi özel metinleri veya doðrudan sayýsal deðerleri kontrol et
        if (selectedFpsText.Equals("Unlimited", StringComparison.OrdinalIgnoreCase)) // Büyük/küçük harf duyarsýz karþýlaþtýrma
        {
            settings.fps = FPS.Unlimited;
            // Debug.Log("FPS Limiti: Unlimited");
        }
        // FPS enum'unda tanýmlý bir deðere parse etmeyi dene
        else if (System.Enum.TryParse<FPS>(selectedFpsText, true, out fpsResult)) // true: büyük/küçük harf duyarsýz
        {
            settings.fps = fpsResult;
            // Debug.Log($"FPS Limiti ayarlandý: {fpsResult}");
        }
        // Doðrudan sayýya parse etmeyi dene (eðer enum isimleri yerine sayýlar varsa)
        else if (int.TryParse(selectedFpsText, out int fpsIntValue) && Enum.IsDefined(typeof(FPS), fpsIntValue))
        {
            settings.fps = (FPS)fpsIntValue;
            // Debug.Log($"FPS Limiti ayarlandý (int): {fpsIntValue}");
        }
        else
        {
            Debug.LogError($"Geçersiz FPS metni: '{selectedFpsText}'. Varsayýlan kullanýlýyor (Unlimited).");
            settings.fps = FPS.Unlimited; // Varsayýlan FPS
        }


        // --- Post-processing Ayarý ---
        settings.postprocessing = postprocess.isOn;
        // Debug.Log($"Post Processing: {settings.postprocessing}");
    }

    /// <summary>
    /// Tüm ayarlarý ilgili metodlarý çaðýrarak uygular.
    /// </summary>
    public void Apply()
    {
        ApplyKeyBindings();
        ApplySoundLevels();
        ApplyVideo();

        Debug.Log("Yeni Ayarlar Uygulandý!");

        // ÝSTEÐE BAÐLI: Ayarlarý hemen diske kaydetmek için
        // if(settings != null)
        // {
        //     // settings.SaveSettings(); // Kaydetme fonksiyonunuzu çaðýrýn
        // }
    }
}