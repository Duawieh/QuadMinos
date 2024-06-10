using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 按 7-bag 规则生成新的 Minos
/// </summary>
public class GenerateMinos : MonoBehaviour
{
    private int[] order = { 0, 1, 2, 3, 4, 5, 6 };
    private int[] order_pre = { 0, 1, 2, 3, 4, 5, 6 };
    private int cur;

    private void swap(ref int _a, ref int _b) { (_b, _a) = (_a, _b); }

    private void ReOrder(ref int[] _array)
    {
        for (int i = 0; i < _array.Length; i++)
        {
            int _index = Random.Range(0, 7);
            swap(ref _array[i], ref _array[_index]);
        }
        return;
    }

    // 序列首位向后移动一位
    public void NextOrder()
    {
        // 环形更新和生成随机队列
        order[cur] = order_pre[cur];
        cur++;
        if (cur >= 7)
        {
            ReOrder(ref order_pre);
            cur = 0;
        }
        return;
    }

    // 返回随机序列
    public int[] GetOrder(int len)
    {
        // 利用环形队列提取随机序列
        int[] rst = new int[len];
        int j = cur;
        for (int i = 0; i < len; i++)
        {
            rst[i] = order[j];
            j++;
            if (j >= 7) j = 0;
        }
        return rst;
    }

    // Start is called before the first frame update
    void Start()
    {
        cur = 0;
        ReOrder(ref order);
        ReOrder(ref order_pre);
    }
}
