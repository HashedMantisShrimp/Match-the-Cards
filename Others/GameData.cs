using System.Collections.Generic;
using UnityEngine;

public class GameData
{
    #region Variables

    internal const string saveFormat = "gameSave.save";
    internal const int totalMatches = 4;
    internal static Data saveData;
    private bool saveDataPresent;
    private string playerName;
    private float time;
    private int score;
    private int moveCounter;
    private int currentMatches;
    private int[] idArray;
    private static SoundPlayer soundManager;
    private List<MainCard> cardList = new List<MainCard>();
    private static string LeaderBoardJSON = string.Empty;
    private const string ExcLeaderBoardSaveInfo = "<color=red>Error</color> found while saving LeaderBoard list:";
    private const string ExcLeaderBoardLoadSave = "<color=red>Error</color> found while loading LeaderBoard list:";
    private const string ExcInternetConnectivity = "<color=red>Error</color> found while checking Internet Connection:";
    private const string ExcLoadSavedGame = "<color=red>Error</color> found while loading save file:";
    private const string ExcDeleteSave = "<color=red>Error</color> found while deleting save file:";
    private const string ExcSaveData = "<color=red>Error</color> found while saving game file:";
    private const string ExcDBSaveLeaderBoard = "<color=red>Error</color> found while saving leaderboard data:";
    private const string ExcDBLoadLeaderBoard = "<color=red>Error</color> found while loading leaderboard data:";
    #endregion

    //---------------------------------------------------------------------------------------------------

    #region Init Functions

    private GameData()
    {
        AssignInitialValues();
    }

    private void AssignInitialValues()
    {
        soundManager = SoundPlayer.Instance;
        playerName = PlayerPrefs.GetString("playerName");
        playerName = (string.IsNullOrEmpty(playerName) || string.IsNullOrWhiteSpace(playerName)) ? "John Doe" : playerName;
    }

    private static GameData instance = new GameData();

    internal static GameData GetInstance()
    {
        return instance;
    }
    #endregion

    //---------------------------------------------------------------------------------------------------

    #region Internal Functions

    //---------------------------------------------------------------------------------------------------
        
    #region Set

    internal static void SetLeaderBoardJSON(string JSONdata)
    {
        LeaderBoardJSON = JSONdata;
        Debug.Log($"GameData {nameof(LeaderBoardJSON)} = {LeaderBoardJSON}");
    }

    internal void SetSaveDataPresent(bool isDataPresent)
    {
        saveDataPresent = isDataPresent;
    }

    internal void AddToCardList(MainCard cardListItem)
    {
        cardList.Add(cardListItem);
    }

    internal void SetTime(float _time)
    {
        time = _time;
    }

    internal void SetScore(int _score)
    {
        score = _score;
    }

    internal void SetMoveCounter(int _moveCounter)
    {
        moveCounter = _moveCounter;
    }

    internal void SetCurrentMatches(int _matches)
    {
        currentMatches = _matches;
    }

    internal void SetIDArray(int[] _idArray)
    {
        idArray = _idArray;
    }
    #endregion
        
    //---------------------------------------------------------------------------------------------------

    #region Get

    #region Const Get Functions

    internal string GetExcSaveData()
    {
        return ExcSaveData;
    }

    internal string GetExcDeleteSave()
    {
        return ExcDeleteSave;
    }

    internal string GetExcLoadSavedGame()
    {
        return ExcLoadSavedGame;
    }

    internal static string GetExcInternetConnectivity()
    {
        return ExcInternetConnectivity;
    }

    internal string GetExcLeaderBoardLoadSave()
    {
        return ExcLeaderBoardLoadSave;
    }

    internal string GetExcLeaderBoardSaveInfo()
    {
        return ExcLeaderBoardSaveInfo;
    }

    internal static string GetExcDBSaveLeaderBoard()
    {
        return ExcDBSaveLeaderBoard;
    }

    internal static string GetExcDBLoadLeaderBoard()
    {
        return ExcDBLoadLeaderBoard;
    }
    #endregion

    //------------------------------------------------

    #region NonConst Get Functions

    internal static SoundPlayer GetSoundManager()
    {
        return soundManager;
    }

    internal string GetLeaderBoardJSON()
    {
        return LeaderBoardJSON;
    }

    internal bool GetSaveDataPresent()
    {
        return saveDataPresent;
    }

    internal string GetPlayerName()
    {
        return playerName;
    }

    internal float GetTime()
    {
        return time;
    }

    internal int GetScore()
    {
        return score;
    }

    internal int GetMoveCounter()
    {
        return moveCounter;
    }

    internal int GetCurrentMatches()
    {
        return currentMatches;
    }

    internal int[] GetIDArray()
    {
        return idArray;
    }

    internal List<MainCard> GetMainCards()
    {
        return cardList;
    }
    #endregion

    #endregion

    //---------------------------------------------------------------------------------------------------
        
    #endregion
}
