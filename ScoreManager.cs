using System.Collections;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    internal int currMatches { get; private set; } = 0;
    internal int movesCounter = 0;
    private GameObject prevCard = null;
    [SerializeField] private GameObject scoreText;
    

    void Update()
    {
        scoreText.GetComponent<TextMesh>().text = $"Moves: {movesCounter}";
    }

    internal void ManageRevealedCards(GameObject card)
    {
        if (prevCard == null)
        {
            prevCard = card;
            prevCard.GetComponent<BoxCollider2D>().enabled = false;
        }
        else
        {
            CheckForMatch(card);
        }
    }

    #region Private Functions

    private void CheckForMatch(GameObject currentCard)
    {
        string prevCardName = prevCard.GetComponent<SpriteRenderer>().sprite.name;
        string currCardName = currentCard.GetComponent<SpriteRenderer>().sprite.name;
        int prevCardID = prevCard.GetInstanceID();
        int currCardID = currentCard.GetInstanceID();

        if (currCardName == prevCardName && currCardID != prevCardID)
        {
            prevCard.GetComponent<MainCard>().enabled = false;
            prevCard.GetComponent<BoxCollider2D>().enabled = false;
            currentCard.GetComponent<MainCard>().enabled = false;
            currentCard.GetComponent<BoxCollider2D>().enabled = false;

            currMatches++;
            Debug.Log($"Number of Matches: {currMatches}");
            prevCard = null;
        }
        else
        {
            StartCoroutine(UnrevealCards(0.5f, currentCard));
            
        }

        movesCounter++;
    }

    private IEnumerator UnrevealCards(float waitTime, GameObject currCard)
    { //Unreveals cards

        yield return new WaitForSeconds(waitTime);
        prevCard.GetComponent<BoxCollider2D>().enabled = true;
        prevCard.GetComponent<MainCard>().Unreveal();
        currCard.GetComponent<MainCard>().Unreveal();

        prevCard = null;
    }

    #endregion
}
