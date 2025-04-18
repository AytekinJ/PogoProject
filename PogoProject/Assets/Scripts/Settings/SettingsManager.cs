using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] private GameSetting gameSetting;
    void Start()
    {
        gameSetting = GameSetting.Instance;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            //gameSetting.ApplySettings(); // öylesine duruyo
        }
    }
}

