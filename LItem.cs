using UnityEngine;

[System.Serializable]
public class LItem : MonoBehaviour
{
    [SerializeField] private TextMesh position;
    [SerializeField] private TextMesh pName;
    [SerializeField] private TextMesh score;
    [SerializeField] private TextMesh time;

    internal int _pos { get; set; }
    internal string _pName { get; private set; }
    internal int _score { get; private set; }
    internal string _time { get; private set; }

    private void AssignValues()
    {
        position = transform.Find("Pos").GetComponent<TextMesh>();
        pName = transform.Find("PName").GetComponent<TextMesh>();
        score = transform.Find("Score").GetComponent<TextMesh>();
        time = transform.Find("Time").GetComponent<TextMesh>();
    }

    private void SetInitData()
    {
        AssignValues();
        //SetPos();
        pName.text = _pName;
        score.text = _score.ToString();
        time.text = _time;
    }

    internal void InitData(int pos, string pName, int score, string time)
    {
        _pos = pos;
        _pName = pName;
        _score = score;
        _time = time;

        SetInitData();
    }

    private void SetPos()
    {
        position.text = _pos.ToString();
    }

}
