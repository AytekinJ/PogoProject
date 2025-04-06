using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class SliderValueCounter : MonoBehaviour
{
    [SerializeField] Slider slider;
    [SerializeField] TextMeshProUGUI text;

    private void Awake()
    {
        slider = GetComponent<Slider>();
        text = GetComponentInChildren<TextMeshProUGUI>();
        SliderValueChanged();
    }

    public void SliderValueChanged()
    {
        text.text = ((int)(slider.value * 100)).ToString()+"%";
    }
}
