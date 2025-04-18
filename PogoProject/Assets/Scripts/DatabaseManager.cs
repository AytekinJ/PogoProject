using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public struct GameData
{
    public int health;
    public int currentLevel;
    public int coins;
}

public class DatabaseManager : MonoBehaviour
{
    private byte xorKey = 0xAF;
    private string levelProgressFilePath;
    private string gameDataFilePath;
    private string settingsDataFilePath;

    public static DatabaseManager main;

    private Dictionary<string, LevelProgressData> allLevelProgress = new Dictionary<string, LevelProgressData>();

    private void Awake()
    {
        if (main == null)
        {
            main = this;
        }
        else if (main != this)
        {
            Destroy(gameObject);
            return;
        }

        levelProgressFilePath = Path.Combine(Application.persistentDataPath, "levelProgress.dat");
        gameDataFilePath = Path.Combine(Application.persistentDataPath, "gameData.dat");
        settingsDataFilePath = Path.Combine(Application.persistentDataPath, "settingsData.dat");

        LoadAllLevelProgress();
    }

    public void SaveGame<T>(T data, string filePath)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        using (MemoryStream ms = new MemoryStream())
        {
      
            formatter.Serialize(ms, data);
            byte[] serializedData = ms.ToArray();
            byte[] encryptedData = XorEncrypt(serializedData);
            File.WriteAllBytes(filePath, encryptedData);
        }
 
    }

    public T LoadGame<T>(string filePath)
    {

        BinaryFormatter formatter = new BinaryFormatter();
        byte[] fileData = File.ReadAllBytes(filePath);
        byte[] decryptedData = XorEncrypt(fileData);
        using (MemoryStream ms = new MemoryStream(decryptedData))
        {
            return (T)formatter.Deserialize(ms);
        }
    }

     private byte[] XorEncrypt(byte[] data)
    {
        byte[] encryptedData = new byte[data.Length];
        for (int i = 0; i < data.Length; i++)
        {
            encryptedData[i] = (byte)(data[i] ^ xorKey);
        }
        return encryptedData;
    }

    public void SaveLevelProgress(LevelProgressData progressData)
    {
        if (progressData == null || string.IsNullOrEmpty(progressData.levelIdentifier))
        {
            Debug.LogError("Kaydedilecek LevelProgressData geçersiz.");
            return;
        }

        allLevelProgress[progressData.levelIdentifier] = progressData;

        SaveGame(allLevelProgress, levelProgressFilePath);

    }

    public void LoadAllLevelProgress()
    {
        if (File.Exists(levelProgressFilePath))
        {
            allLevelProgress = LoadGame<Dictionary<string, LevelProgressData>>(levelProgressFilePath);
            if (allLevelProgress == null)
            {
                 allLevelProgress = new Dictionary<string, LevelProgressData>();
                 Debug.LogWarning("Level ilerleme dosyası okunamadı veya boş. Yeni Dictionary oluşturuldu.");
            }
        }
        else
        {
            allLevelProgress = new Dictionary<string, LevelProgressData>();

        }
    }

    public LevelProgressData GetLevelProgress(string levelIdentifier)
    {
        if (allLevelProgress.TryGetValue(levelIdentifier, out LevelProgressData progress))
        {
            return progress;
        }
        return null;
    }

    public void SaveGeneralGameData(GameData data)
    {
        SaveGame(data, gameDataFilePath);
    }

    public GameData LoadGeneralGameData()
    {
        if (File.Exists(gameDataFilePath))
        {
            return LoadGame<GameData>(gameDataFilePath);
        }
        return new GameData { health = 100, currentLevel = 0, coins = 0 };
    }
    public void SaveSettingsData()
    {
        GameSetting data = GameSetting.Instance;
        GameSettingData saveData = new GameSettingData()
        {
            rWidth = data.rWidth,
            rHeight = data.rHeight,
            fps = data.fps,
            vsync = data.vsync,
            shadows = data.shadows,
            postprocessing = data.postprocessing,
            antialiasing = data.antialiasing,
            antialiasingQuality = data.antialiasingQuality,
            masterVolume = data.masterVolume,
            musicVolume = data.musicVolume,
            sfxVolume = data.sfxVolume,
            inputEnabled = data.inputEnabled,
            right = data.right,
            left = data.left,
            up = data.up,
            down = data.down,
            attack = data.attack,
            rightAim = data.rightAim,
            leftAim = data.leftAim,
            upAim = data.upAim,
            downAim = data.downAim,
            JumpButton = data.JumpButton,
            DpadUp = data.DpadUp,
            DpadDown = data.DpadDown,
            DpadLeft = data.DpadLeft,
            DpadRight = data.DpadRight
        };

        BinaryFormatter formatter = new BinaryFormatter();
        using (MemoryStream ms = new MemoryStream())
        {
            formatter.Serialize(ms, saveData);
            File.WriteAllBytes(settingsDataFilePath, ms.ToArray());
        }

        Debug.Log("Ayarlar dosyaya gömüldü kardeşim.");
    }
    public void LoadSettingsData()
    {
        if (!File.Exists(settingsDataFilePath))
        {
            Debug.LogWarning("Ayar dosyası bulunamadı, sıçtık desene.");
            return;
        }

        byte[] fileData = File.ReadAllBytes(settingsDataFilePath);
        BinaryFormatter formatter = new BinaryFormatter();
        using (MemoryStream ms = new MemoryStream(fileData))
        {
            GameSettingData loadData = (GameSettingData)formatter.Deserialize(ms);
            GameSetting.Instance.rWidth = loadData.rWidth;
            GameSetting.Instance.rHeight = loadData.rHeight;
            GameSetting.Instance.fps = loadData.fps;
            GameSetting.Instance.vsync = loadData.vsync;
            GameSetting.Instance.shadows = loadData.shadows;
            GameSetting.Instance.postprocessing = loadData.postprocessing;
            GameSetting.Instance.antialiasing = loadData.antialiasing;
            GameSetting.Instance.antialiasingQuality = loadData.antialiasingQuality;
            GameSetting.Instance.masterVolume = loadData.masterVolume;
            GameSetting.Instance.musicVolume = loadData.musicVolume;
            GameSetting.Instance.sfxVolume = loadData.sfxVolume;
            GameSetting.Instance.inputEnabled = loadData.inputEnabled;
            GameSetting.Instance.right = loadData.right;
            GameSetting.Instance.left = loadData.left;
            GameSetting.Instance.up = loadData.up;
            GameSetting.Instance.down = loadData.down;
            GameSetting.Instance.attack = loadData.attack;
            GameSetting.Instance.rightAim = loadData.rightAim;
            GameSetting.Instance.leftAim = loadData.leftAim;
            GameSetting.Instance.upAim = loadData.upAim;
            GameSetting.Instance.downAim = loadData.downAim;
            GameSetting.Instance.JumpButton = loadData.JumpButton;
            GameSetting.Instance.DpadUp = loadData.DpadUp;
            GameSetting.Instance.DpadDown = loadData.DpadDown;
            GameSetting.Instance.DpadLeft = loadData.DpadLeft;
            GameSetting.Instance.DpadRight = loadData.DpadRight;
        }

        Debug.Log("Ayarlar başarıyla hortlatıldı.");
    }

}