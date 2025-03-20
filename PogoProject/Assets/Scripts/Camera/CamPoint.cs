using UnityEngine;

public class CamPoint : MonoBehaviour
{
    public float Distance = 5f;

    Camera cam;

    [SerializeField] GameObject Player;

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
            GeneralCamera.LockToTransform(transform);
            HasLocked = true;
        }
        else if (HasLocked && distance > Distance)
        {
            GeneralCamera.UnlockCamera();
            HasLocked = false;
        }
    }

    private void OnDrawGizmos()
    {
        if (Player == null) return;

        Gizmos.DrawLine(transform.position, Player.transform.position);

        float distance = Vector3.Distance(transform.position, Player.transform.position);

        Vector3 midPoint = (transform.position + Player.transform.position) / 2;
        UnityEditor.Handles.Label(midPoint, $"Distance: {distance:F2}");
    }

}