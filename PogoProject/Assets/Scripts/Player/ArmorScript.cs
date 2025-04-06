using UnityEngine;

public class ArmorScript : MonoBehaviour
{
    [SerializeField] bool GiveArmor = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            DecideArmor();
        }
        else
        {
            return;
        }
    }

    void DecideArmor()
    {
        if (GiveArmor)
        {
            HealthScript.AddArmor();
        }
        else if (!GiveArmor)
        {
            HealthScript.RemoveArmor();
        }
    }
}
