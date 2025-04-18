using System;
using System.Collections.Generic;
using TMPro; 
using UnityEngine;
using UnityEngine.UI; 
public enum BindingAction
{
    MoveUp, MoveDown, MoveLeft, MoveRight, Attack,
    AimUp, AimDown, AimLeft, AimRight,
    DpadUp, DpadDown, DpadLeft, DpadRight
}

public class KeyBindingButton : MonoBehaviour
{
    [SerializeField] GameSetting settings;
    [Tooltip("Bu butonun hangi eylemin tu�unu ayarlad���n� belirtir.")]
    public BindingAction actionToBind;

    public TextMeshProUGUI KeyTextComponent
    {
        get
        {
            return GetComponentInChildren<TextMeshProUGUI>(); 
        }
    }
}

public class ApplySettings : MonoBehaviour
{
    [Header("Ba�lant�lar")]
    [SerializeField] KeyBinderInitializer keys; 
    [SerializeField] GameSetting settings;
    [SerializeField] MainMenuController mainMenuController;

    [Header("Ses Ayarlar� UI")]
    [SerializeField] Slider masterSlider;
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider sfxSlider;

    [Header("Video Ayarlar� UI")]
    [SerializeField] TMP_Dropdown resolution;
    [SerializeField] TMP_Dropdown fps;
    [SerializeField] Toggle postprocess;

    [Header("Panels")]
    [SerializeField] GameObject generalPanel;
    [SerializeField] GameObject soundPanel;
    [SerializeField] GameObject videoPanel;
    [SerializeField] GameObject otherPanel;

    private void Start() {
        LoadSettings();
    }
    public void ApplyKeyBindings()
    {
    
        if (keys == null || keys.buttons == null)
        {
            Debug.LogError("ApplySettings: KeyBinderInitializer veya buton listesi atanmam��!");
            return;
        }
    
        if (settings == null)
        {
            Debug.LogError("ApplySettings: GameSetting referans� (settings) atanmam��!");
            return;
        }

    
        List<KeyCode> parsedKeyCodes = new List<KeyCode>();
        Debug.Log("Tu� atamalar� i�in KeyCode'lar okunuyor...");

        foreach (Button button in keys.buttons)
        {
      
            TextMeshProUGUI keyText = button.GetComponentInChildren<TextMeshProUGUI>();

            if (keyText == null)
            {
                Debug.LogError($"Buton '{button.name}' i�inde TextMeshProUGUI bulunamad�! Bu tu� i�in atama yap�lam�yor. Varsay�lan KeyCode.None kullan�l�yor.");
                parsedKeyCodes.Add(KeyCode.None);
                continue;
            }

            string keyString = keyText.text;
            KeyCode keyCodeResult;

            if (Enum.TryParse<KeyCode>(keyString, true, out keyCodeResult))
            {
      
                parsedKeyCodes.Add(keyCodeResult);
            }
            else
            {
          
                Debug.LogError($"Ge�ersiz KeyCode metni '{keyString}' (Buton: {button.name}). Varsay�lan KeyCode.None kullan�l�yor.");
                parsedKeyCodes.Add(KeyCode.None);
            }
        }

        const int expectedButtonCount = 13;
        if (parsedKeyCodes.Count < expectedButtonCount)
        {
            Debug.LogError($"Tu� atama buton say�s� beklenenden az ({parsedKeyCodes.Count}/{expectedButtonCount})! �ndeks tabanl� atamalar yap�lam�yor.");
            return;
        }

        Debug.Log($"Okunan {parsedKeyCodes.Count} KeyCode, GameSetting'e indeks tabanl� atan�yor...");

        settings.JumpButton = parsedKeyCodes[0];
        settings.up = parsedKeyCodes[1];
        settings.down = parsedKeyCodes[2];
        settings.left = parsedKeyCodes[3];
        settings.right = parsedKeyCodes[4];
        settings.attack = parsedKeyCodes[5];
        settings.upAim = parsedKeyCodes[6];
        settings.downAim = parsedKeyCodes[7];
        settings.leftAim = parsedKeyCodes[8];
        settings.rightAim = parsedKeyCodes[9];
        settings.DpadUp = parsedKeyCodes[10];  
        settings.DpadDown = parsedKeyCodes[11]; 
        settings.DpadLeft = parsedKeyCodes[12]; 
        settings.DpadRight = parsedKeyCodes[13];

        Debug.Log("Tu� atamalar� (indeks tabanl�) tamamland�.");
    }

    public void ApplySoundLevels()
    {
        if (settings == null) { Debug.LogError("GameSetting Instance bulunamad�!"); return; }

        settings.masterVolume = Mathf.RoundToInt(masterSlider.value * 100f);
        settings.musicVolume = Mathf.RoundToInt(musicSlider.value * 100f);
        settings.sfxVolume = Mathf.RoundToInt(sfxSlider.value * 100f);
      
    }
    public void ApplyVideo()
    {
        if (settings == null) { Debug.LogError("GameSetting Instance bulunamad�!"); return; }

        string selectedResolutionText = resolution.options[resolution.value].text;
        string[] dimensions = selectedResolutionText.Split('x'); 

        int width, height;
        if (dimensions.Length == 2 &&
            int.TryParse(dimensions[0].Trim(), out width) &&
            int.TryParse(dimensions[1].Trim(), out height))
        {
            settings.rWidth = width;
            settings.rHeight = height;

        }
        else
        {
            Debug.LogError($"Ge�ersiz ��z�n�rl�k metni: '{selectedResolutionText}'. Varsay�lan kullan�l�yor (1920x1080).");
 
            settings.rWidth = 1920;
            settings.rHeight = 1080;
        }


        string selectedFpsText = fps.options[fps.value].text;
        FPS fpsResult;


        if (selectedFpsText.Equals("Unlimited", StringComparison.OrdinalIgnoreCase)) 
        {
            settings.fps = FPS.Unlimited;
   
        }
  
        else if (System.Enum.TryParse<FPS>(selectedFpsText, true, out fpsResult))
        {
            settings.fps = fpsResult;
          
        }
      
        else if (int.TryParse(selectedFpsText, out int fpsIntValue) && Enum.IsDefined(typeof(FPS), fpsIntValue))
        {
            settings.fps = (FPS)fpsIntValue;
      
        }
        else
        {
            Debug.LogError($"Ge�ersiz FPS metni: '{selectedFpsText}'. Varsay�lan kullan�l�yor (Unlimited).");
            settings.fps = FPS.Unlimited; 
        }



        settings.postprocessing = postprocess.isOn;

    }

    public void Apply()
    {
        ApplyKeyBindings();
        ApplySoundLevels();
        ApplyVideo();

        Debug.Log("Yeni Ayarlar Uyguland�!");

        if (Controller.Instance != null)
            Controller.Instance.CheckController();
        else
            Debug.Log("Controller Bulunamadı veya 'Menu Scene'deyiz.");


    }

    void LoadSettings()
    {
        if (mainMenuController != null)
        mainMenuController.ActivateAllPanels();
        else
        {
            generalPanel.SetActive(true);
            soundPanel.SetActive(true);
            videoPanel.SetActive(true);
            otherPanel.SetActive(true);
        }
        if (settings == null) { Debug.LogError("GameSetting Instance bulunamad�!"); return; }

        masterSlider.value = settings.masterVolume / 100f;
        musicSlider.value = settings.musicVolume / 100f;
        sfxSlider.value = settings.sfxVolume / 100f;

        resolution.value = resolution.options.FindIndex(option => option.text == $"{settings.rWidth}x{settings.rHeight}");
        fps.value = fps.options.FindIndex(option => {
            if (settings.fps == FPS.Unlimited)
            return option.text.Equals("Unlimited", StringComparison.OrdinalIgnoreCase);
            else
            return option.text.Equals(((int)settings.fps).ToString(), StringComparison.OrdinalIgnoreCase);
        });
        postprocess.isOn = settings.postprocessing;
        if (mainMenuController != null)
        mainMenuController.ActivateFirstPanel(); 
        else
        {
            generalPanel.SetActive(true);
            soundPanel.SetActive(false);
            videoPanel.SetActive(false);
            otherPanel.SetActive(false);

        }
        
    }
}