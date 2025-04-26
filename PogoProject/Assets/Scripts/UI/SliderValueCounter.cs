using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;

public class SliderValueCounter : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private string exposedParam = "MasterVolume";

    public GameSetting gameSetting;

    private void Awake()
    {
        gameSetting = GameSetting.Instance;
        if (slider == null) slider = GetComponent<Slider>();
        if (text == null) text = GetComponentInChildren<TextMeshProUGUI>();

        slider.value = gameSetting.masterVolume;

        slider.onValueChanged.AddListener(delegate { SliderValueChanged(); });
        SliderValueChanged();
    }

    public void SliderValueChanged()
    {
        float percent = slider.value * 100;
        text.text = ((int)percent) + "%";

        if (audioMixer != null)
        {
            float volume = Mathf.Log10(slider.value <= 0.0001f ? 0.0001f : slider.value) * 20f;
            audioMixer.SetFloat(exposedParam, volume);
        }
    }
}
