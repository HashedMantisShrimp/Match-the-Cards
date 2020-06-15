using UnityEngine.SceneManagement;
using UnityEngine;

public class Reset : MonoBehaviour
{
    private void OnMouseDown()
    {
        Debug.Log("Restarting game");//Implement code to load the first scene
        SceneManager.LoadScene(0);
    }
}
