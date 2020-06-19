using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class LoadGame : MonoBehaviour
{
    private Data saveData;
    private string playerName;
    private string saveFileName;
    private string saveKey;
    internal bool saveDataIsPresent { get; private set; }

    private void Awake()
    {
        AssignFields();
    }

    // Start is called before the first frame update
    void Start()
    {
        LoadSavedGame();
    }

    internal Data SendData()
    {
        return saveData;
    }
    

    private void LoadSavedGame()
    {
        try
        {
            if (IsSaveDataPresent(playerName))
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(Application.persistentDataPath + $"/{saveFileName}", FileMode.Open); //find a way to check if file was found
                saveData = (Data)bf.Deserialize(file);
                saveDataIsPresent = true;
            }
            else
            {
                saveDataIsPresent = false;
            }
        }
        catch (Exception e)
        {
            Debug.Log($"<color=red>ATTENTION</color> Error found while loading save file: {e}");
        }
    }

    private bool IsSaveDataPresent(string playerName)
    {
        saveKey = PlayerPrefs.GetString("playerName");
        int isPresent = PlayerPrefs.GetInt(saveKey);

        if (isPresent == 1)
            return true;

        return false;
    }

    private void AssignFields()
    {
        saveData = new Data();
        playerName = PlayerPrefs.GetString("playerName");
        string saveFormat = PlayerPrefs.GetString("saveFormat");
        playerName = (string.IsNullOrEmpty(playerName) || string.IsNullOrWhiteSpace(playerName)) ? "John Doe" : playerName;
        saveFileName = $"{playerName + saveFormat}";
        saveDataIsPresent = false;
    }
}
