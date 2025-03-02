using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformManager : MonoBehaviour
{
    public float maxTriggerDistance = 5;
    public GameObject player;
    public bool canControl = true;
    public static PlatformManager main { get; private set; }
    private Platform[] platforms;
    private HashSet<Platform> activePlatforms = new HashSet<Platform>();

    private void Awake()
    {
        if (main == null)
            main = this;
        else
            Destroy(gameObject);

        platforms = FindObjectsByType<Platform>(FindObjectsSortMode.None);
        foreach (Platform platform in platforms)
        {
            platform.gameObject.SetActive(false);
        }
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Start()
    {
        StartCoroutine(ControlPlatformList());
    }

    public void ActivatePlatform(Platform platform)
    {
        if (!activePlatforms.Contains(platform))
        {
            activePlatforms.Add(platform);
            ParseProcess(platform);
        }
    }

    private void ParseProcess(Platform platform)
    {
        if (platform.breakablePlatform)
        {
            StartCoroutine(BreakablePlatform(platform));
        }
    }

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
            else
            {
                yield return null;
            }
        }
    }

    private Vector3 lastCameraPosition;
    private float checkThreshold = 1f;
    private WaitForSeconds checkDelay = new WaitForSeconds(0.1f);

    private IEnumerator ControlPlatformList()
    {
        while (canControl)
        {
            if (Vector3.Distance(Camera.main.transform.position, lastCameraPosition) >= checkThreshold)
            {
                lastCameraPosition = Camera.main.transform.position;

                foreach (Platform platform in platforms)
                {
                    float distance = Vector3.Distance(player.transform.position, platform.transform.position);

                    if (distance <= maxTriggerDistance)
                    {
                        if (!activePlatforms.Contains(platform))
                        {
                            platform.gameObject.SetActive(true);
                            ActivatePlatform(platform);
                        }
                    }
                    else
                    {
                        if (activePlatforms.Contains(platform))
                        {
                            activePlatforms.Remove(platform);
                            platform.gameObject.SetActive(false);
                        }
                    }
                }
            }
            yield return checkDelay;
        }
    }
}
