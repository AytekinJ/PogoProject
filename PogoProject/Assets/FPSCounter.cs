using UnityEngine;

public class FPSCounter : MonoBehaviour
{
    // FPS görüntüleme ayarları
    public bool showFPS = true;
    public float updateInterval = 0.5f; // FPS değerini ne sıklıkla güncellemek istediğiniz
    public int fontSize = 24;
    public Color textColor = Color.white;
    public Vector2 position = new Vector2(10, 10);

    private float accum = 0; // Toplam frame süresi
    private int frames = 0; // Toplam frame sayısı
    private float timeLeft; // Bir sonraki güncellemeye kalan süre
    private float fps = 0; // Hesaplanan FPS değeri

    private GUIStyle style;

    private void Start()
    {
        timeLeft = updateInterval;

        // GUI stilini ayarla
        style = new GUIStyle();
        style.fontSize = fontSize;
        style.normal.textColor = textColor;
    }

    private void Update()
    {
        // FPS hesaplama
        timeLeft -= Time.deltaTime;
        accum += Time.timeScale / Time.deltaTime;
        frames++;

        // Belirli aralıklarla FPS değerini güncelle
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
            // FPS değerini ekranda göster
            GUI.Label(new Rect(position.x, position.y, 200, 50), "FPS: " + fps.ToString("F2"), style);
        }
    }
}