using UnityEngine;

public enum FPS { Low = 30, Medium = 60, High = 144, Unlimited = -1 }
public enum AntiAliasing { None, FXAA, MSAA }
public enum Quality { Low, Medium, High }

[CreateAssetMenu(fileName = "GameSetting", menuName = "Settings/GameSetting")]
public class GameSetting : ScriptableObject
{
    private static GameSetting _instance;
    public static GameSetting Instance
    {
        get
        {
            if (_instance == null)
                _instance = Resources.Load<GameSetting>("GameSetting");
            return _instance;
        }
        set => _instance = value;
    }

    [Header("Resolution & Graphics")]
    public int rWidth = 1920;
    public int rHeight = 1080;
    public FPS fps = FPS.High;
    //public bool vsync = true;
    //public bool shadows = true;
    //public bool postprocessing = true;
    //public AntiAliasing antialiasing = AntiAliasing.FXAA;
    //public Quality antialiasingQuality = Quality.High;

    [Header("Audio")]
    [Range(0, 100)] public int masterVolume = 100;
    [Range(0, 100)] public int musicVolume = 100;
    [Range(0, 100)] public int sfxVolume = 100;

    [Header("Input")]
    public bool inputEnabled = true;
    public bool playWithDpad;
    public KeyCode right = KeyCode.D;
    public KeyCode left = KeyCode.A;
    public KeyCode up = KeyCode.W;
    public KeyCode down = KeyCode.S;
    public KeyCode attack = KeyCode.X;
    //public KeyCode rightAim = KeyCode.RightArrow;
    //public KeyCode leftAim = KeyCode.LeftArrow;
    //public KeyCode upAim = KeyCode.UpArrow;
    //public KeyCode downAim = KeyCode.DownArrow;
    public KeyCode JumpButton = KeyCode.Space;
    //public KeyCode DpadUp = KeyCode.JoystickButton4;
    //public KeyCode DpadDown = KeyCode.JoystickButton6;
    //public KeyCode DpadLeft = KeyCode.JoystickButton7;
    //public KeyCode DpadRight = KeyCode.JoystickButton5;
}


[System.Serializable]
public class GameSettingData
{
    public int rWidth, rHeight;
    public FPS fps;
    //public bool vsync, shadows, postprocessing;
    //public AntiAliasing antialiasing;
    //public Quality antialiasingQuality;
    public int masterVolume, musicVolume, sfxVolume;
    public bool inputEnabled;
    public KeyCode right, left, up, down, attack, JumpButton;
    //public KeyCode rightAim, leftAim, upAim, downAim;
    //public KeyCode JumpButton, DpadUp, DpadDown, DpadLeft, DpadRight;
}