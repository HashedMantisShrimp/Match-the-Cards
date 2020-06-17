using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderBoard : MonoBehaviour
{
    private TextMesh positionObject;
    private TextMesh pNameObject;
    private TextMesh scoreObject;
    private TextMesh timeObject;

    internal string key { get; private set; } = "LeaderBoard";
    private string playerName;
    private int score;
    private string time;
    private Dictionary<string, PlayerInfo> leaderBoardPlayers = new Dictionary<string, PlayerInfo>();
    private List<GameObject> leaderBoardObject = new List<GameObject>();
    [SerializeField] private GameObject leaderBoardItem;

    // Start is called before the first frame update

    private void Awake()
    {
        LoadSavedData();
    }

    void Start()
    {
        playerName = null;
        score = -1;
        time = null;
    }

    // Leaderboard will generate LItems on the UI and in their respective positions.
    //Leaderboard will calculate the position of the next/current player.
    //Leaderboard needs to access a file database with info on players.
    

    internal void GetData(string _pName, int _score, string _time)
    {
        playerName = _pName;
        score = _score;
        time = _time;
        CreateNewPlayer(leaderBoardPlayers.Count);
        SortPositions();
        AlignLeaderBoardItems();
        SaveInfo();
    }

    internal void ResetLists()
    {
        foreach (GameObject player in leaderBoardObject)
        {
            player.SetActive(false);
            Destroy(player);
        }
        leaderBoardPlayers.Clear();
        leaderBoardObject.Clear();
    }

    private void SortPositions()
    {
      //  Debug.Log($"score: {score}, playerName: {playerName}, time: {time}");
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
                    }
                }
            }
        }
    }


    private void CreateNewPlayer(int ID)
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

    private void CreateNewPlayer(List<PlayerInfo> playerScripts)
    {
        foreach (PlayerInfo pScript in playerScripts)
        {
            GameObject player = Instantiate(leaderBoardItem);
            AssignValues(player, pScript);
            player.SetActive(false);
            leaderBoardObject.Add(player);
            leaderBoardPlayers.Add(pScript._pName, pScript);
        }
    }

    private void AlignLeaderBoardItems()
    {
        Vector3 templatePosition = leaderBoardItem.transform.position;

        if (leaderBoardObject.Count > 5)
        {

            for (int i = 0; i < 5; i++)
            {
                if (i != 0)
                {
                    leaderBoardObject[i].transform.position = new Vector3(templatePosition.x, templatePosition.y - 2.61f * i, templatePosition.z);
                }
                string name = leaderBoardObject[i].transform.Find("PName").GetComponent<TextMesh>().text;
                AssignValues(leaderBoardObject[i], leaderBoardPlayers[name]);
                leaderBoardObject[i].transform.Find("Pos").GetComponent<TextMesh>().text = (i + 1).ToString();
                leaderBoardObject[i].SetActive(true);
            }
        }
        else if (leaderBoardObject.Count>0 && leaderBoardObject.Count<5)
        {
            for (int i = 0; i < leaderBoardObject.Count; i++)
            {
                if (i != 0)
                {
                    leaderBoardObject[i].transform.position = new Vector3(templatePosition.x, templatePosition.y - 2.61f * i, templatePosition.z);
                }
                string name = leaderBoardObject[i].transform.Find("PName").GetComponent<TextMesh>().text;
                AssignValues(leaderBoardObject[i], leaderBoardPlayers[name]);
                leaderBoardObject[i].transform.Find("Pos").GetComponent<TextMesh>().text = (i+1).ToString();
                leaderBoardObject[i].SetActive(true);
            }
        }
    }
    
    private void AssignValues(GameObject item, PlayerInfo data)
    {
        pNameObject = item.transform.Find("PName").GetComponent<TextMesh>();
        scoreObject = item.transform.Find("Score").GetComponent<TextMesh>();
        timeObject = item.transform.Find("Time").GetComponent<TextMesh>();
        
        pNameObject.text = data._pName;
        scoreObject.text = data._score.ToString();
        timeObject.text = data._time;
    }
    
    private void SaveInfo()
    {
        List<PlayerInfo> playerScripts = new List<PlayerInfo>();
        foreach (PlayerInfo item in leaderBoardPlayers.Values)
        {
            playerScripts.Add(item);
        }

        ListOfPlayers playerList = new ListOfPlayers { leaderBList = playerScripts };
        string jsonData = JsonUtility.ToJson(playerList); //keep on from here
        PlayerPrefs.SetString(key, jsonData);
        PlayerPrefs.Save();
        Debug.Log($"Saved the json data as: {PlayerPrefs.GetString(key)}, jsonData: {jsonData}");
    }

    private void LoadSavedData()
    {
        if (leaderBoardObject.Count == 0)
        {
            string jsonData = PlayerPrefs.GetString(key);
            if (!string.IsNullOrEmpty(jsonData) && !string.IsNullOrWhiteSpace(jsonData))
            {
                ListOfPlayers savedData = JsonUtility.FromJson<ListOfPlayers>(jsonData);
                CreateNewPlayer(savedData.leaderBList);
                SortPositions();
                AlignLeaderBoardItems();
                Debug.Log($"Json data: {jsonData}");
                Debug.Log("align LeaderBoard Items called");
            }
        }
    }

    [System.Serializable]
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
}




