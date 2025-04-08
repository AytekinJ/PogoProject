using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq; // LINQ kullanmak için (opsiyonel ama kullanışlı)

public class KeyBinderInitializer : MonoBehaviour
{
    // !!! DİKKAT: Butonları Inspector'da AŞAĞIDAKİ SIRAYLA atayın !!!
    // 0: Up, 1: Right, 2: Left, 3: Down, 4: Attack,
    // 5: AimUp, 6: AimRight, 7: AimLeft, 8: AimDown
    [SerializeField] private List<Button> buttons;
    [SerializeField] private GameSetting settings; // GameSetting ScriptableObject veya component'ini atayın

    // Ayarlardan alınacak KeyCode'ları tutacak liste (sıralı)
    private List<KeyCode> targetKeyCodes = new List<KeyCode>();

    void Start()
    {
        if (settings == null)
        {
            Debug.LogError("KeyBinderInitializer: GameSetting referansı atanmamış!", this);
            return;
        }

        // Gerekli buton sayısını kontrol et (örneğin 9)
        const int expectedButtonCount = 9;
        if (buttons == null || buttons.Count != expectedButtonCount)
        {
            Debug.LogError($"KeyBinderInitializer: Lütfen Inspector'da tam olarak {expectedButtonCount} adet buton atayın! (Sırasıyla: Up, Right, Left, Down, Attack, AimUp, AimRight, AimLeft, AimDown)", this);
            return;
        }

        // Ayarlardaki KeyCode'ları doğru sırayla listeye doldur
        PopulateTargetKeyCodes();

        // Butonları döngü ile işle
        for (int i = 0; i < buttons.Count; i++)
        {
            Button currentButton = buttons[i];
            KeyCode targetKey = targetKeyCodes[i]; // Bu index'e karşılık gelen KeyCode

            if (currentButton == null)
            {
                Debug.LogWarning($"KeyBinderInitializer: Listenin {i}. indeksindeki buton atanmamış/eksik.");
                continue; // Sonraki butona geç
            }

            // Buton üzerinde KeyBinder scriptini bul
            KeyBinder keyBinder = currentButton.GetComponent<KeyBinder>();

            if (keyBinder != null)
            {
                // KeyBinder bulunduysa, ona InitializeKey metodu ile doğru tuşu ata
                keyBinder.InitializeKey(targetKey);
                // KeyBinder scripti bulunamadıysa, en azından text'i direkt ayarlayalım
                // (Ama bu durumda butona tıklayınca tuş atama çalışmaz)
                TextMeshProUGUI buttonText = currentButton.GetComponentInChildren<TextMeshProUGUI>();
                if (buttonText != null)
                {
                    buttonText.text = targetKey.ToString();
                }
                else
                {
                    Debug.LogWarning($"KeyBinderInitializer: Buton '{currentButton.name}' üzerinde KeyBinder scripti ve TextMeshProUGUI bulunamadı!", currentButton);
                }
            }
        }

        Debug.Log("KeyBinderInitializer: Butonlar başarıyla başlatıldı.");
    }

    // GameSetting objesinden KeyCode'ları alır ve sıralı listeye ekler
    private void PopulateTargetKeyCodes()
    {
        targetKeyCodes.Clear(); // Listeyi temizle (yeniden başlatma durumları için)

        // Ayarlardaki KeyCode'ları belirlenen sıraya göre ekle
        targetKeyCodes.Add(settings.up);      // Index 0
        targetKeyCodes.Add(settings.right);   // Index 1
        targetKeyCodes.Add(settings.left);    // Index 2
        targetKeyCodes.Add(settings.down);    // Index 3
        targetKeyCodes.Add(settings.attack);  // Index 4
        targetKeyCodes.Add(settings.upAim);   // Index 5
        targetKeyCodes.Add(settings.rightAim);// Index 6
        targetKeyCodes.Add(settings.leftAim); // Index 7
        targetKeyCodes.Add(settings.downAim); // Index 8  (Typo düzeltildi, aimDown varsayıldı)

        // Eğer GameSetting'deki field isimleri farklıysa burayı güncellemelisin.
    }
}