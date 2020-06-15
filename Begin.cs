using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Begin : MonoBehaviour
{
    [SerializeField] private Text playerName;
    [SerializeField] private GameObject errorMessage;
    private CameraShake shake;
    private Scene scene;


    void Start()
    {
        shake = FindObjectOfType<CameraShake>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            CheckInput();
        }
    }

    #region Private Functions

    private void OnMouseDown()
    {
        CheckInput();
    }

    private void CheckInput()
    {
        if (!string.IsNullOrEmpty(playerName.text) && !string.IsNullOrWhiteSpace(playerName.text))
        {
            SceneManager.LoadScene(1);
        }
        else
        {
            shake.ShakeCamera(1f, .1f);
            errorMessage.SetActive(true);
            StartCoroutine(DeactivateMsg(4f, errorMessage));
        }
    }

    private void OnDisable()
    {
        PlayerPrefs.SetString("playerName", playerName.text);
    }

    private IEnumerator DeactivateMsg(float waitTime, GameObject msg)
    {
        yield return new WaitForSeconds(waitTime);
        msg.SetActive(false);
    }
    #endregion
}
