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
    private int targetIndex = 0;                // 攻击的目标（0表示自己，剩余数字为敌人序号）
    private int foe = 0;                        // 仇人，即最近一次攻击自己的敌人的编号

    // Start is called before the first frame update
    void Start()
    {
        scorePanel = GetComponent<S_Score>().scorePanel;
    }

    private void ChangeTarget()
    {
        int gameMode = GetComponent<GameProcess>().GameMode;
        int attackMode = GetComponent<GameProcess>().AttackMode;
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
                default: break;
            }
        }
        else targetIndex = 0;
        return;
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

    private Vector3 DamageGenerate(int dmg)
    {
        Vector3 barPos = scorePanel.GetComponent<S_UIScore>().DamageBar(dmg);
        return barPos;
    }

    // 发起攻击，传入攻击力和消行所用方块（用于控制效果）
    // TODO：加入记录和读取功能
    public void Attack(int atk, GameObject mino)
    {
        // 按设定的概率发起攻击
        float prob = Random.Range(0.0f, 1.0f);
        if (prob > GetComponent<GameProcess>().GarbageProb) return;
        // 按设定的比率计算伤害
        float rat = GetComponent<GameProcess>().GarbageRatio;
        int dmg = Mathf.FloorToInt(rat * atk);

        if (dmg < 1) return;

        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        ChangeTarget();

        Vector3 tgt_pos;
        if (targetIndex == 0)
        {
            tgt_pos = DamageGenerate(dmg);
        } 
        else
        {
            tgt_pos = enemies[targetIndex - 1].transform.position;
        }

        GameObject _star = Instantiate(AttackStar);
        _star.transform.localScale = Vector3.one;
        _star.GetComponent<S_AttackStar>().Init(mino, tgt_pos);
        return;
    }
}
