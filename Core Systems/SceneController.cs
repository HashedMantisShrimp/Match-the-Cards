using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneController : MonoBehaviour
{
    #region Variables
    
    public bool winGame = false;
    public bool deleteSave = false;
    public bool dontDeleteSave = false;
    internal float time = 0;
    internal int tMins = 00;
    internal float tSecs = 00;
    private const int gridCols = 4;
    private const int gridRows = 3;
    private const float offSetX = 4f;
    private const float offSetY = 5f;
    private bool gameOver = false;
    private GameObject timeText;
    private GameObject leaderBoardButton;
    private GameObject resetLeaderBoard;
    private ScoreManager scoreManager;
    private LeaderBoard leaderBoard;
    private Data saveData;
    private Save save;
    private GameData gameData;
    [SerializeField] private GameObject SaveAndLoad;
    [SerializeField] private GameObject quitInterface;
    [SerializeField] private GameObject gameArea;
    [SerializeField] private GameObject scoreText;
    [SerializeField] private GameObject congrats;
    [SerializeField] private MainCard originalCard;
    [SerializeField] private Sprite[] imgs;

    #endregion

    //---------------------------------------------------------------------------------------------------

    #region Init Functions

    void Start()
    {
        AssignFields();

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
        
        if (quitInterface.activeSelf)
        {
            gameData.SetTime(time);
            Time.timeScale = 0;
            DeactivateCards(true);
            leaderBoardButton.SetActive(false);
            resetLeaderBoard.SetActive(false);
            Database.LoadLeaderBoardData();//TODO: Delete this later, implement it in the proper place
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

    //---------------------------------------------------------------------------------------------------

    #region Private Functions

    private void InitiateGameBoard() //Handles the organization of the game board
    {
        if (gameData.GetSaveDataPresent())
        {
            OrganizeGameBoard(saveData.IDs);
        }
        else
        {
            OrganizeGameBoard();
        }
    }

    private void AssignFields()
    {
        resetLeaderBoard = FindObjectOfType<ResetLeaderBoard>().gameObject;
        leaderBoard = FindObjectOfType<LeaderBoard>();
        timeText = transform.Find("Time Text").gameObject;
        scoreManager = FindObjectOfType<ScoreManager>();
        leaderBoardButton = FindObjectOfType<ToggleButton>().gameObject;

        if(SaveAndLoad==null)
        SaveAndLoad = FindObjectOfType<Save>().gameObject;

        save = SaveAndLoad.GetComponent<Save>();
        gameData = SaveAndLoad.GetComponent<GameData>();
        saveData = GameData.saveData;
        InitiateGameBoard();
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
        gameData.SetIDArray(numbers);

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
                save.AcquireData(counter);
                gameData.AddToCardList(card);
                counter++;

                float posX = (offSetX * i) + startPos.x;
                float posY = (offSetY * j) + startPos.y;

                card.transform.position = new Vector3(posX, posY, startPos.z);
            }
        }
    }

    private void OrganizeGameBoard(int[] idArray) //Organizes the cards according to previous save
    {
        time = saveData.time;
        gameData.SetCurrentMatches(saveData.matches);
        gameData.SetMoveCounter(saveData.moves);
        List<MainCard> cardsToReveal = new List<MainCard>();

        InstantiateCards(idArray, cardsToReveal);
        RevealCards(cardsToReveal);
        cardsToReveal.Clear();
    }

    private void InstantiateCards(int [] idArray, List<MainCard> cardList)
    {
        Vector3 startPos = originalCard.transform.position;
        int counter = 0;
        gameData.SetIDArray(idArray);

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
                card.ChangeSprite(id, imgs[id]);
                gameData.AddToCardList(card);
                counter++;

                float posX = (offSetX * i) + startPos.x;
                float posY = (offSetY * j) + startPos.y;

                card.transform.position = new Vector3(posX, posY, startPos.z);

                if (newCardInfo != null)
                {
                    card.enabled = newCardInfo.scriptEnabled;
                    card.GetComponent<BoxCollider2D>().enabled = newCardInfo.cardBackEnabled;

                    if (!newCardInfo.cardBackEnabled)
                    {
                        cardList.Add(card);
                        //Debug.Log($"Card added to list card script: {card.enabled}", card.gameObject);
                    }
                }
            }
        }
        save.AcquireData(saveData.cards);
    }

    private void RevealCards(List<MainCard> cardList)
    {
        for (int i = 0; i < cardList.Count; i++)//Reveals the cards that were revealed in prev game
        {
            cardList[i].Unreveal(false);

            if (cardList[i].enabled)//If card is revealed and its script is active, then it hasnt found a match yet
            {
                scoreManager.SetPrevCards(cardList[i].gameObject);
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
                scoreManager.SetMatches();

            if (deleteSave)
                save.DeleteSaveData();

            timeText.GetComponent<TextMesh>().text = $"{gameData.GetPlayerName()} - {tMins}:{Mathf.RoundToInt(tSecs).ToString("D2")}";

            if (gameData.GetCurrentMatches() == GameData.totalMatches)
            {
                //assign certain data to gameData here
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

    internal void SaveGame() //Sends data to the Save script & saves the game
    {//TODO: Don't allow game to be saved if it has already been finished
        List<MainCard> cardArray = gameData.GetMainCards();

        if (cardArray != null)
        {
            if (!gameOver)
            {
                for (int i = 0; i < cardArray.Count; i++)
                {
                    bool isEnabled = cardArray[i].enabled;
                    bool isColliderOn = cardArray[i].gameObject.GetComponent<BoxCollider2D>().enabled;
                    bool cardBackEnabled = cardArray[i].GetCardBackState();

                    save.AcquireData(i, isEnabled, isColliderOn, cardBackEnabled);
                }

                save.AcquireData();
                save.SaveData();
            }
        }
    }

    private IEnumerator Congrats() //Handles the Win-state
    {
        SendData();
        yield return new WaitForSeconds(0.3f);
        scoreManager.CalculateScore();
        int score = gameData.GetScore();//Simplify this
        scoreText.GetComponent<TextMesh>().text = $"Score: {score}";
        leaderBoard.SendData($"{tMins}:{Mathf.RoundToInt(tSecs).ToString("D2")}");
        StartCoroutine(DeactivateCards(1f));
        congrats.SetActive(true);

        if(!dontDeleteSave)
        save.DeleteSaveData();
    }

    private IEnumerator DeactivateCards(float waitTime) //Deactivates cards after waitTime
    {
        yield return new WaitForSeconds(waitTime);

        foreach (MainCard card in gameData.GetMainCards())
        {
            card.GetComponent<Transform>().gameObject.SetActive(false);
        }
    }

    private void DeactivateCards(bool deactivate) //Deactivates cards immediately
    {
        foreach (MainCard card in gameData.GetMainCards())
        {
            card.GetComponent<Transform>().gameObject.SetActive(!deactivate);
        }
    }

    private void SendData() // Sends time to ScoreManager to calculate score
    {
        gameData.SetTime(time);
        scoreManager.SetTime();
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