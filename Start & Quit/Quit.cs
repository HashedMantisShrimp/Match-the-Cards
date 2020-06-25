using System;
using UnityEngine;

public class Quit : MonoBehaviour
{
    [SerializeField] private SceneController sceneController;
    [SerializeField] private GameObject saveGameButton;

    private void Awake()
    {
        if(!sceneController)
        sceneController = FindObjectOfType<SceneController>();
    }

    private void OnEnable()
    {
        saveGameButton.SetActive(true);
    }

    private void OnMouseDown() //checks if yes or no is pressed and acts accordingly
    {
        if (gameObject.name.Equals("yes", StringComparison.OrdinalIgnoreCase))
        {
            Application.Quit();
        }
        else
        {
            sceneController.ToggleEscape();
        }
    }
}