using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ToggleButton : MonoBehaviour
{
    private Color currColor;
    [SerializeField] private GameObject gameArea;
    [SerializeField] private TextMesh buttonText;

    private void Start()
    {
        currColor = GetComponent<SpriteRenderer>().color;
    }

    private void OnMouseDown()
    {
        gameArea.SetActive(!gameArea.activeSelf);

        if (!gameArea.activeSelf)
        {
            buttonText.text = "Back to Game";
            GetComponent<SpriteRenderer>().color = Color.yellow;
        }
        else
        {
            buttonText.text = "LeaderBoard";
            GetComponent<SpriteRenderer>().color = currColor;
        }
    }
}
