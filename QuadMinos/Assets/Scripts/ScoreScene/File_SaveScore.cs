using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class BattleDataInfo
{
    public string _time;
    public float _grade;
}

class HistoryBattleData
{
    /// <summary>
    /// 表示战斗数据的类型
    /// 1 - 计分型（分数越高排名越靠前）(GameMode = 3 / 4)
    /// 2 - 计时型（时间越短排名越靠前）(GameMode = 2)
    /// 3 - 耐力型（时间越长排名越靠前）(GameMode = 5)
    /// </summary>
    public int _type;
    public BattleDataInfo[] infos = null;
}



class BattleDataHeap
{
    /// <summary>
    /// 表示战斗数据的类型
    /// 1 - 计分型（分数越高排名越靠前）(GameMode = 3 / 4)
    /// 2 - 计时型（时间越短排名越靠前）(GameMode = 2)
    /// 3 - 耐力型（时间越长排名越靠前）(GameMode = 5)
    /// </summary>
    public int _type;
    public int Count;
    public BattleDataInfo[] infos;

    public BattleDataHeap()
    {
        Count = 0;
        infos = new BattleDataInfo[20];
        return;
    }

    private void Swap(ref BattleDataInfo a, ref BattleDataInfo b) { (a, b) = (b, a); }

    private bool Earlier(BattleDataInfo a, BattleDataInfo b)
    {
        for (int i = 0; i < a._time.Length; i++)
        {
            if (a._time[i] < b._time[i]) return true;
            if (a._time[i] > b._time[i]) return false;
        }
        return false;
    }

    private bool Better(BattleDataInfo a, BattleDataInfo b)
    {
        if (a._grade == b._grade) return Earlier(a, b);
        if (_type == 2) return a._grade < b._grade;
        else return a._grade > b._grade;
    }

    private void FloatUp(int id)
    {
        int fa = id >> 1;
        if (fa == 0) return;
        if (Better(infos[id], infos[fa]))
        {
            Swap(ref infos[id], ref infos[fa]);
            FloatUp(fa);
        }
        return;
    }

    private void SinkDown(int id)
    {
        int ls = id << 1;
        int rs = id << 1 | 1;
        if (ls <= Count && rs <= Count)
        {
            int betterSon = Better(infos[ls], infos[rs]) ? ls : rs;
            if (Better(infos[betterSon], infos[id]))
            {
                Swap(ref infos[betterSon], ref infos[id]);
                SinkDown(betterSon);
            }
        } 
        else if (ls <= Count)
        {
            if (Better(infos[ls], infos[id]))
            {
                Swap(ref infos[ls], ref infos[id]);
                SinkDown(ls);
            }
        }
        return;
    }

    public void Push(BattleDataInfo info)
    {
        Count++;
        infos[Count] = info;
        FloatUp(Count);
        return;
    }

    public void Pop()
    {
        Swap(ref infos[1], ref infos[Count]);
        infos[Count] = null;
        Count--;
        SinkDown(1);
        return;
    }

    public BattleDataInfo Top()
    {
        return infos[1];
    }
}

public class File_SaveScore : MonoBehaviour
{
    public int RANK = 0;

    public IEnumerator SaveData(int gameMode)
    {
        if (gameMode <= 1 || gameMode >= 5) yield break; // 禅模式和多人模式不纪录

        RANK = -1;      // 开始存储数据，在完成前令 RANK = -1

        // 检查存档数据是否存在，若不存在，新建新存档
        string dataPath = Application.persistentDataPath + "/BattleData/";
        if (!Directory.Exists(dataPath)) Directory.CreateDirectory(dataPath);
        string dataName = "Data" + gameMode + ".json";
        if (!File.Exists(dataPath + "/" + dataName)) 
            InitNewHistoryData(dataPath + "/" + dataName, gameMode);
        yield return null;

        // 从存档读取数据，并加入堆中用于后续排名
        BattleDataHeap historyDatas = GetHistoryData(dataPath + "/" + dataName);
        yield return null;

        // 将本局战斗数据加入排名堆
        float cur_grade;
        if (gameMode == 2) cur_grade = BattleScore._Time;
        else cur_grade = BattleScore._Score;
        BattleDataInfo cur_info = new BattleDataInfo();
        cur_info._time = System.DateTime.Now.ToString("yyyy/MM/dd");
        cur_info._grade = cur_grade;
        historyDatas.Push(cur_info);

        // 获取本局排名，并将新的数据写入硬盘
        HistoryBattleData newData = new HistoryBattleData();
        newData._type = historyDatas._type;
        newData.infos = new BattleDataInfo[10];
        for (int i = 0; i < 10; i++)
        {
            if (historyDatas.Top()._grade == cur_grade)
            {
                if (RANK == -1) RANK = i + 1;
            }
            newData.infos[i] = historyDatas.Top();
            historyDatas.Pop();
        }
        UpdateHistoryData(dataPath + "/" + dataName, newData);

        // 若玩家成绩超出前 10 名，则告知玩家名次为 10+
        if (RANK == -1) RANK = 11;

        yield break;
    }

    private void InitNewHistoryData(string dataPathName, int gameMode)
    {
        HistoryBattleData _data = new HistoryBattleData();

        if (gameMode == 2) _data._type = 2;
        else if (gameMode == 5) _data._type = 3;
        else _data._type = 1;
        _data.infos = new BattleDataInfo[10];
        for (int i = 0; i < 10; i++)
        {
            BattleDataInfo _info = new BattleDataInfo();
            _info._time = "0000/00/00";
            if (_data._type == 2) _info._grade = 1e9f;
            else _info._grade = 0.0f;

            _data.infos[i] = _info;
        }

        string info_js = JsonUtility.ToJson(_data);

        using (StreamWriter _streamWither = new StreamWriter(dataPathName))
        {
            _streamWither.WriteLine(info_js);
            _streamWither.Flush();
            _streamWither.Close();
            _streamWither.Dispose();
        }

        return;
    }

    private void UpdateHistoryData(string dataPathName, HistoryBattleData _data)
    {
        string info_js = JsonUtility.ToJson(_data);

        using (StreamWriter _streamWither = new StreamWriter(dataPathName))
        {
            _streamWither.WriteLine(info_js);
            _streamWither.Flush();
            _streamWither.Close();
            _streamWither.Dispose();
        }

        return;
    }

    private BattleDataHeap GetHistoryData(string dataPathName)
    {
        BattleDataHeap rst = new();

        string _dataInfo = "";
        using (StreamReader _streamReader = File.OpenText(dataPathName))
        {
            _dataInfo = _streamReader.ReadToEnd();
            _streamReader.Close();
            _streamReader.Dispose();
        }

        HistoryBattleData _data = JsonUtility.FromJson<HistoryBattleData>(_dataInfo);
        rst._type = _data._type;
        for (int i = 0; i < _data.infos.Length; i++) rst.Push(_data.infos[i]);

        return rst;
    }

    public void ClickButton_Exit()
    {
        if (RANK == -1) return;
        LoadInfo.SceneName = "MainMenu";
        SceneManager.LoadScene(1);
        return;
    }
}
