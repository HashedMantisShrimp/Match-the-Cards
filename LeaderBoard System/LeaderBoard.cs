using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class LeaderBoard : MonoBehaviour
{
    #region Variables
    private TextMesh positionObject;
    private TextMesh pNameObject;
    private TextMesh scoreObject;
    private TextMesh timeObject;

    internal string key { get; private set; } = "LeaderBoard";
    private ListOfPlayers dbLeaderBoardData = new ListOfPlayers();
    private string playerName;
    private int score;
    private string time;
    private Dictionary<string, PlayerInfo> leaderBoardPlayers = new Dictionary<string, PlayerInfo>();
    private List<GameObject> leaderBoardObject = new List<GameObject>();
    private GameData gameData;
    [SerializeField] private GameObject leaderBoardItem;
    #endregion

    //---------------------------------------------------------------------------------------------------

    void Awake()
    {
        gameData = GameData.GetInstance(); //TODO: This line of code might be unnecessary
        LoadSavedData();
    }

    void Start()
    {
        playerName = null;
        score = -1;
        time = null;
    }

    #region Internal Functions

    internal void SendData(string _time) //Gets data to be assigned to current player
    {
        playerName = gameData.GetPlayerName();
        score = gameData.GetScore();
        time = _time;
        SortingAlgorithm();
        SaveInfo();
    }

    internal void ResetLists() //Resets local LeaderBoard list
    {//TODO: Delete local Player Prefs as well?
        foreach (GameObject player in leaderBoardObject)
        {
            player.SetActive(false);
            Destroy(player);
        }
        leaderBoardPlayers.Clear();
        leaderBoardObject.Clear();
    }
    #endregion

    //---------------------------------------------------------------------------------------------------

    #region Private Functions

    private void SortPositions() //Sorts LeaderBoard items according to score (from lowest to highest)
    {
        if (leaderBoardObject.Count > 0)
        {
            int listCount = leaderBoardObject.Count;

            GameObject player1;
            int score1;
            string name1;

            GameObject player2;
            int score2;
            string name2;
                
            for (int i=0; i < listCount; i++)
            {
                player1 = leaderBoardObject[i];
                name1 = player1.transform.Find("PName").GetComponent<TextMesh>().text;

                PlayerInfo pInfo = leaderBoardPlayers[name1];
                score1 = pInfo._score;

                for (int j = i+1; j < listCount; j++)
                {
                    player2 = leaderBoardObject[j];
                    name2 = player2.transform.Find("PName").GetComponent<TextMesh>().text;

                    PlayerInfo pInfo2 = leaderBoardPlayers[name2];
                    score2 = pInfo2._score;

                    if (score2<score1)
                    {
                        leaderBoardObject[i] = player2;
                        leaderBoardObject[j] = player1;

                        player1 = leaderBoardObject[i];
                    }
                }
            }
        }
    }

    private void CreateNewPlayer() //Creates player or updates player data
    {
        if (!leaderBoardPlayers.ContainsKey(playerName))
        {
            GameObject newPlayer = Instantiate(leaderBoardItem);
            PlayerInfo playerScript = new PlayerInfo
            {
                _pName = playerName,
                _score = score,
                _time = time
            };

            AssignValues(newPlayer, playerScript);
            newPlayer.SetActive(false);

            leaderBoardObject.Add(newPlayer);
            leaderBoardPlayers.Add(playerName, playerScript);
        }
        else
        {
            if (leaderBoardPlayers[playerName]._score > score)
            {
                leaderBoardPlayers[playerName]._score = score;
                leaderBoardPlayers[playerName]._time = time;
            }
        }
    }

    private void CreateNewPlayer(List<PlayerInfo> playerScripts) //Creates players from a loaded list
    {
        foreach (PlayerInfo pScript in playerScripts)
        {
            if (!leaderBoardPlayers.ContainsKey(pScript._pName))
            {
                GameObject player = Instantiate(leaderBoardItem);
                AssignValues(player, pScript);
                player.SetActive(false);
                leaderBoardObject.Add(player);
                leaderBoardPlayers.Add(pScript._pName, pScript);
            }
            else
            {
                if (leaderBoardPlayers[pScript._pName]._score > pScript._score)
                {
                    leaderBoardPlayers[pScript._pName]._score = pScript._score;
                    leaderBoardPlayers[pScript._pName]._time = pScript._time;
                }
            }
        }
    }

    private void AlignLeaderBoardItems() //assigns the physical position of LeaderBoard players
    {
        Vector3 templatePosition = leaderBoardItem.transform.position;

        if (leaderBoardObject.Count > 0)
        {
            int limit = (leaderBoardObject.Count >= 5) ? 5 : leaderBoardObject.Count;

            for (int i = 0; i < limit; i++)
            {
                leaderBoardObject[i].transform.position = new Vector3(templatePosition.x, templatePosition.y - 2.61f * i, templatePosition.z);
                string name = leaderBoardObject[i].transform.Find("PName").GetComponent<TextMesh>().text;
                AssignValues(leaderBoardObject[i], leaderBoardPlayers[name]);
                leaderBoardObject[i].transform.Find("Pos").GetComponent<TextMesh>().text = (i + 1).ToString();
                leaderBoardObject[i].SetActive(true);
            }

            if (leaderBoardObject.Count > 5) //Removes players below the 5th position
            {
                var indexToBeRemoved = new List<int>();

                for (int i = 5; i < leaderBoardObject.Count; i++)//Deactivates & Destroys gameObjects below the 5h position
                {
                    string name = leaderBoardObject[i].transform.Find("PName").GetComponent<TextMesh>().text;
                    Destroy(leaderBoardObject[i]);
                    indexToBeRemoved.Add(i);
                    leaderBoardPlayers.Remove(name);
                }

                for (int i=0; i<indexToBeRemoved.Count; i++)//Removes players below the 5h position from leaderBoardObject list
                {
                    leaderBoardObject.RemoveAt(5);
                }
                indexToBeRemoved.Clear();
            }
        }
    }
    
    private void AssignValues(GameObject item, PlayerInfo data) //Assigns data to LeaderBoard gameObject
    {
        pNameObject = item.transform.Find("PName").GetComponent<TextMesh>();
        scoreObject = item.transform.Find("Score").GetComponent<TextMesh>();
        timeObject = item.transform.Find("Time").GetComponent<TextMesh>();
        
        pNameObject.text = data._pName;
        scoreObject.text = data._score.ToString();
        timeObject.text = data._time;
    }
    
    private async Task SaveInfo() //Saves the current LeaderBoard players into a JSON string
    {
        try
        {
            if (await Internet.CheckInternetConnectivity())
            {
                var dbData = gameData.GetLeaderBoardJSON();

                if (Misc.IsStringValid(dbData))//If player loaded leaderboard data from db, then save
                {
                    var jsonList = PrepareSaveData();
                    await Database.SaveLeaderBoard(jsonList);
                    GameData.SetLeaderBoardJSON(string.Empty);
                    Debug.Log($"Internet connection <color=blue>found</color>, Saved {nameof(LeaderBoard)} data to db.");
                }
                else//If player loaded leaderboard data from local storage, then compare local vs online data and then save
                {
                    var onlineDataJSON = await Database.LoadLeaderBoardData();

                    if (Misc.IsStringValid(onlineDataJSON))
                    {
                        var onlineData = JsonUtility.FromJson<ListOfPlayers>(onlineDataJSON);

                        SortingAlgorithm(onlineData.leaderBList);
                        var jsonList = PrepareSaveData();
                        await Database.SaveLeaderBoard(jsonList);
                    }
                }
            }
            else
            {
                var jsonList = PrepareSaveData();
                Debug.Log($"Internet connection <color=blue>NOT found</color>, Saved {nameof(LeaderBoard)} data to PlayerPrefs.");
                PlayerPrefs.SetString(key, jsonList);
                PlayerPrefs.Save();
                //Debug.Log($"Saved the json data as: {PlayerPrefs.GetString(key)}, jsonData: {jsonList}");
            }
        }
        catch (Exception e)
        {
            Misc.HandleException(e, gameData.GetExcLeaderBoardSaveInfo());
        }
    }

    private void GetDataFromDB() //Gets the JSON leaderboard string loaded from db
    {
        string JSONData = gameData.GetLeaderBoardJSON();

        if (Misc.IsStringValid(JSONData))
        {
            dbLeaderBoardData = JsonUtility.FromJson<ListOfPlayers>(JSONData);
            Debug.Log($"{nameof(dbLeaderBoardData)} will be set to <color=blue>{JSONData}</color>");
        }
        else
        {
            dbLeaderBoardData = null;
            Debug.Log($"{nameof(dbLeaderBoardData)} will be set to <color=red>null</color>");
        }
        //Debug.Log($"{nameof(JSONData)}: {JSONData}.");
    }

    private void LoadSavedData() //Loads the previous LeaderBoard players from JSON string
    {
        try
        {
            if (leaderBoardObject.Count == 0)
            {
                GetDataFromDB();

                if (dbLeaderBoardData == null)
                {
                    string jsonData = PlayerPrefs.GetString(key);
                    if (Misc.IsStringValid(jsonData))
                    {
                        Debug.Log($"{nameof(dbLeaderBoardData)} <color=blue>is null</color>. Loading & building LeaderBoard data from PlayerPrefs");

                        ListOfPlayers savedData = JsonUtility.FromJson<ListOfPlayers>(jsonData);
                        SortingAlgorithm(savedData.leaderBList);
                        Debug.Log($"Loaded PlayerPrefs data: {jsonData}");
                    }
                }
                else
                {
                    Debug.Log($"{nameof(dbLeaderBoardData)} <color=blue>is NOT null</color>. Building LeaderBoard from db Data");
                    SortingAlgorithm(dbLeaderBoardData.leaderBList);
                }
            }
        }
        catch(Exception e)
        {
            Misc.HandleException(e, gameData.GetExcLeaderBoardLoadSave());
        }
    }

    private string PrepareSaveData()//Prepares and returns json data to be saved
    {
        List<PlayerInfo> playerScripts = new List<PlayerInfo>();
        foreach (PlayerInfo item in leaderBoardPlayers.Values)
        {
            playerScripts.Add(item);
        }

        ListOfPlayers playerList = new ListOfPlayers { leaderBList = playerScripts };
        string jsonData = JsonUtility.ToJson(playerList);
        
        return jsonData;
    }

    private void SortingAlgorithm(List<PlayerInfo> lbList)//Handles the creation, sorting and alignment porcess w/ leaderboard list
    {
        CreateNewPlayer(lbList);
        SortPositions();
        AlignLeaderBoardItems();
    }

    private void SortingAlgorithm()//Handles the creation, sorting and alignment porcess w/o leaderboard list
    {
        CreateNewPlayer();
        SortPositions();
        AlignLeaderBoardItems();
    }
    #endregion

    //---------------------------------------------------------------------------------------------------

    #region Extra Classes

    [Serializable]
    private class PlayerInfo
    {
        public string _pName;
        public int _score;
        public string _time;
    }

    private class ListOfPlayers
    {
        public List<PlayerInfo> leaderBList;
    }
    #endregion
}