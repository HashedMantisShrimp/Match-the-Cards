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
    [SerializeField] private GameData gameData;

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
                FileStream file = File.Open(Application.persistentDataPath + $"/{saveFileName}", FileMode.Open); //find a way to check if file was found
                saveData = (Data)bf.Deserialize(file);
                saveDataIsPresent = true;
                //Debug.Log($"Saved Data: Player - {saveData.playerName}, Number of moves - {saveData.moves}, Time - {saveData.time}, matches - {saveData.matches}");
                GameData.saveData = saveData;
                gameData.SetSaveDataPresent(saveDataIsPresent);
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
        playerName = PlayerPrefs.GetString("playerName");
        playerName = (string.IsNullOrEmpty(playerName) || string.IsNullOrWhiteSpace(playerName)) ? "John Doe" : playerName;
        saveFormat = GameData.saveFormat;
        saveData = new Data();
        saveKey = $"save{playerName}";
        saveFileName = $"{playerName + saveFormat}";
    }
    #endregion
}
