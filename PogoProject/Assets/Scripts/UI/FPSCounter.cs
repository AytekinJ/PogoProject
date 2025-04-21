using UnityEngine;

public class FPSCounter : MonoBehaviour
{
    public bool showFPS = false;
    public float updateInterval = 0.5f; 
    public int fontSize = 24;
    public Color textColor = Color.white;
    public Vector2 position = new Vector2(10, 10);

    private float accum = 0; 
    private int frames = 0;
    private float timeLeft; 
    private float fps = 0;

    private GUIStyle style;

    private void Start()
    {
        timeLeft = updateInterval;

        style = new GUIStyle();
        style.fontSize = fontSize;
        style.normal.textColor = textColor;
    }

    private void Update()
    {
        timeLeft -= Time.deltaTime;
        accum += Time.timeScale / Time.deltaTime;
        frames++;

   
        if (timeLeft <= 0.0)
        {
            fps = accum / frames;
            timeLeft = updateInterval;
            accum = 0.0f;
            frames = 0;
        }
    }

    private void OnGUI()
    {
        if (showFPS)
        {
            GUI.Label(new Rect(position.x, position.y, 200, 50), "FPS: " + fps.ToString("F2"), style);
        }
    }
}