using UnityEngine;
using UnityEngine.UI;

public class SettingsPWDP : MonoBehaviour
{
    public Toggle playWithDpadToggle;
    public GameSetting gameSetting;
    public void IsPlayingDpad()
    {
        gameSetting.playWithDpad = playWithDpadToggle.isOn;
    }
}
