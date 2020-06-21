using UnityEngine;

public class ToggleButton : MonoBehaviour
{
    private Color currColor;
    private GameObject resetLeaderBoard;
    [SerializeField] private GameObject gameArea;
    [SerializeField] private TextMesh buttonText;

    private void Awake()
    {
        currColor = GetComponent<SpriteRenderer>().color;
        resetLeaderBoard = FindObjectOfType<ResetLeaderBoard>().gameObject;
    }

    private void Start()
    {
        resetLeaderBoard.SetActive(false);
    }

    private void OnMouseDown() //Handles the toggling of the LeaderBoard button
    {
        gameArea.SetActive(!gameArea.activeSelf);

        if (!gameArea.activeSelf)
        {
            buttonText.text = "Back to Game";
            resetLeaderBoard.SetActive(true);
            Time.timeScale = 0;
            GetComponent<SpriteRenderer>().color = Color.yellow;
        }
        else
        {
            buttonText.text = "LeaderBoard";
            resetLeaderBoard.SetActive(false);
            Time.timeScale = 1;
            GetComponent<SpriteRenderer>().color = currColor;
        }
    }
}
