using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Begin : MonoBehaviour
{
    private CameraShake shake;
    private SoundPlayer soundManager;
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
        StartScript();
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

    private async Task StartScript()//To be called when Monobehaviour calls Start()
    {
        await GetLeaderBoardData();
        while (GameData.GetSoundManager() == null)
        {
            Debug.Log($"GetSoundManager returned null, yielding.");
            await Task.Yield();
        }

        Debug.Log($"GetSoundManager is NOT null, proceeding with script.");
        soundManager = GameData.GetSoundManager();
        soundManager.PlaySoundClip("Exp", true);
    }
    
    private async Task GetLeaderBoardData()// Attempts to fetch LeaderBoard data from db
    {
        if (await Internet.CheckInternetConnectivity())
        {
            GameData.SetLeaderBoardJSON(await Database.LoadLeaderBoardData());
        }
    }

    private void OnMouseDown()
    {
        CheckInput();
    }

    private void CheckInput() //checks if player actually wrote something
    {
        if (Misc.IsStringValid(playerName.text))
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
