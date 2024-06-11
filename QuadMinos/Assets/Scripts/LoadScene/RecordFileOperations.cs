using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;


/// <summary>
/// 玩家进行的操作的记录
/// </summary>
[Serializable]
public class Operations
{
    public int opt;         // 操作的种类
    public float _t;        // 操作进行的时间

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


// 用于存储记录的可序列化类，存取时与 Json 文件相互转化
[Serializable]
public class SerializedRecord
{
    public List<int> minosOrder;
    public List<Operations> operatesOrder;
    public List<AttackRecord> attackOrder;
    public int GameMode;
}


public class RecordFileOperations {
    private static string GetTime()
    {
        string rst = DateTime.Now.ToString();
        rst = rst.Replace('/', '.');
        rst = rst.Replace(':', '-');
        return rst;
    }

    private static void JsonInfoToClass(SerializedRecord _data) {
        BattleRecords.reviewMinosOrder = _data.minosOrder;
        BattleRecords.reviewOperatesOrder = _data.operatesOrder;
        BattleRecords.reviewAttackOrder = _data.attackOrder;
        BattleRecords.GameMode = _data.GameMode;
        return;
    }

    private static SerializedRecord ClassToJsonInfo()
    {
        SerializedRecord rst = new();
        rst.minosOrder = BattleRecords.minosOrder;
        rst.operatesOrder = BattleRecords.operatesOrder;
        rst.attackOrder = BattleRecords.attackOrder;
        rst.GameMode = BattleRecords.GameMode;

        foreach (Operations i in rst.operatesOrder)
        {
            i._t /= 5;
        }

        return rst;
    }

    /// <summary>
    /// 从 persistentDataPath 中读取设置文件并存入类内
    /// </summary>
    public static void LoadRecords(string _fileName) {
        string pth = Application.persistentDataPath + "/BattleRecords";
        if (!Directory.Exists(pth)) Directory.CreateDirectory(pth);
        pth += "/" + _fileName;

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
    public static void SaveRecords() {
        string _dataPathName = Application.persistentDataPath + "/BattleRecords";
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
}
