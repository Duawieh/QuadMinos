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
/// 玩家产生的伤害的记录
/// </summary>
public class AttackRecord {
    public int dmg;         // 产生的伤害量
    public int ept;         // 垃圾行空列横坐标
    public int tgt;         // 攻击目标索引
    public int frm;         // 攻击来源索引

    public AttackRecord(int dmg, int ept, int tgt) {
        this.dmg = dmg;
        this.ept = ept;
        this.tgt = tgt;
        this.frm = 0;
    }

    public AttackRecord(int dmg, int ept, int tgt, int frm) {
        this.dmg = dmg;
        this.ept = ept;
        this.tgt = tgt;
        this.frm = frm;
    }
}


/// <summary>
/// 游戏录像
/// </summary>
public static class BattleRecords {
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
