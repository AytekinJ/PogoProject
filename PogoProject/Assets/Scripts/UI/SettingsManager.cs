using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    
}
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
public struct Settings
{
    public int rWidth;
    public int rHeight;
    public int volume;
    public int fps;
    public bool vsync;
    public bool shadows;
    public bool postprocessing;
    public AntiAliasing antialiasing;
    public Quality antialiasingQuality;
}
