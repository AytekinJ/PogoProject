using UnityEngine;

public enum FPS
{
    Low = 30,
    Mid = 60,
    High = 144,
    Unlimited = -1
}
public enum AntiAliasing
{
    None,
    FXAA,
    MSAA,
    TAA
}
public enum Quality
{
    Low,
    Medium,
    High
}

[CreateAssetMenu(fileName = "GameSetting", menuName = "Scriptable Objects/GameSetting")]
public class GameSetting : ScriptableObject
{
    [Header("Ekran Ayarlari")]
    [SerializeField] public int rWidth = 1920;
    [SerializeField] public int rHeight = 1080;
    [SerializeField] public FPS fps = FPS.Mid; 
    [SerializeField] public bool vsync = true; 
    [SerializeField] 
    [Header("Grafik Ayarlari")]
    public bool shadows = true;                  
    [SerializeField] public bool postprocessing = true;          
    [SerializeField] public AntiAliasing antialiasing = AntiAliasing.FXAA; 
    [SerializeField] public Quality antialiasingQuality = Quality.Medium;

    [Header("Ses Ayarlari")]
    [Range(0, 100)] public int masterVolume = 100;
    [Range(0, 100)] public int musicVolume = 100;
    [Range(0, 100)] public int sfxVolume = 100;
    
    [Header("Input Ayarlari")]
    [SerializeField] public bool inputEnabled = true;
    [SerializeField] public KeyCode right = KeyCode.D;
    [SerializeField] public KeyCode left = KeyCode.A;
    [SerializeField] public KeyCode up = KeyCode.W;
    [SerializeField] public KeyCode down = KeyCode.S;
    [SerializeField] public KeyCode attack = KeyCode.X;
    [SerializeField] public KeyCode rightAim = KeyCode.RightArrow;
    [SerializeField] public KeyCode leftAim = KeyCode.LeftArrow;
    [SerializeField] public KeyCode upAim = KeyCode.UpArrow;
    [SerializeField] public KeyCode downAim = KeyCode.DownArrow;
    
    
    

    public void ApplySettings()
    {
        Screen.SetResolution(rWidth, rHeight, FullScreenMode.FullScreenWindow);
        QualitySettings.vSyncCount = vsync ? 1 : 0;
        Application.targetFrameRate = (int)fps;
        QualitySettings.antiAliasing = antialiasing == AntiAliasing.MSAA ? 4 : 0;
        Debug.Log("Ayarlar uygulandı!");
    }
}

