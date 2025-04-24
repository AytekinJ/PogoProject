using UnityEngine;

public class CursorHider : MonoBehaviour
{
    public float idleTime = 1.5f;
    private float idleTimer = 0f;
    private Vector3 lastMousePosition;

    void Start()
    {
        lastMousePosition = Input.mousePosition;
        Cursor.visible = true;
    }

    void Update()
    {
        if (Input.mousePosition != lastMousePosition)
        {
            idleTimer = 0f;
            if (!Cursor.visible)
                Cursor.visible = true;
        }
        else
        {
            idleTimer += Time.deltaTime;

            if (idleTimer >= idleTime)
            {
                Cursor.visible = false;
            }
        }

        lastMousePosition = Input.mousePosition;
    }
}
