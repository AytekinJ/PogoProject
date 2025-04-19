using UnityEngine;
using UnityEngine.UI;

public class HealthUIScript : MonoBehaviour
{
    TMPro.TextMeshProUGUI Text; 

    void Start() {
        Text = GetComponent<TMPro.TextMeshProUGUI>(); 
        UpdateUI(); 

    }

    void Update() {

         UpdateUI();
    }

    public void UpdateUI(int health, bool armor) 
    {
         if (Text != null) {
             Text.text = $"Health: {health}\nArmor: {armor}";
         }
    }
    public void UpdateUI() 
    {
         if (HealthScript.Instance != null && Text != null) {
              Text.text = $"Health: {HealthScript.Instance.HealthValue}\nArmor: {HealthScript.Instance.HasArmor}";
         }
    }
}
