using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SaveGame : MonoBehaviour
{
    Data data;
    internal string saveKey { get; private set; }
    internal const string saveFormat = "gameSave.save";
    private string saveFileName;

    private void Awake()
    {
        data = new Data();
        saveKey = $"save{PlayerPrefs.GetString("playerName")}";
    }

    internal void AcquireData(int[] idArray) //Acquires the shuffled arrays used for card IDs
    {
        data.IDs = idArray;
    }

    internal void AcquireData(string pName, float time, int nMoves, int nMatches) //Acquires general game data
    {
        data.playerName = pName;
        data.time = time;
        data.moves = nMoves;
        data.matches = nMatches;
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

    internal void SaveData(string playerName) //Saves acquired data into a file
    {
        try
        {
            saveFileName = playerName + saveFormat;
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(Application.persistentDataPath + $"/{saveFileName}");
            PlayerPrefs.SetInt(saveKey, 1);
            PlayerPrefs.Save();

            bf.Serialize(file, data);
            file.Close();
            Debug.Log("Game Saved.");
        }
        catch (Exception e)
        {
            Debug.Log($"<color=red>ATTENTION</color> Error found while saving game file: {e.Message}");
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


