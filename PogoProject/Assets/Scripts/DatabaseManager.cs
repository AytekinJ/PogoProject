using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using System.Collections.Generic;

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
}