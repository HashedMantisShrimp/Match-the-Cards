using UnityEngine;

public class ResetLeaderBoard : MonoBehaviour
{
    [SerializeField] private LeaderBoard script;

    private void Awake()
    {
        if(!script)
        script = FindObjectOfType<LeaderBoard>();
    }

    private void OnMouseDown() //If clicked, resets the leaderboard list
    {
        PlayerPrefs.DeleteKey(script.key);
        script.ResetLists();
    }
}
