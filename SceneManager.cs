using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    public const int gridCols = 4;
    public const int gridRows = 2;
    public const float offSetX = 4f;
    public const float offSetY = 5f;
    internal float time = 0;
    internal int tMins = 00;
    internal float tSecs = 00;
    private GameObject timeText;
    private ScoreManager scoreManager;
    private const int totalMatches = 4;
    [SerializeField] private MainCard originalCard;
    [SerializeField] private Sprite[] imgs;

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

        timeText.GetComponent<TextMesh>().text = $"Time - {tMins}:{Mathf.RoundToInt(tSecs).ToString("D2")}";
        // Debug.Log($"tMins: {tMins}, tMins Rounded: {Mathf.RoundToInt(tMins).ToString()}, time: {time}");

        if (scoreManager.currMatches == totalMatches)
        {
            Debug.Log("Congratulations You Won!");
        }
        else
        {
            time += Time.deltaTime;
            tSecs = time % 60;
            tMins = (Mathf.RoundToInt(time) % 60 == 0) ? Mathf.RoundToInt(time) / 60 : tMins;
        }

    }

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
        int[] numbers = { 0, 0, 1, 1, 2, 2, 3, 3 };
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
                    card = Instantiate(originalCard) as MainCard;
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
    
    
}
