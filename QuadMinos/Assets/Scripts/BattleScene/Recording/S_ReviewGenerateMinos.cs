using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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
