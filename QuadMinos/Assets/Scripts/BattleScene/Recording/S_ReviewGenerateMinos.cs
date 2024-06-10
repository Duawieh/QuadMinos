using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 玩家进行的操作的记录
/// </summary>
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
/// 游戏录像
/// </summary>
public static class BattleRecords {
    public static List<int> minosOrder;
    public static List<int> reviewMinosOrder;
    public static List<Operations> operatesOrder;
    public static List<Operations> reviewOperatesOrder;
    /// <summary>
    /// 伤害记录表
    /// 第一维 i 表示伤害在第 i 次操作后被累计
    /// 第二维 i 表示垃圾行在第 i 列空缺
    /// 第三维 i 表示垃圾行共添加 i 行
    /// </summary>
    public static List<Vector3> garbageOrder;
    public static List<Vector3> reviewGarbageOrder;
    public static int GameMode;
    public static float Gravity;
    public static float LockTime;

    public static int operatesIndex;    // 操作记录列表的当前读取索引
    public static int minosIndex;       // 块序记录列表的当前读取索引
    public static int garbageIndex;     // 伤害记录列表的当前读取索引
}


public class S_ReviewGenerateMinos : MonoBehaviour
{
    // 序列首位向后移动一位
    public void NextOrder()
    {
        BattleRecords.minosIndex++;
        return;
    }

    // 返回随机序列
    public int[] GetOrder(int len, ref List<int> minosOrder)
    {
        // 利用环形队列提取随机序列
        int[] rst = new int[len];
        int j = BattleRecords.minosIndex;
        for (int i = 0; i < len; i++)
        {
            rst[i] = minosOrder[j];
            j++;
            if (j >= 7) j = 0;
        }
        return rst;
    }
}
