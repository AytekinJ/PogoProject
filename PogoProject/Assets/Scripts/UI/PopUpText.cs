using UnityEngine;
using TMPro;

public class PopUpText : MonoBehaviour
{
    public GameSetting gameSetting;

    TMP_Text text;

    void Start()
    {
        text = GetComponent<TMP_Text>();
        text.text = $"Press '{gameSetting.attack}' To Perform Attack";
    }


}
