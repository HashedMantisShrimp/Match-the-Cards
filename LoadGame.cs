using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class LoadGame : MonoBehaviour
{
    private Data saveData;
    private const string saveFormat = "gameSave.save";
    private string playerName;
    private string saveFileName;
    private string saveKey;
    internal bool saveDataIsPresent { get; private set; } = false;

    private void Awake()
    {
        AssignFields();
        LoadSavedGame();
    }

    internal Data SendData()
    {
        return saveData;
    }

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
                //Debug.Log($"Saved Data: Player - {saveData.playerName}, Number of moves - {saveData.moves}");
            }
            else
            {
                saveDataIsPresent = false;
                Debug.Log("Save data is not present");
            }
        }
        catch (Exception e)
        {
            Debug.Log($"<color=red>ATTENTION</color> Error found while loading save file: {e.Message}");
        }
    }

    private bool IsSaveDataPresent(string playerName) //Checks if there is a save file for the current player
    {
        saveKey = $"save{playerName}";
        int isPresent = PlayerPrefs.GetInt(saveKey);

        if (isPresent == 1)
        {
            //Debug.Log($"PLayer was found isPresent: {isPresent}, saveKey: {saveKey}, playerName: {playerName}");
            return true;
        }

        //Debug.Log($"Player was <color=red>NOT</color> found isPresent: {isPresent}, saveKey: {saveKey}, playerName: {playerName}");
        return false;
    }

    private void AssignFields()
    {
        saveData = new Data();
        playerName = PlayerPrefs.GetString("playerName");
        playerName = (string.IsNullOrEmpty(playerName) || string.IsNullOrWhiteSpace(playerName)) ? "John Doe" : playerName;
        saveFileName = $"{playerName + saveFormat}";
    }

    #endregion
}
