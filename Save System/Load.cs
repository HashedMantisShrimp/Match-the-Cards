using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class Load : MonoBehaviour
{
    internal bool saveDataIsPresent { get; private set; } = false;
    private Data saveData;
    private string saveFormat;
    private string playerName;
    private string saveFileName;
    private string saveKey;
    private GameData gameData;

    private void Awake()
    {
        AssignFields();
        LoadSavedGame();
    }

    //---------------------------------------------------------------------------------------------------

    #region Private Functions

    private void LoadSavedGame() //Loads a save file, if any exists
    {
        try
        {
            if (IsSaveDataPresent(playerName))
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(Application.persistentDataPath + $"/{saveFileName}", FileMode.Open); //TODO: Find a way to check if file was found
                saveData = (Data)bf.Deserialize(file);
                saveDataIsPresent = true;
                Debug.Log($"<color=yellow>Loaded Data</color> - Player: {saveData.playerName}, Number of moves: {saveData.moves}, matches: {saveData.matches}, Time: {saveData.time}");
                GameData.saveData = saveData;
                gameData.SetSaveDataPresent(saveDataIsPresent);
                file.Close();
            }
            else
            {
                saveDataIsPresent = false;
                gameData.SetSaveDataPresent(saveDataIsPresent);
                //Debug.Log("Save data is not present");
            }
        }
        catch (Exception e)
        {
            Debug.Log($"<color=red>ATTENTION</color> Error found while loading save file: {e.Message}");
        }
    }

    private bool IsSaveDataPresent(string playerName) //Checks if there is a save file for the current player
    {
        
        int isPresent = PlayerPrefs.GetInt(saveKey);

        if (isPresent == 1)
        {
            //Debug.Log($"PLayer was found isPresent: {isPresent}, saveKey: {saveKey}, playerName: {playerName}");
            return true;
        }
        
        return false;
    }

    private void AssignFields()
    {
        gameData = GameData.GetInstance();
        playerName = PlayerPrefs.GetString("playerName");
        playerName = (string.IsNullOrEmpty(playerName) || string.IsNullOrWhiteSpace(playerName)) ? "John Doe" : playerName;
        saveFormat = GameData.saveFormat;
        saveData = new Data();
        saveKey = $"save{playerName}";
        saveFileName = $"{playerName + saveFormat}";
    }
    #endregion
}
