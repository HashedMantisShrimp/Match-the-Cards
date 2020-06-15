using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class LItem : MonoBehaviour
{
    private TextMesh pos;
    private TextMesh pName;
    private TextMesh score;
    private TextMesh time;

    private void Awake()
    {
        pos = transform.Find("Pos").GetComponent<TextMesh>();
        pName = transform.Find("PName").GetComponent<TextMesh>();
        score = transform.Find("Score").GetComponent<TextMesh>();
        time = transform.Find("Time").GetComponent<TextMesh>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    internal void InitData(int _pos, string _pName, int _score, string _time)
    {
        pos.text = _pos.ToString();
        pName.text = _pName;
        score.text = _score.ToString();
        time.text = _time;
    }

}
