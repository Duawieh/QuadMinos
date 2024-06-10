using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_ReviewRecordOperations : MonoBehaviour
{
    private float T;        // 自 GAME_START 至今的时间
    private int optInd = 0; // 当前操作序号

    private void Update() {
        if (GetComponent<S_Score>().BEGIN_TIME == -1) return;
        T = Time.time - GetComponent<S_Score>().BEGIN_TIME;

        // 将此时间前所有为执行的操作按表内顺序执行（可能有多个同时进行的操作，以表内顺序为准）
        while (T >= BattleRecords.reviewOperatesOrder[optInd]._t) {
            GameObject mino = GameObject.FindGameObjectWithTag("ActMino");
            if (mino != null) {
                switch (BattleRecords.reviewOperatesOrder[optInd].opt) {
                    case 0:
                        mino.GetComponent<Mino_Active>().Operate_L();
                        break;
                    case 1:
                        mino.GetComponent<Mino_Active>().Operate_R();
                        break;
                    case 2:
                        mino.GetComponent<Mino_Active>().Operate_D();
                        break;
                    case 3:
                        mino.GetComponent<Mino_Active>().Operate_H();
                        break;
                    case 4:
                        mino.GetComponent<Mino_Active>().Operate_A();
                        break;
                    case 5:
                        mino.GetComponent<Mino_Active>().Operate_C();
                        break;
                    case 6:
                        mino.GetComponent<Mino_Active>().Operate_HD();
                        break;
                    default :
                        break;
                }
                optInd++;
            }
            else break;
        }

        return;
    }
}
