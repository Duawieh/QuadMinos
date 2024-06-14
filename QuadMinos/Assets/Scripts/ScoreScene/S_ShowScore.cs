using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class S_ShowScore : MonoBehaviour
{
    public AudioClip music_rank1;
    public AudioClip music_rank2;
    public GameObject celebration;
    public GameObject rankBoard;
    public GameObject scorBoard;
    public GameObject exitBoard;

    public GameObject rankText; // 展示排名（多人模式下展示多人排名，否则展示纪录排名）
    public GameObject scorText; // 展示得分（多人模式和40行展示游玩时间，否则展示最终得分）
    public GameObject scrmText; // 展示最高得分（得分展示为时间时此处展示最终得分，否则展示最高得分）
    public GameObject perfText; // 展示表现评分（展示表现评分）

    // Start is called before the first frame update
    void Start()
    {
        ShowScoreInfo();
        StartCoroutine(InitAnimation());
    }

    private void ShowScoreInfo()
    {
        ShowPerformance();
        int GameMode = BattleScore.GameMode;
        if (GameMode == 0) return;
        StartCoroutine(ShowRank(GameMode));
        ShowScore(GameMode);
    }

    private string GetTimeFromSeconds(float _seconds)
    {
        int _H, _M;
        // 计算小时数
        if (_seconds >= 3600.0f)
        {
            _H = Mathf.FloorToInt(_seconds / 3600.0f);
            _seconds -= _H * 3600;
        }
        else _H = 0;
        // 计算分钟数
        if (_seconds >= 60.0f)
        {
            _M = Mathf.FloorToInt(_seconds / 60.0f);
            _seconds -= _M * 60;
        }
        else _M = 0;

        string _h = _H < 10 ? "0" + _H : _H.ToString();
        string _m = _M < 10 ? "0" + _M : _M.ToString();

        if (_H == 0) return _m + " : " + _seconds.ToString("f3");
        return _h + " : " + _m + " : " + _seconds.ToString("f1");
    }

    private void ShowScore(int _mode)
    {
        string _score;
        string _score_max;
        if (_mode == 1 || _mode == 2 || _mode == 5)
        {
            _score = GetTimeFromSeconds(BattleScore._Time);
            _score_max = "(" + BattleScore._Score.ToString() + ")";
        }
        else
        {
            _score = BattleScore._Score.ToString();
            _score_max = "(" + BattleScore._Score_Max.ToString() + ")";
        }
        scorText.GetComponent<Text>().text = _score;
        scrmText.GetComponent<Text>().text = _score_max;
        return;
    }

    private void ShowPerformance()
    {
        string _pps = (BattleScore._Locked / BattleScore._Time).ToString("f3");
        string _lpm = (BattleScore._Lines / BattleScore._Time * 60.0f).ToString("f3");
        string _apm = (BattleScore._Attacked / BattleScore._Time * 60.0f).ToString("f3");
        string _performace = _pps + "\n" + _lpm + "\n" + _apm;
        perfText.GetComponent<Text>().text = _performace;
        return;
    }

    private IEnumerator ShowRank(int _mode)
    {
        StartCoroutine(GetComponent<File_SaveScore>().SaveData(_mode));
        while (true)
        {
            if (GetComponent<File_SaveScore>().RANK != -1) break;
            yield return null;
        }

        int rank = GetComponent<File_SaveScore>().RANK;
        if (rank <= 10)
        {
            rankText.GetComponent<Text>().text = rank.ToString();
        }
        else
        {
            rankText.GetComponent<Text>().text = "10+";
        }
        RankEffects(rank);
        yield break;
    }

    private IEnumerator InitAnimation()
    {
        float _t = 0.0f;

        float _scale = ScaleInScreen.Get_Scale(1.0f, gameObject);

        rankBoard.GetComponent<Image>().color = new Color(0.2f, 0.2f, 0.2f, 1.0f);
        scorBoard.GetComponent<Image>().color = new Color(0.2f, 0.2f, 0.2f, 1.0f);
        exitBoard.GetComponent<Image>().color = new Color(0.2f, 0.2f, 0.2f, 1.0f);

        transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        transform.localPosition = Vector3.zero;
        while (_t < 1.0f)
        {
            _t += Time.deltaTime;

            float _s = Functions.F_paraFadeout(_t, 1.0f, 0.1f, _scale);
            transform.localScale = new Vector3(_s, _s, _s);

            yield return null;
        }

        transform.localScale = new Vector3(_scale, _scale, _scale);

        yield break;
    }

    private void RankEffects(int rank)
    {
        AudioSource ads = GetComponent<AudioSource>();

        if (rank == 1) {
            ads.clip = music_rank1;
            GameObject clb = Instantiate(celebration, transform.parent);
            clb.transform.localPosition = Vector3.zero;
            clb.transform.localScale = Vector3.one;
        }
        else ads.clip = music_rank2;
        ads.volume = GameSettings.MusicVolume;
        ads.Play();

        Color rankColor = Color.HSVToRGB((rank - 1) / 10.0f, 1.0f, Functions.F_paraFadeinout(rank - 1, 10, 1.0f, 0.5f));
        rankBoard.GetComponent<Image>().color = rankColor;
        scorBoard.GetComponent<Image>().color = rankColor;
        exitBoard.GetComponent<Image>().color = rankColor;

        return;
    }
}
