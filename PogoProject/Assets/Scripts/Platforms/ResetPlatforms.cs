using UnityEngine;

public class ResetPlatforms : MonoBehaviour
{

    void Start()
    {
        
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Y))
        {
            ResetAllPlatforms();
        }
    }
    public static void ResetAllPlatforms()
    {
        NewMovingPlatform[] platforms = FindObjectsByType<NewMovingPlatform>(FindObjectsSortMode.None);
        for(int i = platforms.Length; i > 0; i--)
        {
            Debug.Log("Platform" + i + "resetlendi.");
            platforms[i-1].ResetPlatform();
        }
    }
}
