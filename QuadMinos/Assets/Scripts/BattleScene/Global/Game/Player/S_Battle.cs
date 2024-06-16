using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Battle : MonoBehaviour
{
    public int DMG_Height;                      // 受到的伤害会产生的垃圾行高度，即伤害条总长度
    public List<GameObject> DMG;                // 生成的伤害条序列，也包含相关的伤害信息
    public GameObject AttackStar;               // 攻击粒子效果

    private GameObject scorePanel;
    private GameObject[] enemies;               // 敌人列表（列表中不包含自身，若发现 targetIndex != 0，需要特判读取）
    private int foe = 0;                        // 仇人，即最近一次攻击自己的敌人的编号

    // Start is called before the first frame update
    void Start()
    {
        scorePanel = GetComponent<S_Score>().scorePanel;
    }

    private int ChangeTarget()
    {
        int gameMode = GetComponent<GameProcess>().GameMode;
        int attackMode = GetComponent<GameProcess>().AttackMode;

        int targetIndex;
        if (gameMode == 5)
        {
            switch (attackMode)
            {
                case 1:
                    targetIndex = Random.Range(0, enemies.Length) + 1;  // 随机切换攻击目标
                    break;
                case 2:
                    targetIndex = 1;
                    break;
                case 3:
                    targetIndex = foe + 1;
                    break;
                default:
                    targetIndex = 0;
                    break;
            }
        }
        // 非多人模式时，伤害目标为玩家自身
        else targetIndex = 0;
        return targetIndex;
    }

    // 根据得分扣除受到攻击产生的垃圾行序列
    public void DamageDefense(int scr)
    {
        scr /= 100;
        while (scr >= 1)
        {
            if (DMG.Count == 0) break;
            GameObject bar = DMG[DMG.Count - 1];
            if (bar.GetComponent<S_UIDamage>().DMG <= scr)
            {
                // 将该伤害条清空
                DMG_Height -= bar.GetComponent<S_UIDamage>().DMG;
                scr -= bar.GetComponent<S_UIDamage>().DMG;
                DMG.Remove(bar);
                bar.GetComponent<S_UIDamage>().Disappear();
            }
            else
            {
                // 扣除伤害条记录的部分伤害
                bar.GetComponent<S_UIDamage>().ChangeLength(bar.GetComponent<S_UIDamage>().DMG - scr);
                DMG_Height -= scr;
                scr = 0;
            }
        }
        return;
    }

    private Vector3 DamageGenerate(int _dmg, int _ept)
    {
        Vector3 barPos = scorePanel.GetComponent<S_UIScore>().DamageBar(_dmg, _ept);
        return barPos;
    }

    /// <summary>
    /// 录制伤害记录
    /// </summary>
    /// <param name="_dmg">伤害量</param>
    /// <param name="_ept">空列横坐标</param>
    /// <param name="_tgt">伤害目标</param>
    private void RecordAttackInfo(int _dmg, int _ept, int _tgt) {
        if (!GetComponent<GameProcess>().RecordMode) return;
        BattleRecords.attackOrder.Add(new AttackRecord(_dmg, _ept, _tgt));
        return;
    }

    /// <summary>
    /// 录制伤害记录
    /// </summary>
    /// <param name="_dmg">伤害量</param>
    /// <param name="_ept">空列横坐标</param>
    /// <param name="_tgt">伤害目标</param>
    /// <param name="_frm">伤害来源的敌人索引</param>
    private void RecordAttackInfo(int _dmg, int _ept, int _tgt, int _frm) {
        if (!GetComponent<GameProcess>().RecordMode) return;
        BattleRecords.attackOrder.Add(new AttackRecord(_dmg, _ept, _tgt, _frm));
        return;
    }

    /// <summary>
    /// 【玩家产生的攻击】 
    /// 由传入的攻击力和攻击方块调用对应的攻击效果，并完成攻击操作的读取和记录
    /// </summary>
    /// <remarks>
    /// 若为观看记录模式，会忽略传入的 atk 值，在本方法内完成记录的读取。
    /// 若为录制记录模式，会在本方法内完成记录的录制。
    /// </remarks>
    /// <param name="atk">产生的伤害量</param>
    /// <param name="mino">产生伤害所使用的 mino</param>
    public void Attack(int atk, GameObject mino)
    {
        // 根据记录 或 根据设定的伤害概率与倍率 计算所得伤害
        int _dmg;
        // 根据记录 或 随机生成的 垃圾行空列
        int _ept;
        // 根据记录 或 根据设定的攻击模式 计算所得攻击目标
        int _tgt;

        // 读取记录
        if (GetComponent<GameProcess>().ReviewMode) {
            if (BattleRecords.reviewAttackOrder.Count <= BattleRecords.attackIndex) return;
            _dmg = BattleRecords.reviewAttackOrder[BattleRecords.attackIndex].dmg;
            _ept = BattleRecords.reviewAttackOrder[BattleRecords.attackIndex].ept;
            _tgt = BattleRecords.reviewAttackOrder[BattleRecords.attackIndex].tgt;
            BattleRecords.attackIndex++;
        }
        else {
            // 按设定的比率计算伤害
            float rat = GetComponent<GameProcess>().GarbageRatio;
            // 按玩家设定的攻击模式获取攻击目标
            enemies = GameObject.FindGameObjectsWithTag("Enemy");
            int targetIndex = ChangeTarget();

            _dmg = Mathf.FloorToInt(rat * atk);
            _ept = Random.Range(1, 11);
            _tgt = targetIndex;
        }

        // 仅在非回放模式下需要进行随机攻击，回放模式会按记录决定是否攻击
        if (!GetComponent<GameProcess>().ReviewMode) {
            // 按设定的概率发起攻击
            float prob = Random.Range(0.0f, 1.0f);
            if (prob > GetComponent<GameProcess>().GarbageProb) {
                RecordAttackInfo(0, _ept, _tgt);
                return;
            }
        }
        // 若发起攻击，正常录制伤害记录
        RecordAttackInfo(_dmg, _ept, _tgt);

        if (_dmg < 1) return;

        // 实施攻击
        Vector3 tgt_pos;
        if (_tgt == 0) {
            tgt_pos = DamageGenerate(_dmg, _ept);
        } 
        else {
            tgt_pos = enemies[_tgt - 1].transform.position;
            // TODO：仅用于多人模式，待编写
        }

        GameObject _star = Instantiate(AttackStar);
        _star.transform.localScale = Vector3.one;
        _star.GetComponent<S_AttackStar>().Init(ref mino, tgt_pos);
        return;
    }

    /// <summary>
    /// 【敌人产生的攻击】 
    /// 由传入的攻击力和攻击来源调用对应的攻击效果，并完成攻击操作的读取和记录
    /// </summary>
    /// <remarks>
    /// 若为观看记录模式，会忽略传入的 atk 值，在本方法内完成记录的读取。
    /// 若为录制记录模式，会在本方法内完成记录的录制。
    /// </remarks>
    /// <param name="atk">产生的伤害量</param>
    /// <param name="enemy">产生伤害的敌人索引（从 1 开始）</param>
    public void Attack(int atk, int enemy)
    {
        // 根据记录 或 根据服务器传入的值 获取所得伤害
        int _dmg;
        // 根据记录 或 随机生成的 垃圾行空列
        int _ept;
        // 根据记录 或 根据服务器传入的值 获取所得攻击目标
        int _tgt;
        // 根据记录 或 根据服务器传入的值 获取伤害来源
        int _frm;

        // 读取记录
        if (GetComponent<GameProcess>().ReviewMode) {
            _dmg = BattleRecords.reviewAttackOrder[BattleRecords.attackIndex].dmg;
            _ept = BattleRecords.reviewAttackOrder[BattleRecords.attackIndex].ept;
            _tgt = BattleRecords.reviewAttackOrder[BattleRecords.attackIndex].tgt;
            _frm = BattleRecords.reviewAttackOrder[BattleRecords.attackIndex].frm;
            BattleRecords.attackIndex++;
        }
        else {
            // 按设定的比率计算伤害
            float rat = GetComponent<GameProcess>().GarbageRatio;
            // 按玩家设定的攻击模式获取攻击目标
            enemies = GameObject.FindGameObjectsWithTag("Enemy");

            _dmg = Mathf.FloorToInt(rat * atk);
            _ept = Random.Range(1, 11);
            _tgt = 0;
            _frm = enemy - 1;
        }

        // 仅在非回放模式下需要进行随机攻击，回放模式会按记录决定是否攻击
        if (!GetComponent<GameProcess>().ReviewMode) {
            // 按设定的概率发起攻击
            float prob = Random.Range(0.0f, 1.0f);
            if (prob > GetComponent<GameProcess>().GarbageProb) {
                RecordAttackInfo(0, _ept, _tgt, _frm);
                return;
            }
        }
        // 录制伤害记录
        RecordAttackInfo(_dmg, _ept, _tgt, _frm);

        if (_dmg < 1) return;

        // 实施攻击
        Vector3 tgt_pos = DamageGenerate(_dmg, _ept);
        Vector3 frm_pos = enemies[_frm].transform.position;
        GameObject _star = Instantiate(AttackStar);
        _star.transform.localScale = Vector3.one;
        _star.GetComponent<S_AttackStar>().Init(frm_pos, tgt_pos);
        return;
    }
}
