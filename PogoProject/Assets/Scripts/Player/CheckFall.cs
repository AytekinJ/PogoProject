using UnityEngine;

public class CheckFall : MonoBehaviour
{
    public float[] transforms = new float[2];
    GameObject player;
    [SerializeField] Controller controller;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        controller = player.GetComponent<Controller>();
        transforms[0] = controller.playerRb.linearVelocityY;
        transforms[1] = controller.playerRb.linearVelocityY;
    }

    void Update()
    {
        // Shift previous frame
        transforms[1] = transforms[0];
        // Update current frame
        transforms[0] = controller.playerRb.linearVelocityY;

        //Debug.Log("Current: " + transforms[0]);
        //Debug.Log("Previous: " + transforms[1]);
    }

    //private void OnDrawGizmos()
    //{
    //    if (transforms != null && transforms.Length >= 2 && transforms[0] != null && transforms[1] != null)
    //    {
    //        Gizmos.color = Color.red;
    //        Gizmos.DrawLine(transforms[0], transforms[1]);
    //    }
    //}
}
