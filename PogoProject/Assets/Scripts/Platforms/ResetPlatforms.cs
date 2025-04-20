using System.Collections;
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
            ResetAllChainsaws();
        }
    }
    public static void ResetAllPlatforms()
    {
        NewMovingPlatform[] platforms = FindObjectsByType<NewMovingPlatform>(FindObjectsSortMode.None);
        for(int i = platforms.Length; i > 0; i--)
        {
            platforms[i-1].ResetPlatform();
        }
    }

    public static void ResetAllChainsaws()
    {
        ChainsawScript[] chainsaws = FindObjectsByType<ChainsawScript>(FindObjectsSortMode.None);
        for(int i = chainsaws.Length; i > 0; i--)
        {
            chainsaws[i-1].ResetChainsaw();
        }
    }

    static IEnumerator LateResetPlatform(NewMovingPlatform platform)
    {
        yield return new WaitForSeconds(0.2f);
        platform.ResetPlatform();
    }

    static IEnumerator LateResetChainsaw(ChainsawScript chainsaw)
    {
        yield return new WaitForSeconds(0.2f);
        chainsaw.ResetChainsaw();
    }
}
