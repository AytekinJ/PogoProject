using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Internal;
using UnityEngine;
using UnityEngine.UIElements;

public class PlatformManager : MonoBehaviour
{
    [Header("Moving Platform Settings")]
    private const float MAX_TOLERANCE = 0.01f;
    public float interpolationSpeed = 1;
    [Space(3f)]
    
    [Header("Optimization")]
    public float maxTriggerDistance = 5;
    public bool canControl = true;
    [Space(3f)]
    [Header("Other")] 
    public GameObject player;
    public static PlatformManager main { get; private set; }
    [SerializeField] private Platform[] platforms;
    [SerializeField] private HashSet<Platform> activePlatforms = new HashSet<Platform>();
    private Dictionary<Platform,Platform> switchPlatforms = new Dictionary<Platform, Platform>();
    Color notActiveColor = new Color(1f, 1f, 1f, 0.2f); 

    private void Awake()
    {
        if (main == null)
            main = this;
        else
            Destroy(gameObject);

        platforms = FindObjectsByType<Platform>(FindObjectsSortMode.None);
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
        if (platform.movingPlatform)
        {
            StartCoroutine(MovingPlatform(platform));
        }
        if (platform.jumpSwitch)
        {
            if (platform.dominantPlatform)
            {
                    switchPlatforms.Add(platform, platform.matchedPlatform);
                    StartCoroutine(JumpSwitch(platform,platform.matchedPlatform));
            }
            else
            {
                switchPlatforms.Add(platform.matchedPlatform, platform);
                StartCoroutine(JumpSwitch(platform.matchedPlatform,platform));
            }
        }
    }
    
    #region Platform Processes
    #region Breakable Platform
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
    #endregion

    #region Moving Platform
    private IEnumerator MovingPlatform(Platform platform)
    {
        Vector2 startPos, endPos;
        startPos = new Vector2(platform.center.x - platform.moveLength, platform.center.y);
        endPos = new Vector2(platform.center.x + platform.moveLength, platform.center.y);
        Debug.Log(startPos);
        Debug.Log(endPos);
        if (platform.useLength)
        {
            while (activePlatforms.Contains(platform))
            {

                yield return MoveToPosition(platform, endPos);
                yield return MoveToPosition(platform, startPos);
            }
        }
    }

    private IEnumerator MoveToPosition(Platform platform, Vector3 target)
    {
        while (Vector2.Distance(platform.transform.position, target) > MAX_TOLERANCE && activePlatforms.Contains(platform))
        {
            platform.transform.position = Vector2.MoveTowards(platform.transform.position, target, interpolationSpeed * Time.deltaTime);
            if (platform.isInteracted)
            {
                platform.interactingObject?.transform.SetParent(platform.transform);
            }
            yield return null;
        }
    }
#endregion

    #region JumpSwitch

    private IEnumerator JumpSwitch(Platform dominant, Platform match)
    {
        BoxCollider2D dominantCollider = dominant.GetComponent<BoxCollider2D>();
        BoxCollider2D matchCollider = match.GetComponent<BoxCollider2D>();
        Controller playerController = player.GetComponent<Controller>();
        
        dominantCollider.enabled = false;
        matchCollider.enabled = true;

        while (activePlatforms.Contains(dominant) && activePlatforms.Contains(match))
        {
            if (Input.GetKeyDown(KeyCode.Space) && !playerController.hasJumpedDuringCoyote)
            {

                dominantCollider.enabled = !dominantCollider.enabled;
                matchCollider.enabled = !matchCollider.enabled;
                
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
            }

            yield return null;
        }
    }
        
    #endregion
#endregion

    #region Optimization
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
                            Debug.Log($"Platform {platform.name} activated");
                        }
                    }
                    else
                    {
                        if (activePlatforms.Contains(platform))
                        {
                            activePlatforms.Remove(platform);
                            platform.gameObject.SetActive(false);
                            Debug.Log($"Platform {platform.name} deactivated");

                            // Platform devre dışı bırakıldığında collider'ları sıfırla
                            if (platform.jumpSwitch)
                            {
                                BoxCollider2D collider = platform.GetComponent<BoxCollider2D>();
                                if (collider != null)
                                {
                                    collider.enabled = true; // Varsayılan duruma getir
                                }
                            }
                        }
                    }
                }
            }
            yield return checkDelay;
        }
    }
    #endregion
}
