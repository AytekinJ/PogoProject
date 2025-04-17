using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
public class SceneLoader : MonoBehaviour
{
    public Slider progressBar;
    public TextMeshProUGUI infoText;
    public string sceneToLoad = ":D";

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
    "Sneaking in another 'using System;'..."
    };

    void Start()
{
    StartCoroutine(LoadSceneAsync(SceneManager.GetActiveScene().name));
}

IEnumerator LoadSceneAsync(string sceneName)
{
    float elapsedTime = 0f;
    
    infoText.text = GetRandomMessage();
    Resources.UnloadUnusedAssets();
    GC.Collect();
    yield return new WaitForSeconds(1.5f);
    elapsedTime += 1.5f;

    AsyncOperation op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
    op.allowSceneActivation = false;

    yield return new WaitForSeconds(1f);
    elapsedTime += 1f;

    float targetProgress = 0f;
    while (op.progress < 0.9f)
    {
        targetProgress = Mathf.Clamp01(op.progress / 0.9f);

        // Slider'ı smooth şekilde ilerlet
        while (progressBar.value < targetProgress)
        {
            progressBar.value = Mathf.MoveTowards(progressBar.value, targetProgress, Time.deltaTime * 0.25f);
            yield return null;
        }

        if (UnityEngine.Random.value > 0.5f)
        {
            infoText.text = GetRandomMessage();
            yield return new WaitForSeconds(1.5f);
            elapsedTime += 1.5f;
        }

        yield return null;
    }

    // Son %10 bar için cilalı giriş
    float finalFillTime = 1.5f;
    float t = 0f;
    float start = progressBar.value;

    while (t < finalFillTime)
    {
        t += Time.deltaTime;
        progressBar.value += UnityEngine.Random.Range(0.05f, 0.2f);
        yield return new WaitForSeconds(UnityEngine.Random.Range(0.4f, 0.8f));
        if (UnityEngine.Random.value >= 0.8) infoText.text = GetRandomMessage();
        if (progressBar.value >= 1f)
        {
            progressBar.value = 1f;
            break;
        }
    }


    op.allowSceneActivation = true;

    yield return new WaitForSeconds(2f);
    SceneManager.UnloadSceneAsync("LoadingScene");
}

string GetRandomMessage()
{
    return funnyMessages[UnityEngine.Random.Range(0, funnyMessages.Length)];
}
}
public static class SceneData
{
    public static string SceneToLoad = "";
}

