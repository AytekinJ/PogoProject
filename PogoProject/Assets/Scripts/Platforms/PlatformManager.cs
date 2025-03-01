using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformManager : MonoBehaviour
{
    public float maxTriggerDistance = 15;
    public GameObject player;
    public bool canControl = true;
    public static PlatformManager main { get; private set; }
    private Platform[] platforms;
    private List<Platform> activePlatforms = new List<Platform>();

    private void Awake()
    {
        if (main == null)
            main = this;
        else
            Destroy(gameObject);
        
        platforms = FindObjectsByType<Platform>(FindObjectsSortMode.None);
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public void ActivatePlatform(Platform platform)
    {
        activePlatforms.Add(platform);
        ParseProcess(platform);
    }

    private void ParseProcess(Platform platform)
    {
        if (platform.breakablePlatform)
        {
            
        }
    }

    //be careful about optimizing the list because if not, this coroutine will work for forever
    private IEnumerator BreakablePlatform(Platform platform)
    {
        while (activePlatforms.Contains(platform))
        {
            if (platform.isInteracted)
            {
                yield return new WaitForSeconds(platform.duration);
                platform.gameObject.SetActive(false);
                yield return new WaitForSeconds(platform.delay);
                platform.gameObject.SetActive(true);
            }
        }
    }

    
    private Vector3 lastCameraPosition;
    private float checkThreshold = 1f; 

    private IEnumerator controlPlatformList()
    {
        while (canControl)
        {

            if (Vector3.Distance(Camera.main.transform.position, lastCameraPosition) >= checkThreshold)
            {
                lastCameraPosition = Camera.main.transform.position;

                for (int i = 0; i < platforms.Length; i++)
                {
                    float distance = Vector3.Distance(player.transform.position, platforms[i].transform.position);

                    if (distance <= maxTriggerDistance)
                    {
                        if (!activePlatforms.Contains(platforms[i]))
                        {
                            platforms[i].gameObject.SetActive(true);
                            activePlatforms.Add(platforms[i]);
                        }
                    }
                    else
                    {
                        if (activePlatforms.Contains(platforms[i]))
                        {
                            activePlatforms.Remove(platforms[i]);
                            platforms[i].gameObject.SetActive(false);
                        }
                    }
                }
            }
            yield return null;
        }
    }

}
