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

    private void OnMouseDown()
    {
        PlayerPrefs.DeleteKey(script.key);
        script.ResetLists();
    }
}
