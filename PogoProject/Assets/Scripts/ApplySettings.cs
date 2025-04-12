using System;
using System.Collections.Generic;
using TMPro; // TextMeshPro i�in gerekli
using UnityEngine;
using UnityEngine.UI; // UI elemanlar� (Button, Slider, Toggle) i�in gerekli

//-----------------------------------------------------------------------------
// �NEML� NOTLAR:
// 1. Bu kodun �al��mas� i�in, her bir tu� atama butonuna (Key Binding Button)
//    a�a��da tan�mlanan `KeyBindingButton` script'ini eklemeniz ve
//    Inspector'dan `actionToBind` alan�n� do�ru eyleme ayarlaman�z GEREK�R.
// 2. `GameSetting` script'inizin ve i�indeki `FPS` enum'unun tan�ml� oldu�unu,
//    ve `settings` �zerinden eri�ilebilir bir Singleton veya
//    ScriptableObject yap�s�nda oldu�unu varsay�yoruz.
// 3. ��z�n�rl�k (Resolution) dropdown'�n�n metinlerinin "1920x1080" gibi
//    "Geni�likxY�kseklik" format�nda oldu�unu varsay�yoruz.
// 4. FPS dropdown'�ndaki metinlerin `FPS` enum isimleriyle (veya "Unlimited" gibi
//    �zel bir metinle) e�le�ti�ini varsay�yoruz.
//-----------------------------------------------------------------------------

/// <summary>
/// Her bir tu� atama butonuna eklenmesi gereken yard�mc� script.
/// Hangi eylemin bu buton taraf�ndan ayarland���n� belirtir.
/// </summary>
/// 

public enum BindingAction
{
    MoveUp, MoveDown, MoveLeft, MoveRight,
    Attack,
    AimUp, AimDown, AimLeft, AimRight,
    DpadUp, DpadDown, DpadLeft, DpadRight
    // Gerekiyorsa ba�ka eylemler ekleyin
}

public class KeyBindingButton : MonoBehaviour
{
    [SerializeField] GameSetting settings;
    [Tooltip("Bu butonun hangi eylemin tu�unu ayarlad���n� belirtir.")]
    public BindingAction actionToBind;

    // Butonun i�indeki TextMeshProUGUI referans�n� otomatik bulmak istemiyorsan�z
    // veya farkl� bir yerdeyse, buraya s�r�kleyip b�rakabilirsiniz.
    // [SerializeField] private TextMeshProUGUI keyTextComponent;

    // TextMeshProUGUI bile�enini almak i�in bir yard�mc� property
    public TextMeshProUGUI KeyTextComponent
    {
        get
        {
            // if (keyTextComponent == null) // E�er manuel referans kullan�l�yorsa
            // {
            //     keyTextComponent = GetComponentInChildren<TextMeshProUGUI>();
            // }
            // return keyTextComponent;
            return GetComponentInChildren<TextMeshProUGUI>(); // Direkt alt objeden bul
        }
    }
}

// --- GameSetting S�n�f� ve FPS Enum'� i�in �rnek Tan�m (S�Z�N PROJEN�ZDE ZATEN OLMALI) ---
/*
public enum FPS
{
    Unlimited = 0, // Veya -1 gibi �zel bir de�er
    Thirty = 30,
    Sixty = 60,
    OneTwenty = 120,
    OneFourtyFour = 144
    // Di�er FPS de�erleri
}

public class GameSetting // : ScriptableObject veya : MonoBehaviour (Singleton)
{
    // Singleton �rne�i (Projennizde uygun �ekilde implemente edilmeli)
    private static GameSetting _instance;
    public static GameSetting Instance
    {
        get
        {
            if (_instance == null)
            {
                // Projenize uygun bir �ekilde instance'� bulma veya olu�turma mant���
                Debug.LogError("GameSetting Instance bulunamad�!");
            }
            return _instance;
        }
    }
    // Awake i�inde instance'� ayarlama (E�er MonoBehaviour ise)
    // void Awake() { if (_instance == null) _instance = this; else Destroy(gameObject); DontDestroyOnLoad(gameObject); }

    // ---- Ayar Alanlar� ----
    // Tu� Atamalar� (Varsay�lan de�erler atanabilir)
    public KeyCode up = KeyCode.W;
    public KeyCode down = KeyCode.S;
    public KeyCode left = KeyCode.A;
    public KeyCode right = KeyCode.D;
    public KeyCode attack = KeyCode.Mouse0;
    public KeyCode upAim = KeyCode.UpArrow;
    public KeyCode downAim = KeyCode.DownArrow;
    public KeyCode leftAim = KeyCode.LeftArrow;
    public KeyCode rightAim = KeyCode.RightArrow;
    public KeyCode DpadUp = KeyCode.JoystickButton5; // �rnek, kontrolc�ye g�re de�i�ebilir
    public KeyCode DpadDown = KeyCode.JoystickButton7;
    public KeyCode DpadLeft = KeyCode.JoystickButton8;
    public KeyCode DpadRight = KeyCode.JoystickButton6;

    // Ses Seviyeleri
    public int masterVolume = 80;
    public int musicVolume = 80;
    public int sfxVolume = 80;

    // Video Ayarlar�
    public int rWidth = 1920;
    public int rHeight = 1080;
    public FPS fps = FPS.Unlimited;
    public bool postprocessing = true;

    // Ayarlar� Kaydetme/Y�kleme fonksiyonlar� burada veya ba�ka bir manager'da olabilir
    // public void SaveSettings() { ... }
    // public void LoadSettings() { ... }
}
*/
// --- Ana Ayar Uygulama Script'i ---

public class ApplySettings : MonoBehaviour
{
    [Header("Ba�lant�lar")]
    [SerializeField] KeyBinderInitializer keys; // Buton listesini i�eren script (varsay�m)
    [SerializeField] GameSetting settings; // GameSetting referans� (opsiyonel, Instance kullan�l�yorsa gerekmeyebilir)

    [Header("Ses Ayarlar� UI")]
    [SerializeField] Slider masterSlider;
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider sfxSlider;

    [Header("Video Ayarlar� UI")]
    [SerializeField] TMP_Dropdown resolution;
    [SerializeField] TMP_Dropdown fps;
    [SerializeField] Toggle postprocess;

    /// <summary>
    /// UI �zerindeki tu� atamalar�n� GameSetting nesnesine uygular.
    /// Buton s�ras�na de�il, butonlara eklenmi� KeyBindingButton component'�na g�re �al���r.
    /// </summary>
    public void ApplyKeyBindings()
    {
        // --- Gerekli Referans Kontrolleri ---
        if (keys == null || keys.buttons == null)
        {
            Debug.LogError("ApplySettings: KeyBinderInitializer veya buton listesi atanmam��!");
            return;
        }
        // 'settings' referans�n� kullan�yoruz (senin kodundaki gibi)
        if (settings == null)
        {
            Debug.LogError("ApplySettings: GameSetting referans� (settings) atanmam��!");
            return;
        }

        // --- Buton Metinlerinden KeyCode Listesi Olu�turma ---
        List<KeyCode> parsedKeyCodes = new List<KeyCode>();
        Debug.Log("Tu� atamalar� i�in KeyCode'lar okunuyor...");

        foreach (Button button in keys.buttons)
        {
            // Butonun alt�ndaki TextMeshPro bile�enini bul
            TextMeshProUGUI keyText = button.GetComponentInChildren<TextMeshProUGUI>();

            if (keyText == null)
            {
                Debug.LogError($"Buton '{button.name}' i�inde TextMeshProUGUI bulunamad�! Bu tu� i�in atama yap�lam�yor. Varsay�lan KeyCode.None kullan�l�yor.");
                // Hata durumunda listeye ge�ersiz veya varsay�lan bir de�er ekle
                parsedKeyCodes.Add(KeyCode.None);
                continue; // Sonraki butona ge�
            }

            string keyString = keyText.text;
            KeyCode keyCodeResult;

            // Metni KeyCode'a d�n��t�rmeyi dene (TryParse ile g�venli)
            // true parametresi b�y�k/k���k harf duyars�z d�n���m sa�lar
            if (System.Enum.TryParse<KeyCode>(keyString, true, out keyCodeResult))
            {
                // Ba�ar�l� olursa listeye ekle
                parsedKeyCodes.Add(keyCodeResult);
            }
            else
            {
                // Ba�ar�s�z olursa hata logla ve varsay�lan/ge�ersiz de�er ekle
                Debug.LogError($"Ge�ersiz KeyCode metni '{keyString}' (Buton: {button.name}). Varsay�lan KeyCode.None kullan�l�yor.");
                parsedKeyCodes.Add(KeyCode.None);
            }
        }

        // --- �ndeks Tabanl� Atama (KIRILGAN YAPI!) ---
        // Beklenen buton say�s�n� kontrol et (hatan�n erken fark edilmesi i�in)
        // Orijinal kodunda 13 atama vard�.
        const int expectedButtonCount = 13;
        if (parsedKeyCodes.Count < expectedButtonCount)
        {
            Debug.LogError($"Tu� atama buton say�s� beklenenden az ({parsedKeyCodes.Count}/{expectedButtonCount})! �ndeks tabanl� atamalar yap�lam�yor.");
            return; // Hatal� atama yapmamak i�in ��k
        }

        Debug.Log($"Okunan {parsedKeyCodes.Count} KeyCode, GameSetting'e indeks tabanl� atan�yor...");

        // D�KKAT: A�a��daki atamalar tamamen butonlar�n 'keys.buttons' listesindeki
        // SIRASINA ba��ml�d�r! (0: up, 1: down, 2: left, ...)
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

        Debug.Log("Tu� atamalar� (indeks tabanl�) tamamland�.");
    }

    /// <summary>
    /// Slider'lardaki ses seviyelerini GameSetting nesnesine uygular.
    /// </summary>
    public void ApplySoundLevels()
    {
        if (settings == null) { Debug.LogError("GameSetting Instance bulunamad�!"); return; }

        // Slider de�eri (0.0-1.0 aral���nda) * 100 ile 0-100 aral���nda int'e �evrilir.
        settings.masterVolume = Mathf.RoundToInt(masterSlider.value * 100f);
        settings.musicVolume = Mathf.RoundToInt(musicSlider.value * 100f);
        settings.sfxVolume = Mathf.RoundToInt(sfxSlider.value * 100f);
        // Debug.Log($"Ses seviyeleri uyguland�: Master={settings.masterVolume}, Music={settings.musicVolume}, SFX={settings.sfxVolume}");
    }

    /// <summary>
    /// Dropdown ve Toggle'daki video ayarlar�n� GameSetting nesnesine uygular.
    /// </summary>
    public void ApplyVideo()
    {
        if (settings == null) { Debug.LogError("GameSetting Instance bulunamad�!"); return; }

        // --- ��z�n�rl�k Ayar� ---
        string selectedResolutionText = resolution.options[resolution.value].text;
        string[] dimensions = selectedResolutionText.Split('x'); // "1920x1080" -> ["1920", "1080"]

        int width, height;
        if (dimensions.Length == 2 &&
            int.TryParse(dimensions[0].Trim(), out width) &&
            int.TryParse(dimensions[1].Trim(), out height))
        {
            settings.rWidth = width;
            settings.rHeight = height;
            // Debug.Log($"��z�n�rl�k ayarland�: {width}x{height}");
        }
        else
        {
            Debug.LogError($"Ge�ersiz ��z�n�rl�k metni: '{selectedResolutionText}'. Varsay�lan kullan�l�yor (1920x1080).");
            // Varsay�lan veya bilinen bir ��z�n�rl��e geri d�n�lebilir
            settings.rWidth = 1920;
            settings.rHeight = 1080;
        }

        // --- FPS Ayar� ---
        string selectedFpsText = fps.options[fps.value].text;
        FPS fpsResult;

        // "Unlimited" gibi �zel metinleri veya do�rudan say�sal de�erleri kontrol et
        if (selectedFpsText.Equals("Unlimited", StringComparison.OrdinalIgnoreCase)) // B�y�k/k���k harf duyars�z kar��la�t�rma
        {
            settings.fps = FPS.Unlimited;
            // Debug.Log("FPS Limiti: Unlimited");
        }
        // FPS enum'unda tan�ml� bir de�ere parse etmeyi dene
        else if (System.Enum.TryParse<FPS>(selectedFpsText, true, out fpsResult)) // true: b�y�k/k���k harf duyars�z
        {
            settings.fps = fpsResult;
            // Debug.Log($"FPS Limiti ayarland�: {fpsResult}");
        }
        // Do�rudan say�ya parse etmeyi dene (e�er enum isimleri yerine say�lar varsa)
        else if (int.TryParse(selectedFpsText, out int fpsIntValue) && Enum.IsDefined(typeof(FPS), fpsIntValue))
        {
            settings.fps = (FPS)fpsIntValue;
            // Debug.Log($"FPS Limiti ayarland� (int): {fpsIntValue}");
        }
        else
        {
            Debug.LogError($"Ge�ersiz FPS metni: '{selectedFpsText}'. Varsay�lan kullan�l�yor (Unlimited).");
            settings.fps = FPS.Unlimited; // Varsay�lan FPS
        }


        // --- Post-processing Ayar� ---
        settings.postprocessing = postprocess.isOn;
        // Debug.Log($"Post Processing: {settings.postprocessing}");
    }

    /// <summary>
    /// T�m ayarlar� ilgili metodlar� �a��rarak uygular.
    /// </summary>
    public void Apply()
    {
        ApplyKeyBindings();
        ApplySoundLevels();
        ApplyVideo();

        Debug.Log("Yeni Ayarlar Uyguland�!");

        // �STE�E BA�LI: Ayarlar� hemen diske kaydetmek i�in
        // if(settings != null)
        // {
        //     // settings.SaveSettings(); // Kaydetme fonksiyonunuzu �a��r�n
        // }
    }
}