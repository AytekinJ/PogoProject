using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[Serializable]
public struct GameData
{
    public int health;
    public int currentLevel;
    public int coins;
}

public class SaveSystem : MonoBehaviour
{
    private byte xorKey = 0xAF;
    [SerializeField] private string filePath;
    public static SaveSystem instance;

    private void Awake()
    {
        instance = this;
    }

    public void SaveGame(GameData data)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        using (MemoryStream ms = new MemoryStream())
        {
            Debug.Log("Saving Game....");
            formatter.Serialize(ms, data);
            byte[] serializedData = ms.ToArray();
            Debug.Log($"Serialized Data: {BitConverter.ToString(serializedData)}");

            byte[] encryptedData = XorEncrypt(serializedData);
            Debug.Log($"Encrypted Data: {BitConverter.ToString(encryptedData)}");

            File.WriteAllBytes(filePath, encryptedData);
        }
        Debug.Log("Game Saved Successfully");
    }

    public GameData LoadGame()
    {
        Debug.Log("Loading Game....");
        BinaryFormatter formatter = new BinaryFormatter();
        byte[] fileData = File.ReadAllBytes(filePath);
        Debug.Log($"Read File Data: {BitConverter.ToString(fileData)}");

        byte[] decryptedData = XorEncrypt(fileData);
        Debug.Log($"Decrypted Data: {BitConverter.ToString(decryptedData)}");

        using (MemoryStream ms = new MemoryStream(decryptedData))
        {
            Debug.Log("Game Loaded Successfully");
            return (GameData)formatter.Deserialize(ms);
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
}
