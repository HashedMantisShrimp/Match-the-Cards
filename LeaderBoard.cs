using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderBoard : MonoBehaviour
{
    private TextMesh positionObject;
    private TextMesh pNameObject;
    private TextMesh scoreObject;
    private TextMesh timeObject;

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
        //CalculatePosition();
        AlignLeaderBoardItems();
        SaveInfo();
    }

    private void CalculatePosition()
    {
        if (score != -1 && !string.IsNullOrEmpty(playerName) && !string.IsNullOrEmpty(time))
        {
            if (leaderBoardPlayers.Count > 0)
            {
                if (leaderBoardPlayers.ContainsKey(playerName))
                {

                    foreach (PlayerInfo player in leaderBoardPlayers.Values)
                    {
                        if (score < player._score)
                        {
                            CreateNewPlayer(player._pos);
                            player._pos++; // keep this up tomorrow
                        }
                        else
                        {
                            CreateNewPlayer(player._pos++);
                        }
                    }
                }
                else
                {
                    //update the score if it is better
                }
            }
            else
            {
                CreateNewPlayer(0);
            }
        }
        else
        {
            Debug.Log("<color=red>One of the variables is null</color>");
        }
    }


    private void CreateNewPlayer(int ID)
    {
        if (!leaderBoardPlayers.ContainsKey(playerName))
        {
            GameObject newPlayer = Instantiate(leaderBoardItem);
            PlayerInfo playerScript = new PlayerInfo
            {
                _pos = ID + 1,
                _pName = playerName,
                _score = score,
                _time = time
            };

            AssignValues(newPlayer, playerScript);
            newPlayer.SetActive(false);
            
            leaderBoardObject.Add(newPlayer);
            leaderBoardPlayers.Add(playerName, playerScript);
        }
        Debug.Log("Player is already in database.");
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

        if (leaderBoardObject.Count > 10)
        {

            for (int i = 0; i < 10; i++)
            {
                if (i != 0)
                {
                    leaderBoardObject[i].transform.position = new Vector3(templatePosition.x, templatePosition.y - 2.61f * i, templatePosition.z);
                }

                leaderBoardObject[i].SetActive(true);
            }
        }
        else if (leaderBoardObject.Count>0 && leaderBoardObject.Count<10)
        {
            for (int i = 0; i < leaderBoardObject.Count; i++)
            {
                if (i != 0)
                {
                    leaderBoardObject[i].transform.position = new Vector3(templatePosition.x, templatePosition.y - 2.61f * i, templatePosition.z);
                }

                leaderBoardObject[i].SetActive(true);
            }
        }
    }
    
    private void AssignValues(GameObject item, PlayerInfo data)
    {
        positionObject = item.transform.Find("Pos").GetComponent<TextMesh>();
        pNameObject = item.transform.Find("PName").GetComponent<TextMesh>();
        scoreObject = item.transform.Find("Score").GetComponent<TextMesh>();
        timeObject = item.transform.Find("Time").GetComponent<TextMesh>();

        positionObject.text = data._pos.ToString();
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
        PlayerPrefs.SetString("LeaderBoard", jsonData);
        PlayerPrefs.Save();
        Debug.Log($"Saved the json data as: {PlayerPrefs.GetString("LeaderBoard")}, jsonData: {jsonData}");
    }

    private void LoadSavedData()
    {
        if (leaderBoardObject.Count == 0)
        {
            string jsonData = PlayerPrefs.GetString("LeaderBoard");
            if (!string.IsNullOrEmpty(jsonData) && !string.IsNullOrWhiteSpace(jsonData))
            {
                ListOfPlayers savedData = JsonUtility.FromJson<ListOfPlayers>(jsonData);
                CreateNewPlayer(savedData.leaderBList);

                AlignLeaderBoardItems();
                Debug.Log($"Json data: {jsonData}");
                Debug.Log("align LeaderBoard Items called");
            }
        }
    }

    [System.Serializable]
    private class PlayerInfo
    {
        public int _pos;
        public string _pName;
        public int _score;
        public string _time;
    }

    private class ListOfPlayers
    {
        public List<PlayerInfo> leaderBList;
    }
}




