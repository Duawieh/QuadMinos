using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 自定坐标系类 - 战场坐标
/// (标识战场上每一格的坐标，左下角为 (1,1)，右下角为 (10,1))
/// </summary>
public class PosIndex
{
    public int x_id;
    public int y_id;

    public PosIndex(int x_id, int y_id)
    {
        this.x_id = x_id;
        this.y_id = y_id;
    }

    public static PosIndex GetIndex(float _x, float _y)
    {
        int rst_x, rst_y;
        _x += 1.76005f;         // 此处多加 0.00005，防止浮点精度损失（下同）
        _y += 3.36005f;
        rst_x = (int)(_x / 0.32f);
        rst_y = (int)(_y / 0.32f);
        return new PosIndex(rst_x, rst_y);
    }

    public static PosIndex GetIndex(Vector2 p)
    {
        int rst_x, rst_y;
        p.x += 1.76005f;
        p.y += 3.36005f;
        rst_x = (int)(p.x / 0.32f);
        rst_y = (int)(p.y / 0.32f);
        return new PosIndex(rst_x, rst_y);
    }

    public static Vector2 GetPosition(int x_id, int y_id)
    {
        float rst_x, rst_y;
        rst_x = x_id * 0.32f - 1.76f;
        rst_y = y_id * 0.32f - 3.36f;
        return new Vector2(rst_x, rst_y);
    }

    public Vector2 GetPosition()
    {
        float rst_x, rst_y;
        rst_x = x_id * 0.32f - 1.76f;
        rst_y = y_id * 0.32f - 3.36f;
        return new Vector2(rst_x, rst_y);
    }

    public static PosIndex operator +(PosIndex a, PosIndex b)
    {
        return new PosIndex(a.x_id + b.x_id, a.y_id + b.y_id);
    }

    public static PosIndex operator -(PosIndex a, PosIndex b)
    {
        return new PosIndex(a.x_id - b.x_id, a.y_id - b.y_id);
    }
}

public class PlayfieldState : MonoBehaviour
{
    public int holdMino = -1;                               // 当前 HOLD 的 mino 的类型（-1 为空）
    public GameObject[][] takenBy = new GameObject[12][];   // 当前位置上的 Mino (仅锁定的 mino)

    private GameObject emptyWall;

    // 初始化战场状态，填入边界砖块
    private void FieldInit()
    {
        emptyWall = new GameObject();
        emptyWall.name = "EmptyWall";
        for (int i = 0; i < 12; i++)
        {
            takenBy[i] = new GameObject[51];
        }
        for (int i = 1; i <= 10; i++)
        {
            for (int j = 1; j <= 50; j++)
            {
                takenBy[i][j] = null;
            }
        }
        for (int i = 0; i < 12; i++)
        {
            takenBy[i][0] = emptyWall;
        }
        for (int i = 0; i <= 50; i++)
        {
            takenBy[0][i] = emptyWall;
            takenBy[11][i] = emptyWall;
        }
        return;
    }

    private void swap(ref int a, ref int b) { (a, b) = (b, a); }

    // Start is called before the first frame update
    void Start()
    {
        FieldInit();
    }

    public void OperateHold(int curMinoType)
    {
        if (holdMino < 0)
        {
            holdMino = curMinoType;
            GetComponent<GameProcess>().NEXT_MINO(false);
        }
        else
        {
            swap(ref curMinoType, ref holdMino);
            GetComponent<DrawNewMinos>().DrawMino(curMinoType, false);
        }
        GetComponent<DrawNewMinos>().DrawHoldMino(holdMino);
    }

    // 递归检查并 Clear 被填满的行，同时下落保留的行，返回消行数
    public int Clear(int l, int drp)
    {
        // 达到行数上限，返回（正常情况下此语句不应被执行）
        if (l > 50) return 0;

        // 记录当前堆叠的最大高度
        GetComponent<GameProcess>().blockHeight = l - drp - 1;

        int blocks = 0;
        int clear_lines = 0;
        bool cleared = true;
        for (int i = 1; i <= 10; i++)
        {
            // 检测这一行是否有空格，如果有，不消除
            if (takenBy[i][l] == null)
            {
                cleared = false;
                if (i == 1)
                {
                    // 如果这一行的第一个位置就是空格，检测这一行是否全为空
                    for (int j = 2; j <= 10; j++)
                    {
                        if (takenBy[j][l] != null)
                        {
                            blocks++;
                            break;
                        }
                    }
                }
                break;
            } else blocks++;
        }
        // 如果该行为空行，其上不可能有砖块，回溯
        if (blocks == 0) return 0;

        if (cleared)
        {
            // 如果造成了消除，消除该行
            drp++;
            clear_lines++;
            for (int i = 1; i <= 10; i++)
            {
                takenBy[i][l].GetComponent<Mino_Locked>().Clear();
                takenBy[i][l] = null;
            }
        } 
        else if (drp != 0) { 
            // 如果未造成消除，且其下有消除过的行，下移此行
            for (int i = 1; i <= 10; i++)
            {
                if (takenBy[i][l] == null)
                {
                    takenBy[i][l - drp] = null;
                } 
                else
                {
                    takenBy[i][l].transform.localPosition += new Vector3(0, -0.32f, 0) * drp;
                    takenBy[i][l - drp] = takenBy[i][l];
                    takenBy[i][l] = null;
                }
            }
        }

        // 递归返回消行数
        return clear_lines + Clear(l + 1, drp);
    }

    // 递归将堆叠上移，为下方插入垃圾行腾出空间
    public void Add(int l, int drp)
    {
        if (l > 50) return;
        // 检验是否到达堆叠最上方，如果是，返回并从最上方行开始上移
        bool _empty = true;
        for (int i = 1; i <= 10; i++)
            if (takenBy[i][l] != null)
            {
                _empty = false;
                break;
            }
        if (_empty) return;

        Add(l + 1, drp);

        for (int i = 1; i <= 10; i++)
        {
            if (takenBy[i][l] == null) continue;
            takenBy[i][l].GetComponent<Mino_Locked>().Lock(new PosIndex(i, l + drp), 7);
            takenBy[i][l] = null;
        }

        return;
    }

    // 检查全清，如果消除造成全清，得分至少为 10 分
    public float AllClear()
    {
        for (int i = 1; i <= 10; i++)
        {
            if (takenBy[i][1] != null) return 0.0f;
        }
        // 判断完成，为完美消除，执行对应效果，返回相应得分
        GetComponent<S_VisualEffect>().Anim_AllClear(true);
        return 10.0f;
    }
}
