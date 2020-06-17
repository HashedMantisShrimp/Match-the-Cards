using UnityEngine.SceneManagement;
using UnityEngine;

public class RestartGame : MonoBehaviour
{
    private void OnMouseDown() //Loads the initial scene
    {
        Debug.Log("Restarting game");
        SceneManager.LoadScene(0);
    }
}
