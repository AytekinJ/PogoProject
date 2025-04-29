using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class SceneLoader : MonoBehaviour
{
    [Header("UI Elements")]
    [Tooltip("The UI Slider to represent loading progress.")]
    public Slider progressBar;
    [Tooltip("The TextMeshProUGUI element to display loading messages.")]
    public TextMeshProUGUI infoText;

    [Header("Loading Settings")]
    [Tooltip("Minimum time the loading screen should be displayed (in seconds).")]
    public float minimumLoadTime = 2.0f;
    [Tooltip("How quickly the progress bar visually catches up to the actual load progress.")]
    public float progressBarSpeed = 1.0f;
    [Tooltip("How often to change the funny loading message (in seconds).")]
    public float messageChangeInterval = 2.5f;

    string[] funnyMessages = new string[]
    {
       // ... (mesajlarınız burada) ...
       "Cleaning the mess you made...",
       "Stealing RAM from your neighbor...",
       "Loading... hopefully...",
       "Convincing Unity not to crash...",
       "Preparing coffee for Coffe Man",
       "Coffee Man: FUCKING HELL",
       "Oh sh*t I think I broke something...",
       "Optimizing the optimization...",
       "Wait a minute, our developer is sliding reels...",
       "Listening to Master Of Puppets...",
       "Feeding the hamsters powering this...",
       "Summoning Tony Stark to help...",
       "Calling mom for emotional support...",
       "Rebinding your GPU with duct tape...",
       "Polishing the pixels...",
       "Asking the hamster for more power...",
       "Definitely not mining crypto...",
       "Buffering the buffer...",
       "Adjusting the difficulty to 'Easy' for you...",
       "There were 108 conflicts on Level 3.",
       "BRRUUUUUUUUUUUMMMMMMMM BRRUUUUUUUUUUUUMMMMMMMM",
       #region csm
       "Isogu zattou mo karuku crack na shiten Itemo naitemo tatenu flat na shiten Hazumu biito wa kizamu clap nashi de Wasuretai hodo okoru chain mawashite Subete wo kakikeshite engine on Shita narashi ima Sabitsuke kuroku CHAINSAW BLOOD Chi ga tagite mou nitatte mou yamerenai Kudaki kittemo ugattemo yamanu Hu, hu, hu, hu Tell me why cry? Tell me why? CHAINSAW BLOOD Chi ga tagitte mou nitatte mou tomarenai Chi wo kurau tabi akumu mata yogiru Hu, hu, hu, hu Tell me why cry? Tell me why you grinning? Mousou ni okasareta Unou sousa funou kinou no sousai hou ni Tsukatta ai no te! Hai hai Warakashita bad na scheme wo kutte shimau shinshi Fuwaa to tachikurami kotoba wo yuushi 'Chainsaw is tsukaeru ai no te' 'Ah? Nan datte?' Isogu zattou mo karuku crack na shiten Itemo naitemo tatenu flat na shiten Hazumu biito wa kizamu clap nashi de Wasuretai hodo okoru chain mawashite Subete wo kakikeshite engine on Shita narashi ima Sabitsuke kuroku CHAINSAW BLOOD Chi ga tagite mou nitatte mou yamerenai Kudaki kittemo ugattemo yamanu Hu, hu, hu, hu Tell me why cry? Tell me why? CHAINSAW BLOOD Chi ga tagitte mou nitatte mou tomarenai Chi wo kurau tabi akumu mata yogiru Hu, hu, hu, hu Tell me why cry? Tell me why you grinning?  Mousou ni okasareta Unou sousa funou kinou no sousai hou ni Tsukatta ai no te! Hai hai Warakashita bad na scheme wo kutte shimau shinshi Fuwaa to tachikurami kotoba wo yuushi 'Chainsaw is tsukaeru ai no te' 'Ah? Nan datte?'",
       #endregion
       "Hey. hey God.",
       "Devils never cry...",
       "JACKPOT!",
       "Stealing mechanics from the Hollow Knight...",
       "Stealing everything from the Mario...",
       "El Psy Congroo...",
       "Koichi truly are a reliable guy",
       "Yare yare daze",
       "YES! YES! YES!",
       "NO! NO! NO!",
       "You insignificant F*CK!",
       "Machine, turn back now. The layers of this game are not for your kind. Turn back, or you will be crossing the Will of GOD...",
       "Machine... I will cut you down, break you apart, splay the gore of your profane form across the STARS!",
       "GRRRRIIIIIFFFFIIIIIIIIIIIIIIIITHHHHH",
       "Madeline doesn't look like our main character. Not even close.",
       "Silk Song will come out.",
       "Git Gud",
       "Never Gonna Give You Up",
       "The cake is a lie",
       "Plin Plin Plon...",
       "Also try Minecraft!",
       "Also try Terraria!",
       "Don't you dare go hollow.",
       @"\[T]/",
       "Beer seek seek lest",
       #region  A
       "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA",
        #endregion
        "Forge, I can! Strong, I am!",
    };


    private float timeElapsed = 0f;
    private float lastMessageChangeTime = 0f;

    void Awake()
    {
        if (progressBar == null)
        {
            Debug.LogError("SceneLoader: Progress Bar referansı Inspector'da ayarlanmamış!");
            enabled = false;
            return;
        }
        if (infoText == null)
        {
            Debug.LogError("SceneLoader: Info Text referansı Inspector'da ayarlanmamış!");
            enabled = false;
            return;
        }

        progressBar.value = 0f;
        infoText.text = GetRandomMessage();
        lastMessageChangeTime = Time.time;
    }

    void Start()
    {
        string sceneToLoad = SceneData.SceneToLoad;

        if (string.IsNullOrEmpty(sceneToLoad))
        {
            Debug.LogError("SceneLoader: SceneData.SceneToLoad içinde sahne adı belirtilmemiş! Sahne yüklenemiyor.");
            infoText.text = "Hata: Hedef sahne belirtilmedi!";
            return;
        }

        StartCoroutine(LoadSceneAsync(sceneToLoad));
    }

    IEnumerator LoadSceneAsync(string sceneName)
    {
        timeElapsed = 0f;
        progressBar.value = 0f;
        GC.Collect();
        Resources.UnloadUnusedAssets();

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);


        asyncOperation.allowSceneActivation = false;

        while (!asyncOperation.isDone)
        {
            timeElapsed += Time.unscaledDeltaTime;

            float targetProgress = Mathf.Clamp01(asyncOperation.progress / 0.9f);

            progressBar.value = Mathf.MoveTowards(progressBar.value, targetProgress, Time.unscaledDeltaTime * progressBarSpeed);

            if (Time.unscaledTime >= lastMessageChangeTime + messageChangeInterval)
            {
                infoText.text = GetRandomMessage();
                lastMessageChangeTime = Time.unscaledTime;
            }

            if (asyncOperation.progress >= 0.9f && Mathf.Approximately(progressBar.value, 1.0f) && timeElapsed >= minimumLoadTime)
            {
                infoText.text = "Finalizing..."; 
                asyncOperation.allowSceneActivation = true; 
            }

            yield return null;
        }
    }

    string GetRandomMessage()
    {
        if (funnyMessages == null || funnyMessages.Length == 0)
        {
            return "Loading..."; 
        }
        int randomIndex = UnityEngine.Random.Range(0, funnyMessages.Length);
        return funnyMessages[randomIndex];
    }

    public static void ReloadCurrentScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        Debug.Log($"Sahne yeniden yükleniyor: {currentSceneName}");


        SceneData.SceneToLoad = currentSceneName;

        SceneManager.LoadScene("LoadingScene", LoadSceneMode.Single);
    }
}

public static class SceneData
{
    public static string SceneToLoad = "";

    public static void LoadScene()
    {
        SceneManager.LoadScene("LoadingScene", LoadSceneMode.Single); // LoadingScene'in adının bu olduğundan emin olun
    }

    public static bool isLevelSelection = false;
}