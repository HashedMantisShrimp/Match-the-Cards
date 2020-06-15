using System.Collections;
using UnityEngine;

public class SceneController : MonoBehaviour
{
    #region Variables
    public const int gridCols = 4;
    public const int gridRows = 3;
    public const float offSetX = 4f;
    public const float offSetY = 5f;
    internal float time = 0;
    internal int tMins = 00;
    internal float tSecs = 00;
    private GameObject timeText;
    private ScoreManager scoreManager;
    private const int totalMatches = 4;
    private string playerName;
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
        timeText = transform.Find("Time Text").gameObject;
        scoreManager = FindObjectOfType<ScoreManager>();
    }

    void Start()
    {
        OrganizeGameBoard();
    }

    private void Update()
    {
        UpdateGameStatus();
    }

    #endregion

    #region Private Functions

    private int[] ShuffleArray(int[] array)
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

    private void OrganizeGameBoard()
    {
        Vector3 startPos = originalCard.transform.position;
        int[] numbers = { 0, 0, 0, 1, 1, 1, 2, 2, 2, 3, 3, 3 };
        numbers = ShuffleArray(numbers);

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

                float posX = (offSetX * i) + startPos.x;
                float posY = (offSetY * j) + startPos.y;

                card.transform.position = new Vector3(posX, posY, startPos.z);
            }
        }
    }

    private void UpdateGameStatus()
    {
        timeText.GetComponent<TextMesh>().text = $"{playerName} - {tMins}:{Mathf.RoundToInt(tSecs).ToString("D2")}";

        if (scoreManager.currMatches == totalMatches)
        {
            Debug.Log($"Congratulations You Won! Time: {time}");
            StartCoroutine(Congrats());
        }
        else
        {
            time += Time.deltaTime;
            tSecs = time % 60;
            tMins = (Mathf.RoundToInt(time) % 60 == 0) ? Mathf.RoundToInt(time) / 60 : tMins;
        }
    }

    private IEnumerator Congrats()
    {
        SendData();
        yield return new WaitForSeconds(0.3f);
        scoreManager.CalculateScore();
        scoreText.GetComponent<TextMesh>().text = $"Score: {scoreManager.score}";
        StartCoroutine(DeactivateCards(1f));
        congrats.SetActive(true);
        

        
        //implement code to save information into a file or playerPrefs
    }

    private IEnumerator DeactivateCards(float waitTime)
    {
        MainCard[] cardArray = FindObjectsOfType<MainCard>();

        yield return new WaitForSeconds(waitTime);
        foreach (MainCard card in cardArray )
        {
            card.GetComponent<Transform>().gameObject.SetActive(false);
        }
    }

    private void SendData()
    {
        scoreManager.time = time;
    }

    #endregion

}
