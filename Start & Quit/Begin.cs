using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Begin : MonoBehaviour
{
    private CameraShake shake;
    private GameData gameData;
    [SerializeField] private Text playerName;
    [SerializeField] private GameObject errorMessage;

    private void Awake()
    {
        shake = FindObjectOfType<CameraShake>();
        Database.GetInstance();
        GameData.GetInstance();
        //Database.TestConnection();
    }

    void Start()
    {
        GetLeaderBoardData();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            CheckInput();
        }
    }
    
    //---------------------------------------------------------------------------------------------------

    #region Private Functions

    private async Task GetLeaderBoardData()
    {
        //Debug.Log($"{nameof(gameData.SetLeaderBoardJSON)} was called. leaderboardJSON should be set.");
        GameData.SetLeaderBoardJSON(await Database.LoadLeaderBoardData());
    }

    private void OnMouseDown()
    {
        CheckInput();
    }

    private void CheckInput() //checks if player actually wrote something
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

    private IEnumerator DeactivateMsg(float waitTime, GameObject msg) //disables errorMessage after waitTime
    {
        yield return new WaitForSeconds(waitTime);
        msg.SetActive(false);
    }
    #endregion
}
