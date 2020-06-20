using System.Collections;
using System;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    internal int currMatches { get; private set; }
    internal int score { get; private set; }
    internal int movesCounter;
    internal float time;
    private Data savedData;
    private GameObject prevCard1;
    private GameObject prevCard2;
    [SerializeField] private LoadGame loadGame;
    [SerializeField] private GameObject movesText;

    void Start()
    {
        AssignFields();
    }

    void Update()
    {
        movesText.GetComponent<TextMesh>().text = $"Moves: {movesCounter}";
    }

    #region Internal Functions

    internal void ManageRevealedCards(GameObject card) //Manages the revealed cards
    {
        if (prevCard1 == null)
        {
            prevCard1 = card;
            prevCard1.GetComponent<BoxCollider2D>().enabled = false;
        }
        else if (prevCard2 == null)
        {
            prevCard2 = card;
            prevCard2.GetComponent<BoxCollider2D>().enabled = false;
        }
        else
        {
            CheckForMatch(card);
        }
    }

    internal void CalculateScore() //Calculates player score
    {
        if (time != -1)
        {
            int secs = Mathf.RoundToInt(Mathf.RoundToInt(time));
            score = (movesCounter * 5) + secs;

            Debug.Log($"Seconds: {secs}, score: {score}");
            return;
        }

        Debug.Log("<color=red>Time has not been updated</color>. returning -1.");
        score = -1;
    }

    internal void SetMatches(int totalMatches) //Checks if all matches have been found
    {
        currMatches = totalMatches;
    }

    internal void SetPrevCards(GameObject _prevCard)
    {
        if (prevCard1 == null)
        {
            prevCard1 = _prevCard;
        }
        else if (prevCard2 == null)
        {
            prevCard2 = _prevCard;
        }
        else 
        {
            prevCard1 = null;
            prevCard2 = null;
        }
    }

    #endregion

    #region Private Functions

    private void AssignFields()
    {
        if (loadGame == null)
            loadGame = FindObjectOfType<LoadGame>();

        if (loadGame.saveDataIsPresent)
            savedData = loadGame.SendData();

        if (savedData != null)
        {
            movesCounter = savedData.moves;
            currMatches = savedData.matches;
        }
        else
        {
            movesCounter = 0;
            currMatches = 0;
            prevCard1 = null;
            prevCard2 = null;
        }
        time = -1;
    }

    private void CheckForMatch (GameObject currentCard) //Checks if revealed cards match
    {
        string prevCardName1 = prevCard1.GetComponent<SpriteRenderer>().sprite.name;
        string prevCardName2 = prevCard2.GetComponent<SpriteRenderer>().sprite.name;
        string currCardName = currentCard.GetComponent<SpriteRenderer>().sprite.name;

        int prevCardID1 = prevCard1.GetInstanceID();
        int prevCardID2 = prevCard2.GetInstanceID();
        int currCardID = currentCard.GetInstanceID();

        if (currCardName.Equals(prevCardName1, StringComparison.OrdinalIgnoreCase) && currCardName.Equals(prevCardName2, StringComparison.OrdinalIgnoreCase)
            && (currCardID != prevCardID1) && (currCardID != prevCardID2) && (prevCardID1 != prevCardID2))
        {
            prevCard1.GetComponent<MainCard>().enabled = false;
            prevCard1.GetComponent<BoxCollider2D>().enabled = false;

            prevCard2.GetComponent<MainCard>().enabled = false;
            prevCard2.GetComponent<BoxCollider2D>().enabled = false;

            currentCard.GetComponent<MainCard>().enabled = false;
            currentCard.GetComponent<BoxCollider2D>().enabled = false;

            currMatches++;
            Debug.Log($"Number of Matches: {currMatches}");
            prevCard1 = null;
            prevCard2 = null;
        }
        else
        {
            StartCoroutine(UnrevealCards(0.5f, currentCard));
        }

        movesCounter++;
    }

    private IEnumerator UnrevealCards(float waitTime, GameObject currCard) //Unreveals cards after waitTime, called when 3 cards have been revealed
    { 

        yield return new WaitForSeconds(waitTime);
        prevCard1.GetComponent<BoxCollider2D>().enabled = true;
        prevCard1.GetComponent<MainCard>().Unreveal(true);

        prevCard2.GetComponent<BoxCollider2D>().enabled = true;
        prevCard2.GetComponent<MainCard>().Unreveal(true);

        currCard.GetComponent<MainCard>().Unreveal(true);

        prevCard1 = null;
        prevCard2 = null;
    }
    #endregion
}