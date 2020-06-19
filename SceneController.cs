using System.Collections;
using UnityEngine;

public class SceneController : MonoBehaviour
{
    #region Variables

    public bool winGame = false;
    public const int gridCols = 4;
    public const int gridRows = 3;
    public const float offSetX = 4f;
    public const float offSetY = 5f;
    internal float time = 0;
    internal int tMins = 00;
    internal float tSecs = 00;
    private const int totalMatches = 4;
    private string playerName;
    private int score =-1;
    private bool gameOver = false;
    private GameObject timeText;
    private GameObject leaderBoardButton;
    private GameObject resetLeaderBoard;
    private ScoreManager scoreManager;
    private LeaderBoard leaderBoard;
    private Data saveData;
    private SaveGame saveGame;
    private LoadGame loadGame;
    private MainCard[] cardArray;
    [SerializeField] private GameObject SaveAndLoad;
    [SerializeField] private GameObject quitInterface;
    [SerializeField] private GameObject gameArea;
    [SerializeField] private GameObject scoreText;
    [SerializeField] private GameObject congrats;
    [SerializeField] private MainCard originalCard;
    [SerializeField] private Sprite[] imgs;

    #endregion

    #region Init Functions

    private void OnEnable()
    {
        playerName = PlayerPrefs.GetString("playerName");
        playerName = (string.IsNullOrEmpty(playerName) || string.IsNullOrWhiteSpace(playerName)) ? "John Doe" : playerName;
    }

    private void Awake()
    {
        AssignFields();
    }

    void Start()
    {
        loadGame = SaveAndLoad.GetComponent<LoadGame>();

        if (loadGame.saveDataIsPresent)
        {
            OrganizeGameBoard(saveData.IDs);
        }
        else
        {
            OrganizeGameBoard();
        }
        
        if (quitInterface != null)
            quitInterface.SetActive(false);
    }

    void Update()
    {
        UpdateGameStatus();
    }

    #endregion

    internal void ToggleEscape() //Handles the 'paused-game' state
    {
        quitInterface.SetActive(!quitInterface.activeSelf);

        if (!gameOver)
        {
            saveGame.AcquireData(playerName, time, scoreManager.movesCounter, scoreManager.currMatches);
            saveGame.SaveData(playerName);
        }
        

        if (cardArray != null)
        {
            for(int i=0; i<cardArray.Length; i++)
            {
                int id = cardArray[i].id;
                bool isEnabled = cardArray[i].enabled;
                bool isColliderOn = cardArray[i].gameObject.GetComponent<BoxCollider2D>().enabled;
                bool cardBackEnabled = cardArray[i].GetCardBackState();

                saveGame.AcquireData(id, isEnabled, isColliderOn, cardBackEnabled);
            }
        }

        if (quitInterface.activeSelf)
        {
            Time.timeScale = 0;
            DeactivateCards(true);
            leaderBoardButton.SetActive(false);
            resetLeaderBoard.SetActive(false);
        }
        else
        {
            Time.timeScale = 1;
            if(!gameOver)
            DeactivateCards(false);
            leaderBoardButton.SetActive(true);
            resetLeaderBoard.SetActive(true);
        }
    }

    #region Private Functions

    private void AssignFields()
    {
        resetLeaderBoard = FindObjectOfType<ResetLeaderBoard>().gameObject;
        leaderBoard = FindObjectOfType<LeaderBoard>();
        timeText = transform.Find("Time Text").gameObject;
        scoreManager = FindObjectOfType<ScoreManager>();
        leaderBoardButton = FindObjectOfType<ToggleButton>().gameObject;

        if(SaveAndLoad==null)
        SaveAndLoad = FindObjectOfType<SaveGame>().gameObject;

        saveGame = SaveAndLoad.GetComponent<SaveGame>();
    }

    private int[] ShuffleArray(int[] array) //shuffles an array
    {
        int[] newArray = array.Clone() as int[];

        for (int i = 0; i < newArray.Length; i++)
        {
            int temp = newArray[i];

            int r = Random.Range(i, newArray.Length);
            newArray[i] = newArray[r];
            newArray[r] = temp;
        }
        return newArray;
    }

    private void OrganizeGameBoard() //Shuffles the position of card types and organizes them
    {
        Vector3 startPos = originalCard.transform.position;
        int[] numbers = { 0, 0, 0, 1, 1, 1, 2, 2, 2, 3, 3, 3 };
        int counter=0;
        numbers = ShuffleArray(numbers);
        saveGame.AcquireData(numbers);

        for (int i = 0; i < gridCols; i++)
        {
            for (int j = 0; j < gridRows; j++)
            {
                MainCard card;

                if (i == 0 && j == 0)
                {
                    card = originalCard;
                }
                else
                {
                    card = Instantiate(originalCard, gameArea.transform) as MainCard;
                }

                int index = j * gridCols + i;
                int id = numbers[index];

                card.ChangeSprite(id, imgs[id]);
                saveGame.AcquireData(counter);
                counter++;

                float posX = (offSetX * i) + startPos.x;
                float posY = (offSetY * j) + startPos.y;

                card.transform.position = new Vector3(posX, posY, startPos.z);
            }
        }
    }

    private void OrganizeGameBoard(int[] idArray)
    {
        Vector3 startPos = originalCard.transform.position;
        time = saveData.time;
        int counter = 0;

        for (int i = 0; i < gridCols; i++)
        {
            for (int j = 0; j < gridRows; j++)
            {
                MainCard card;
                CardInfo newCardInfo;

                if (i == 0 && j == 0)
                {
                    card = originalCard;
                }
                else
                {
                    card = Instantiate(originalCard, gameArea.transform) as MainCard;
                }

                int index = j * gridCols + i;
                int id = idArray[index];
                newCardInfo = saveData.cards[counter];
                counter++;

                card.ChangeSprite(id, imgs[id]);
                card.enabled = newCardInfo.scriptEnabled;

                if (!newCardInfo.cardBackEnabled)
                {
                    card.Unreveal();
                    scoreManager.SetPrevCards(card.gameObject);
                }
                    
                

                float posX = (offSetX * i) + startPos.x;
                float posY = (offSetY * j) + startPos.y;

                card.transform.position = new Vector3(posX, posY, startPos.z);
            }
        }
    }

    private void UpdateGameStatus() //Checks & handles the 'state' of the game
    {
        CheckPauseStatus();

        if (!gameOver)
        {
            WinCheatCode();

            if (winGame)
                scoreManager.SetMatches(totalMatches);

            timeText.GetComponent<TextMesh>().text = $"{playerName} - {tMins}:{Mathf.RoundToInt(tSecs).ToString("D2")}";

            if (scoreManager.currMatches == totalMatches)
            {
                Debug.Log($"Congratulations You Won! Time: {time}");
                StartCoroutine(Congrats());
                gameOver = true;

            }
            else
            {
                time += Time.deltaTime;
                tSecs = time % 60;
                tMins = (Mathf.RoundToInt(time) % 60 == 0) ? Mathf.RoundToInt(time) / 60 : tMins;
            }
        }
        
    }

    private IEnumerator Congrats() //Handles the Win-state
    {
        SendData();
        yield return new WaitForSeconds(0.3f);
        scoreManager.CalculateScore();
        score = scoreManager.score;
        scoreText.GetComponent<TextMesh>().text = $"Score: {score}";
        leaderBoard.SendData(playerName, score, $"{tMins}:{Mathf.RoundToInt(tSecs).ToString("D2")}");
        StartCoroutine(DeactivateCards(1f));
        congrats.SetActive(true);
    }

    private IEnumerator DeactivateCards(float waitTime) //Deactivates cards after waitTime
    {
        if (cardArray == null)
        {
            cardArray = FindObjectsOfType<MainCard>();
        }
        
        yield return new WaitForSeconds(waitTime);

        foreach (MainCard card in cardArray )
        {
            card.GetComponent<Transform>().gameObject.SetActive(false);
        }
    }

    private void DeactivateCards(bool deactivate) //Deactivates cards immediately
    {
        if (cardArray == null)
        {
            cardArray = FindObjectsOfType<MainCard>();
        }

        foreach (MainCard card in cardArray)
        {
            card.GetComponent<Transform>().gameObject.SetActive(!deactivate);
        }
    }

    private void SendData() // Sends time to ScoreManager to calculate score
    {
        scoreManager.time = time;
    }

    private void CheckPauseStatus() //Checks the 'paused-game' state
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleEscape();
        }
    }

    private void WinCheatCode()
    {
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.W))
        { 
                winGame = true;
        }
    }

    #endregion
}