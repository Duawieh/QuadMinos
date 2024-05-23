using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class S_File_ShowRecords : MonoBehaviour
{
    public GameObject Score_40Line;
    public GameObject Score_Blitz;
    public GameObject Score_Marathon;
    public GameObject Date_40Line;
    public GameObject Date_Blitz;
    public GameObject Date_Marathon;

    // Start is called before the first frame update
    void Start()
    {
        ShowRecords();
    }

    private void ShowRecords()
    {
        // 显示 四十行 模式的历史最佳成绩（若有同成绩则取时间最早者（时间排序在保存时处理））
        BattleDataInfo _data_40line = GetTopRecord(2);
        if (_data_40line._time != "0000/00/00") {
            Score_40Line.GetComponent<Text>().text = GetTimeFromSeconds(_data_40line._grade);
            Date_40Line.GetComponent<Text>().text = _data_40line._time;
        } 
        else
        {
            Score_40Line.GetComponent<Text>().text = "";
            Date_40Line.GetComponent<Text>().text = "";
        }

        // 显示 闪电战 模式的历史最佳成绩（若有同成绩则取时间最早者（时间排序在保存时处理））
        BattleDataInfo _data_blitz = GetTopRecord(3);
        if (_data_blitz._time != "0000/00/00")
        {
            Score_Blitz.GetComponent<Text>().text = _data_blitz._grade.ToString("f0");
            Date_Blitz.GetComponent<Text>().text = _data_blitz._time;
        }
        else
        {
            Score_Blitz.GetComponent<Text>().text = "";
            Date_Blitz.GetComponent<Text>().text = "";
        }

        // 显示 闪电战 模式的历史最佳成绩（若有同成绩则取时间最早者（时间排序在保存时处理））
        BattleDataInfo _data_marathon = GetTopRecord(4);
        if (_data_marathon._time != "0000/00/00")
        {
            Score_Marathon.GetComponent<Text>().text = _data_marathon._grade.ToString("f0");
            Date_Marathon.GetComponent<Text>().text = _data_marathon._time;
        }
        else
        {
            Score_Marathon.GetComponent<Text>().text = "";
            Date_Marathon.GetComponent<Text>().text = "";
        }
    }

    private BattleDataInfo GetTopRecord(int _gameMode)
    {
        BattleDataInfo topRecord = new();
        topRecord._time = "0000/00/00";

        HistoryBattleData historyData;
        string dataPath = Application.persistentDataPath + "/BattleData/Data" + _gameMode + ".json";
        string info_js;

        if (!File.Exists(dataPath)) return topRecord;

        using (StreamReader _streamReader = new StreamReader(dataPath))
        {
            info_js = _streamReader.ReadToEnd();
            _streamReader.Close();
            _streamReader.Dispose();
        }

        historyData = JsonUtility.FromJson<HistoryBattleData>(info_js);
        topRecord = historyData.infos[0];

        return topRecord;
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
}
