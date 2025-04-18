using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] private GameSetting gameSetting;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            //gameSetting.ApplySettings(); // öylesine duruyo
        }
    }
}

