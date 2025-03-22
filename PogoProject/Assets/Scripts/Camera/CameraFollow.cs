using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
	public Vector3 offset = new Vector3(0, 0, -10);
	public float smoothValue = 17f;

	Vector3 velocity = Vector3.zero;
    //-3.5 normal, -0.65 up
    private void Update() {
        Vector3 movePosition = new Vector3(target.position.x, target.position.y) + offset;
		transform.position = Vector3.SmoothDamp(transform.position, movePosition, ref velocity, smoothValue * Time.deltaTime);
        
        
    }
}
