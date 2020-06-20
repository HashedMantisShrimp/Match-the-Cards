using UnityEngine;

public class MainCard : MonoBehaviour
{
    internal int id { get; private set; }
    private GameObject cardBack;
    private ScoreManager scoreManager;

    private void Awake()
    {
        scoreManager = FindObjectOfType<ScoreManager>(); //simplify this line of code
        cardBack = transform.Find("Card_Back").gameObject;
    }

    #region Internal Functions

    internal void ChangeSprite(int id, Sprite img) //Assigns a sprite (type of card) to a card
    {
        this.id = id;
        GetComponent<SpriteRenderer>().sprite = img;
    }

    internal void Unreveal(bool unreveal)
    {
        cardBack.SetActive(unreveal);
    }

    internal bool GetCardBackState()
    {
        return cardBack.activeSelf;
    }

    #endregion

    private void OnMouseDown() //reveals the type of card
    {
        if (cardBack.activeSelf)
            Unreveal(false);
        scoreManager.ManageRevealedCards(gameObject); //calls function to check if revealed cards match
    }
    
}
