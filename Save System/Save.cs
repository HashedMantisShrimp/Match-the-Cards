using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class Save : MonoBehaviour
{
    Data data = new Data();
    private string playerName;
    private string saveFileName;
    private string saveKey;
    GameData gameData;

    private void Start()
    {
        gameData = GameData.GetInstance();
        saveKey = $"save{gameData.GetPlayerName()}";
    }

    #region AcquireData

    internal void AcquireData() //Acquires general game data
    {
        data.IDs = gameData.GetIDArray();
        data.playerName = gameData.GetPlayerName();
        data.time = gameData.GetTime();
        data.moves = gameData.GetMoveCounter();
        data.matches = gameData.GetCurrentMatches();
        Debug.Log($"Save - Time: {data.time}, Moves: {data.moves}, Matches: {data.matches}");
    }

    internal void AcquireData(int id) //Acquires a new ID based on card Count
    {
        CardInfo newCard = new CardInfo();
        data.cards.Add(id, newCard);
    }

    internal void AcquireData(int id, bool scriptEnabled, bool boxColliderEnabled, bool isCardBackActive) //Acquires card info
    {
        data.cards[id].scriptEnabled = scriptEnabled;
        data.cards[id].boxColliderEnabled = boxColliderEnabled;
        data.cards[id].cardBackEnabled = isCardBackActive;
    }

    internal void AcquireData(Dictionary<int, CardInfo> _cards)
    {
        data.cards = _cards;
    }
    #endregion

    //---------------------------------------------------------------------------------------------------

    internal async Task SaveData() //Saves acquired data into a file
    {
        FileStream file = File.Create(Application.persistentDataPath + $"/{saveFileName}");//TODO: Revisit this change later, will it work the same for db?
        //TODO: Consider instead, opening the file inside the try block and referencing an outside variable to it
        try
        {
            playerName = gameData.GetPlayerName();
            saveFileName = playerName + GameData.saveFormat;

            bool internetConnection = await Internet.CheckInternetConnectivity();

            if (internetConnection)
            {
                //Substitute with code to save data onto online db

                BinaryFormatter bf = new BinaryFormatter();
                PlayerPrefs.SetInt(saveKey, 1);
                PlayerPrefs.Save();

                bf.Serialize(file, data);
                Debug.Log("Game Saved. With Internet connection");
            }
            else
            {

                BinaryFormatter bf = new BinaryFormatter();
                PlayerPrefs.SetInt(saveKey, 1);
                PlayerPrefs.Save();

                bf.Serialize(file, data);
                Debug.Log("Game Saved. Without Internet connection");
            }
        }
        catch (Exception e)
        {
            Misc.HandleException(e, gameData.GetExcSaveData());
        }
        finally
        {
            file.Close();
        }
    }

    internal void DeleteSaveData() //TODO: Test this method, maybe implement async & await
    {
        try
        {
            if (gameData.GetSaveDataPresent())
            {
                if (Misc.IsStringNullOr(playerName))
                    playerName = gameData.GetPlayerName();

                if (Misc.IsStringNullOr(saveFileName))
                    saveFileName = playerName + GameData.saveFormat;

                PlayerPrefs.DeleteKey(saveKey);
                PlayerPrefs.Save();

                File.Delete(Application.persistentDataPath + $"/{saveFileName}");

                Debug.Log($"<color=red>{playerName}'s</color> save file was deleted.");

                gameData.SetSaveDataPresent(false);
            }
        }
        catch (Exception e)
        {
            Misc.HandleException(e, gameData.GetExcDeleteSave());
        }
    }
}

[Serializable]
public class CardInfo
{
    public bool scriptEnabled;
    public bool boxColliderEnabled;
    public bool cardBackEnabled;
}

[Serializable]
public class Data
{
    public int moves;
    public int matches;
    public string playerName;
    public float time;
    public int[] IDs = new int[12];
    public Dictionary<int, CardInfo> cards = new Dictionary<int, CardInfo>();
}
