using UnityEngine;
using TMPro;

public class PopupPogoText : MonoBehaviour
{
    public GameSetting gameSetting;

    TMP_Text text;

    void Start()
    {
        text = GetComponent<TMP_Text>();
        text.text = $"Press '{gameSetting.attack}' while holding '{gameSetting.down}' to perform Pogo";
    }
}
