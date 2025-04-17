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

    string[] funnyMessages = new string[] // yes, I don`t have a life
    {
    "Cleaning the mess you made...",
    "Creating elements to make your life easier...",
    "Loading something just to show off...",
    "Actually loading the game...",
    "Summoning Cthulhu to assist...",
    "Stealing RAM from your neighbor...",
    "Compressing the air...",
    "Generating bugs on purpose...",
    "Loading... hopefully...",
    "Aligning chakras...",
    "Convincing Unity not to crash...",
    "Uploading your secrets to the cloud...",
    "Sending memes to NASA...",
    "Cursing every null reference...",
    "Rendering coffee for the dev...",
    "Waiting for someone to press Alt+F4...",
    "Running in circles...",
    "Oh sh*t I think I broke something...",
    "Optimizing the optimization...",
    "Yeeting unused assets into the void...",
    "Wait a minute, our developer is sliding reels...",
    "Yes, we are loading...",
    "Loading... or is it just a dream?",
    "Listening to Master Of Puppets...",
    "Loading... but first, let me take a selfie.",
    "Downloading some RAM, you poor guy...",
    "Pretending to work...",
    "Calculating the square root of your problems...",
    "Sacrificing a goat to fix build errors...",
    "Making up numbers to show progress...",
    "Swearing at the code, gently...",
    "Feeding the hamsters powering this...",
    "Watching cat videos until loading is done...",
    "Smashing bugs with a hammer...",
    "Consulting the AI overlords...",
    "Teleporting bits and bytes...",
    "Injecting caffeine into the main thread...",
    "Patching the spaghetti code...",
    "Taming wild pointers...",
    "Writing a haiku while loading...",
    "Waiting for the pizza delivery...",
    "Whistling at shaders...",
    "Rebooting your common sense...",
    "Calibrating chaos engine...",
    "Summoning Tony Stark to help...",
    "Calling mom for emotional support...",
    "Reticulating splines...",
    "Tickling the heap...",
    "Rebinding your GPU with duct tape...",
    "Performing dark rituals on the stack...",
    "Looking busy...",
    "Sneaking in another 'using System;'...",
    "Calibrating the flux capacitor...",
    "Dividing by zero...",
    "Generating witty loading commentary...",
    "Compiling shaders... or maybe just procrastinating.",
    "Searching for the 'Any' key...",
    "Polishing the pixels...",
    "Reticulating splines...",
    "Asking the hamster for more power...",
    "Waking up the processor...",
    "Trying not to spill the coffee...",
    "Definitely not mining crypto...",
    "Aligning hyperdrive coordinates...",
    "Buffering the buffer...",
    "Are we there yet?",
    "Untangling the spaghetti code...",
    "Loading awesome things...",
    "Making sure background processes look busy...",
    "Adjusting the difficulty to 'Easy' for you...",
    "Summoning digital magic...",
    "Hold on, updating my antivirus..."
    };

private float timeElapsed = 0f;
    private float lastMessageChangeTime = 0f;

    void Awake()
    {
        if (progressBar == null)
        {
            Debug.LogError("SceneLoader: Progress Bar reference not set in the Inspector!");
            enabled = false;
            return;
        }
        if (infoText == null)
        {
            Debug.LogError("SceneLoader: Info Text reference not set in the Inspector!");
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
            Debug.LogError("SceneLoader: No scene name provided in SceneData.SceneToLoad! Cannot load scene.");
            infoText.text = "Error: Target scene not specified!";
            return;
        }

        StartCoroutine(LoadSceneAsync(sceneToLoad));
    }

    IEnumerator LoadSceneAsync(string sceneName)
    {
  
        timeElapsed = 0f;
        progressBar.value = 0f; 


        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);


        asyncOperation.allowSceneActivation = false;

        while (!asyncOperation.isDone)
        {
            timeElapsed += Time.deltaTime;


            float targetProgress = Mathf.Clamp01(asyncOperation.progress / 0.9f);

            progressBar.value = Mathf.MoveTowards(progressBar.value, targetProgress, Time.deltaTime * progressBarSpeed);


            if (Time.time >= lastMessageChangeTime + messageChangeInterval)
            {
                infoText.text = GetRandomMessage();
                lastMessageChangeTime = Time.time;
            }

   
            if (asyncOperation.progress >= 0.9f && progressBar.value >= 1.0f && timeElapsed >= minimumLoadTime)
            {
             
                infoText.text = "Finalizing..."; 
                yield return new WaitForSeconds(0.2f); 

        
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
}

public static class SceneData
{
    public static string SceneToLoad = "";
}

