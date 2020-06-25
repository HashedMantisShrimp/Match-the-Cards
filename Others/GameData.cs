using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{// Make class and/or varibales static?

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
    private List<MainCard> cardList = new List<MainCard>();

    private void Awake()
    {
        AssignInitialValues();
    }

    private void AssignInitialValues()
    {
        playerName = PlayerPrefs.GetString("playerName");
        playerName = (string.IsNullOrEmpty(playerName) || string.IsNullOrWhiteSpace(playerName)) ? "John Doe" : playerName;
    }

    //---------------------------------------------------------------------------------------------------

    #region Internal Functions

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
}
