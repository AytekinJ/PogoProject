using UnityEngine;

public class TextMover : MonoBehaviour
{
    public float Speed = 5f;

    void Update()
    {
        transform.position += new Vector3(0, Speed, 0) * Time.deltaTime;
    }
}
