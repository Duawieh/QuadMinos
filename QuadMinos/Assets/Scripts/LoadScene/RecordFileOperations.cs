using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using UnityEngine;


/// <summary>
/// 玩家进行的操作的记录
/// </summary>
public class Operations
{
    public int opt;         // 操作的种类
    public float _t;        // 操作进行的时间

    public Operations() {
        this.opt = 0;
        this._t = 0f;
    }

    public Operations(int opt, float _t)
    {
        this.opt = opt;
        this._t = _t;
    }
}


/// <summary>
/// 玩家产生的伤害的记录
/// </summary>
[Serializable]
public class AttackRecord
{
    public int dmg;         // 产生的伤害量
    public int ept;         // 垃圾行空列横坐标
    public int tgt;         // 攻击目标索引
    public int frm;         // 攻击来源索引

    public AttackRecord(int dmg, int ept, int tgt)
    {
        this.dmg = dmg;
        this.ept = ept;
        this.tgt = tgt;
        this.frm = 0;
    }

    public AttackRecord(int dmg, int ept, int tgt, int frm)
    {
        this.dmg = dmg;
        this.ept = ept;
        this.tgt = tgt;
        this.frm = frm;
    }
}


/// <summary>
/// 读取到的和要保存的游戏录像信息
/// </summary>
public static class BattleRecords
{
    public static List<int> minosOrder;
    public static List<int> reviewMinosOrder;
    public static List<Operations> operatesOrder;       // 操作记录表，无论录制状态如何都应在游戏内进行记录 
    public static List<Operations> reviewOperatesOrder;
    public static List<AttackRecord> attackOrder;
    public static List<AttackRecord> reviewAttackOrder;
    public static int GameMode;

    public static int operatesIndex;    // 操作记录列表的当前读取索引
    public static int minosIndex;       // 块序记录列表的当前读取索引
    public static int attackIndex;      // 攻击记录列表的当前读取索引

    public static bool saved;           // 是否完成保存
    public static bool loaded;          // 是否完成加载

    public static void Clear()
    {
        minosOrder = new List<int>();
        reviewMinosOrder = new List<int>();
        operatesOrder = new List<Operations>();
        reviewOperatesOrder = new List<Operations>();
        attackOrder = new List<AttackRecord>();
        reviewAttackOrder = new List<AttackRecord>();
        GameMode = 1;
        operatesIndex = 0;
        minosIndex = 0;
        attackIndex = 0;
    }
}


[Serializable]
public class JsonOperations {
    public int o;
    public string T;

    public JsonOperations() {
        this.o =  0;
        this.T = "";
    }

    public JsonOperations(int o, string T) {
        this.o = o;
        this.T = T;
    }

    public float StringToFloat(object FloatString) {
        float result;
        if (FloatString != null) {
            if (float.TryParse(FloatString.ToString(), out result)) return result;
            else return 0.0f;
        }
        else return 0.0f;
    }
}


// 用于存储记录的可序列化类，存取时与 Json 文件相互转化
[Serializable]
public class SerializedRecord
{
    public List<int> minosOrder;
    public List<JsonOperations> operatesOrder;
    public List<AttackRecord> attackOrder;
    public int GameMode;

    public SerializedRecord() {
        this.minosOrder = new();
        this.operatesOrder = new();
        this.attackOrder = new();
        this.GameMode = 0;
    }
}


public class RecordFileOperations {
    private static string[] gameModeFoldersName = { "/unkown", "/ZEN", "/40L", "/BLZ", "/MRT" };

    /// <summary>
    /// 获取归一化的时间字符串，使时间信息符合 19 字符长的文件名格式。
    /// </summary>
    /// <remarks>
    /// 归一化后结果将使时间符合 yyyy.MM.dd HH-mm-ss的格式。
    /// </remarks>
    private static string GetTime()
    {
        DateTime currentTime = DateTime.Now;
        string rst = currentTime.ToString("yyyy.MM.dd HH-mm-ss");
        return rst;
    }

    private static void JsonInfoToClass(SerializedRecord _data) {
        BattleRecords.reviewOperatesOrder = new();

        // 由于压缩了浮点数精度，Json 中的 JsonOperations 类的时间以字符串形式存储
        // 从 Json 中提取数据之前需要先解字符串，从 JsonOperations 类转为 Operations 类
        foreach (JsonOperations i in _data.operatesOrder) {
            if (i.T == "") continue;
            Operations Opt = new();
            Opt.opt = i.o;
            Opt. _t = i.StringToFloat(i.T);
            BattleRecords.reviewOperatesOrder.Add(Opt);
        }

        BattleRecords.reviewMinosOrder = _data.minosOrder;
        BattleRecords.reviewAttackOrder = _data.attackOrder;
        BattleRecords.GameMode = _data.GameMode;

        return;
    }

    private static SerializedRecord ClassToJsonInfo()
    {
        SerializedRecord rst = new();
        rst.minosOrder = BattleRecords.minosOrder;
        rst.attackOrder = BattleRecords.attackOrder;
        rst.GameMode = BattleRecords.GameMode;

        // 时间仅保留至小数点后第三位
        // 因为帧率 60 的前提下，后面的位数没有意义
        foreach (Operations i in BattleRecords.operatesOrder)
        {
            JsonOperations JOJO = new();
            JOJO.o = i.opt;
            JOJO.T = i._t.ToString("f3");
            rst.operatesOrder.Add(JOJO);
        }

        return rst;
    }

    /// <summary>
    /// 从 persistentDataPath 中读取记录文件并存入类内
    /// </summary>
    public static void LoadRecords(int _gameMode, string _fileName) {
        string pth = Application.persistentDataPath 
            + "/BattleRecords" + gameModeFoldersName[_gameMode];
        if (!Directory.Exists(pth)) Directory.CreateDirectory(pth);
        pth += "/" + _fileName + ".json";

        if (!File.Exists(pth))
        {
            Debug.Log("读取失败：记录不存在。");
            return;
        }

        SerializedRecord _data;
        string info_js = "";

        using (StreamReader _streamReader = new StreamReader(pth))
        {
            info_js = _streamReader.ReadToEnd();
            _streamReader.Close();
            _streamReader.Dispose();
        }

        _data = JsonUtility.FromJson<SerializedRecord>(info_js);
        JsonInfoToClass(_data);

        BattleRecords.loaded = true;
        return;
    }

    /// <summary>
    /// 将本局游戏录制的记录存入 persistentDataPath 中
    /// </summary>
    public static void SaveRecords(int _gameMode) {
        string _dataPathName = Application.persistentDataPath
            + "/BattleRecords" + gameModeFoldersName[_gameMode];
        if (!Directory.Exists(_dataPathName)) Directory.CreateDirectory(_dataPathName);
        string currentTime = GetTime();
        string _filePathName = _dataPathName + "/" + currentTime + ".json";

        SerializedRecord _record = ClassToJsonInfo();
        string info_js = JsonUtility.ToJson(_record);

        using (StreamWriter _streamWriter = new StreamWriter(_filePathName))
        {
            _streamWriter.WriteLine(info_js);
            _streamWriter.Flush();
            _streamWriter.Close();
            _streamWriter.Dispose();
        }

        BattleRecords.saved = true;
        return;
    }

    /// <summary>
    /// 从 persistentDataPath 中删除记录文件
    /// </summary>
    public static void DeleteRecords(int _gameMode, string _fileName) {
        string _dataPathName = Application.persistentDataPath 
            + "/BattleRecords" + gameModeFoldersName[_gameMode];
        if (!Directory.Exists(_dataPathName)) {
            Debug.Log("删除失败：路径不存在");
        }
        string _filePathName = _dataPathName + "/" + _fileName + ".json";
        if (!File.Exists(_filePathName)) {
            Debug.Log("删除失败：记录不存在");
        }
        File.Delete(_filePathName);
        return;
    }

    public static List<string> GetAllRecords(int _gameMode) {
        string recordsFolderPath = Application.persistentDataPath + "/BattleRecords" + gameModeFoldersName[_gameMode];
        if (!Directory.Exists(recordsFolderPath)) return new();

        string[] fileNames = Directory.GetFiles(recordsFolderPath, "*.json");
        List<string> rst = new();
        foreach (string fullname in fileNames) {
            rst.Add(Path.GetFileNameWithoutExtension(fullname));
        }
        rst.Reverse();
        
        return rst;
    }
}
