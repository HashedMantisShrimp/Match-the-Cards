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
    private Dictionary<string, LItem> leaderBoardPlayers = new Dictionary<string, LItem>();
    private List<GameObject> leaderBoardObject = new List<GameObject>();
    [SerializeField] private GameObject leaderBoardItem;

    // Start is called before the first frame update

    private void Awake()
    {
        //LoadSavedData();
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

                    foreach (LItem player in leaderBoardPlayers.Values)
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

    private void AlignLeaderBoardItems()
    {
        Vector2 templatePosition = leaderBoardItem.transform.position;

        if (leaderBoardObject.Count > 10)
        {

            for (int i = 0; i < 10; i++)
            {
                if (i != 0)
                {
                    leaderBoardObject[i].transform.position = new Vector2(templatePosition.x, templatePosition.y - 1.57f * i);
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
                    leaderBoardObject[i].transform.position = new Vector2(templatePosition.x, templatePosition.y - 1.57f * i);
                }

                leaderBoardObject[i].SetActive(true);
            }
        }
    }

    private void CreateNewPlayer(int ID)
    {
        if (!leaderBoardPlayers.ContainsKey(playerName))
        {
            GameObject newPlayer = Instantiate(leaderBoardItem);
            newPlayer.TryGetComponent(out LItem script);
            newPlayer.SetActive(false);

            script.InitData(ID + 1, playerName, score, time);
            leaderBoardObject.Add(newPlayer);
            leaderBoardPlayers.Add(playerName, script);
        }
        Debug.Log("Player is already in database.");
    }

    private void SaveInfo()
    {
        ListOfPlayers playerList = new ListOfPlayers { leaderBList = leaderBoardPlayers };
        string jsonData = JsonUtility.ToJson(playerList);
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
                leaderBoardObject = JsonUtility.FromJson<List<GameObject>>(jsonData);
                AlignLeaderBoardItems();
                Debug.Log($"Json data: {jsonData}");
                Debug.Log("align LeaderBoard Items called");
            }
        }
    }
}

public class ListOfPlayers
{
    internal Dictionary<string, LItem> leaderBList;
}
