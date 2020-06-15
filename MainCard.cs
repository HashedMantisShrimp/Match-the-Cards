using UnityEngine;

public class MainCard : MonoBehaviour
{
    private GameObject cardBack;
    private ScoreManager scoreManager;
    private int _id; //simplify this?
    public int id { get { return id; } }


    void Start()
    {
        scoreManager = FindObjectOfType<ScoreManager>();
        cardBack = transform.Find("Card_Back").gameObject;
    }

    #region Internal Functions
    internal void ChangeSprite(int id, Sprite img)
    {
        _id = id;
        GetComponent<SpriteRenderer>().sprite = img;
    }

    internal void Unreveal()
    {
        cardBack.SetActive(true);
    }
    #endregion

    private void OnMouseDown()
    {
        if (cardBack.activeSelf)
            cardBack.SetActive(false);
        scoreManager.ManageRevealedCards(gameObject);
    }
    
}
