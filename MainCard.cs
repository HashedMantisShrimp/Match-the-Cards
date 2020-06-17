using UnityEngine;

public class MainCard : MonoBehaviour
{
    internal int id { get; private set; }
    private GameObject cardBack;
    private ScoreManager scoreManager;


    void Start()
    {
        scoreManager = FindObjectOfType<ScoreManager>();
        cardBack = transform.Find("Card_Back").gameObject;
    }

    #region Internal Functions

    internal void ChangeSprite(int id, Sprite img) //Assigns a sprite (type of card) to a card
    {
        this.id = id;
        GetComponent<SpriteRenderer>().sprite = img;
    }

    internal void Unreveal()
    {
        cardBack.SetActive(true);
    }

    #endregion

    private void OnMouseDown() //reveals the type of card
    {
        if (cardBack.activeSelf)
            cardBack.SetActive(false);
        scoreManager.ManageRevealedCards(gameObject); //calls function to check if revealed cards match
    }
    
}
