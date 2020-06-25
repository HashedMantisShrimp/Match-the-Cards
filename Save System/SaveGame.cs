using UnityEngine;

public class SaveGame : MonoBehaviour
{
   [SerializeField] private SceneController sceneController;

    private void Start()
    {
        if (!sceneController)
            sceneController = FindObjectOfType<SceneController>();
    }

    private void OnMouseDown()
    {
        sceneController.SaveGame();
        gameObject.SetActive(false);
    }
}
