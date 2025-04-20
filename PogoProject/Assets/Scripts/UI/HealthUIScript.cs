using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthUIScript : MonoBehaviour
{
    //Text Text; // UnityEngine.UI Text ise using ekleyin
    public TMP_Text Text; // TMP ise using ekleyin

    void Start() {
        Text = GetComponent<TMP_Text>(); // Veya UnityEngine.UI.Text
        UpdateUI(HealthScript.Instance.HealthValue); // Başlangıçta güncelle
        // İdeal olarak HealthScript'teki bir event'e abone olunmalı
    }

    //void Update() {
    //     // Update içinde sürekli güncellemek yerine event kullanmak daha iyi
    //     // Ama şimdilik böyle kalabilir:
    //     UpdateUI();
    //}

    public void UpdateUI(int health) // HealthScript tarafından çağrılabilir hale getirildi
    {
         if (Text != null) {
             Text.text = $"Health: {health}";
         }
    }
    //public void UpdateUI() // Instance üzerinden veri çekmek için
    //{
    //     if (HealthScript.Instance != null && Text != null) {
    //          Text.text = $"Health: {HealthScript.Instance.HealthValue}/*\nArmor: {HealthScript.Instance.HasArmor}*/";
    //     }
    //}
}
