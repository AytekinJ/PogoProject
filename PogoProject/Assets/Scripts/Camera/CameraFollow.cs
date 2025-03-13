using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
	public Vector3 offset = new Vector3(0, 0, -10);
	public float smooth = 0.125f;

	Vector3 velocity = Vector3.zero;

    private void Update() {
        Vector3 movePosition = new Vector3(target.position.x, -4) + offset;
		transform.position = Vector3.SmoothDamp(transform.position, movePosition, ref velocity, smooth);
    }
}
