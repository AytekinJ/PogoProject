using UnityEngine;
using UnityEngine.SceneManagement; // Kaldırılabilir, artık kullanılmıyor

public class EndScreenButtons : MonoBehaviour
{
    public void RestartGame()
    {
        Time.timeScale = 1f; // Oyunun hızını normale döndürdüğünden emin ol
        SceneLoader.ReloadCurrentScene();
    }

    public void LevelSelection()
    {
        Time.timeScale = 1f; // Oyunun hızını normale döndürdüğünden emin ol
        // --- Düzeltilmiş Sahne Yükleme ---
        SceneData.SceneToLoad = "MainMenu"; // Hedef sahne adını ayarla
        SceneData.LoadScene(); // SceneLoader'ı kullan
        // ------------------------------------
    }
}