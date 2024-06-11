using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class S_ReviewRecordOperations : MonoBehaviour {
    private float T;        // 自 GAME_START 至今的时间

    private void Update() {
        if (GetComponent<S_Score>().BEGIN_TIME == -1) return;
        T = Time.time - GetComponent<S_Score>().BEGIN_TIME;

        // 取出当前应当执行的操作编号，若队列不足，说明操作记录已经播放完毕，游戏已经结束
        int optInd = BattleRecords.operatesIndex;
        if (BattleRecords.reviewOperatesOrder.Count <= optInd) return;

        // 将此时间前所有为执行的操作按表内顺序执行（可能有多个同时进行的操作，以表内顺序为准）
        while (T >= BattleRecords.reviewOperatesOrder[optInd]._t) {
            GameObject mino = GameObject.FindGameObjectWithTag("ActMino");
            if (mino != null) {
                switch (BattleRecords.reviewOperatesOrder[optInd].opt) {
                    // 左移操作
                    case 0:
                        mino.GetComponent<Mino_Active>().Operate_L();
                        break;
                    // 右移操作
                    case 1:
                        mino.GetComponent<Mino_Active>().Operate_R();
                        break;
                    // 软降操作
                    case 2:
                        mino.GetComponent<Mino_Active>().Operate_D();
                        break;
                    // 换块操作
                    case 3:
                        mino.GetComponent<Mino_Active>().Operate_H();
                        break;
                    // 逆旋操作
                    case 4:
                        mino.GetComponent<Mino_Active>().Operate_A();
                        break;
                    // 正旋操作
                    case 5:
                        mino.GetComponent<Mino_Active>().Operate_C();
                        break;
                    // 固定操作
                    case 6:
                        // 固定操作按硬降操作执行
                        // 硬降操作在存储时会拆分为若干软降操作和一次固定操作
                        // 但是 Lock() 是私有方法，因此这里按硬降间接调用
                        mino.GetComponent<Mino_Active>().Operate_HD();
                        break;
                    // 受到攻击
                    case 7:
                        // TODO：多人模式专用，待编写
                    default :
                        break;
                }
                optInd++;
            }
            else break;
        }

        // 同步顺序执序索引
        BattleRecords.operatesIndex = optInd;

        return;
    }
}
