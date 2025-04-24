using UnityEngine;
using System.Collections;

public class EnableAfter : MonoBehaviour
{
    public GameObject[] ObjectsToOpen;

    public Behaviour[] ComponentsToOpen;

    public float WaitSeconds = 3f;

    void Awake()
    {
        StartCoroutine(Enable());
        StartCoroutine(EnableComponents());
    }

    IEnumerator EnableComponents()
    {
        yield return new WaitForSeconds(WaitSeconds);

        foreach (var item in ComponentsToOpen)
        {
            item.enabled = true;
        }
    }

    IEnumerator Enable()
    {
        yield return new WaitForSeconds(WaitSeconds);

        foreach (var item in ObjectsToOpen)
        {
            item.SetActive(true);
        }
    }
}
