using UnityEngine;
using UnityEngine.UI;

public class HealthUIScript : MonoBehaviour
{
    Text Text;

    private void Start()
    {
        Text = GetComponent<Text>();
    }

    void Update()
    {
        Text.text = $"Health: {HealthScript.HealthValue}\nArmor: {HealthScript.HasArmor}";
    }
}
