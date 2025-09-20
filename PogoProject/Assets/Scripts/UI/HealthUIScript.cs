using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthUIScript : MonoBehaviour
{
    public TMP_Text Text;

    void Start() {
        Text = GetComponent<TMP_Text>();
        UpdateUI(HealthScript.Instance.HealthValue);
    }



    public void UpdateUI(int health)
    {
         if (Text != null) {
             Text.text = $"Health: {health}";
         }
    }

}
