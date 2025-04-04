using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CamPoint : MonoBehaviour
{
    public float Distance = 5f;
    public bool LockX;
    public bool LockY;

    Camera cam;
    GameObject Player;
    bool HasLocked = false;

    void Start()
    {
        cam = Camera.main;
        Player = GameObject.FindGameObjectWithTag("Player");
        InvokeRepeating(nameof(CheckDistance), 0, 0.1f);
    }

    public void CheckDistance()
    {
        float distance = Vector3.Distance(transform.position, Player.transform.position);

        if (!HasLocked && distance <= Distance)
        {
            GeneralCamera.Instance.LockToTransform(transform, LockX, LockY);
            HasLocked = true;
        }
        else if (HasLocked && distance > Distance)
        {
            GeneralCamera.Instance.UnlockCamera();
            HasLocked = false;
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (Player == null) return;

        Gizmos.DrawLine(transform.position, Player.transform.position);

        float distance = Vector3.Distance(transform.position, Player.transform.position);

        Vector3 midPoint = (transform.position + Player.transform.position) / 2;
        Handles.Label(midPoint, $"Distance: {distance:F2}");
    }
#endif
}