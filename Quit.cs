using System;
using UnityEngine;

public class Quit : MonoBehaviour
{
    private SceneController sceneController;

    private void Awake()
    {
        sceneController = FindObjectOfType<SceneController>();
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