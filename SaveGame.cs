using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections.Generic;
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

    internal void AcquireData(int[] idArray) { data.IDs = idArray; }
    internal void AcquireData(string pName, float time, int nMoves, int nMatches) { data.playerName = pName; data.time = time; data.moves = nMoves; data.matches = nMatches; }

    internal void AcquireData(int id) { CardInfo newCard = new CardInfo();  data.cards.Add(id, newCard); }
    internal void AcquireData(int id, bool scriptEnabled, bool boxColliderEnabled, bool isCardBackActive)
    { data.cards[id].scriptEnabled = scriptEnabled; data.cards[id].boxColliderEnabled = boxColliderEnabled; data.cards[id].cardBackEnabled = isCardBackActive; }

    internal void SaveData(string playerName)
    {
        try
        {
            saveFileName = playerName + saveFormat;
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(Application.persistentDataPath + $"/{saveFileName}");
            PlayerPrefs.SetInt(saveKey, 1);
            PlayerPrefs.SetString("saveFormat", saveFormat);
            PlayerPrefs.Save();

            bf.Serialize(file, data);
            file.Close();
            Debug.Log("Game Saved.");
        }
        catch (Exception e)
        {
            Debug.Log($"<color=red>ATTENTION</color> Error found while saving game file: {e}");
        }
        
    }
    
}

[System.Serializable]
public class CardInfo
{
    public bool scriptEnabled;
    public bool boxColliderEnabled;
    public bool cardBackEnabled;
}

[System.Serializable]
public class Data
{
    public int moves;
    public int matches;
    public string playerName;
    public float time;
    public int[] IDs = new int[12];
    public Dictionary<int, CardInfo> cards = new Dictionary<int, CardInfo>();
}


