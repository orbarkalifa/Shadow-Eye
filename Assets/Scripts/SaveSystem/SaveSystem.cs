using UnityEngine;
using System.IO;

public static class SaveSystem
{
    private static readonly string SAVE_FILE_PATH = Path.Combine(Application.persistentDataPath, "retryData.json");

    public static void SaveOnDeath(PlayerSaveData data)
    {
        try
        {
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(SAVE_FILE_PATH, json);
            Debug.Log("Retry data saved to: " + SAVE_FILE_PATH);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to save retry data: " + e.Message);
        }
    }

    public static PlayerSaveData LoadForRetry()
    {
        if (File.Exists(SAVE_FILE_PATH))
        {
            try
            {
                string json = File.ReadAllText(SAVE_FILE_PATH);
                return JsonUtility.FromJson<PlayerSaveData>(json);
            }
            catch (System.Exception e)
            {
                Debug.LogError("Failed to load retry data: " + e.Message);
                return null;
            }
        }
        return null; 
    }
}