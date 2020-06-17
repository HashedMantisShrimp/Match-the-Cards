using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetLeaderBoard : MonoBehaviour
{
    LeaderBoard script;

    private void Awake()
    {
        script = FindObjectOfType<LeaderBoard>();
    }

    private void OnMouseDown() //If clicked, resets the leaderboard list
    {
        PlayerPrefs.DeleteKey(script.key);
        script.ResetLists();
    }
}
