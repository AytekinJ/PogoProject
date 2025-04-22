using UnityEngine;

public class Enabler : MonoBehaviour
{
    public GameObject[] ObjectsToOpen;

    void Awake()
    {
        foreach (var item in ObjectsToOpen)
        {
            item.SetActive(true);
        }
    }

}
