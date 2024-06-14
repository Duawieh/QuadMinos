using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

// SRS 旋转系统踢墙检测表
public class SRS
{
    private PosIndex[][][] checkTable = new PosIndex[3][][];    // 踢墙检测偏移表，分别对应JLSTZ块、I块、O块，四个方向（0,R,2,L)，五次检测
    private PosIndex[][][] rotPosTable = new PosIndex[7][][];   // 旋转位置表，分别对应七种方块，四个方向（0,R,2,L)，四个子块

    public void Init()
    {
        // 初始化旋转位置表
        {
            // I
            rotPosTable[0] = new PosIndex[4][];
            rotPosTable[0][0] = new PosIndex[] { new PosIndex(0, 0), new PosIndex(-1, 0), new PosIndex(+1, 0), new PosIndex(+2, 0) };
            rotPosTable[0][1] = new PosIndex[] { new PosIndex(0, 0), new PosIndex(0, +1), new PosIndex(0, -1), new PosIndex(0, -2) };
            rotPosTable[0][2] = new PosIndex[] { new PosIndex(0, 0), new PosIndex(-1, 0), new PosIndex(-2, 0), new PosIndex(+1, 0) };
            rotPosTable[0][3] = new PosIndex[] { new PosIndex(0, 0), new PosIndex(0, -1), new PosIndex(0, +1), new PosIndex(0, +2) };
            // J
            rotPosTable[1] = new PosIndex[4][];
            rotPosTable[1][0] = new PosIndex[] { new PosIndex(0, 0), new PosIndex(-1, 0), new PosIndex(-1, +1), new PosIndex(+1, 0) };
            rotPosTable[1][1] = new PosIndex[] { new PosIndex(0, 0), new PosIndex(0, -1), new PosIndex(0, +1), new PosIndex(+1, +1) };
            rotPosTable[1][2] = new PosIndex[] { new PosIndex(0, 0), new PosIndex(-1, 0), new PosIndex(+1, 0), new PosIndex(+1, -1) };
            rotPosTable[1][3] = new PosIndex[] { new PosIndex(0, 0), new PosIndex(0, +1), new PosIndex(0, -1), new PosIndex(-1, -1) };
            // L
            rotPosTable[2] = new PosIndex[4][];
            rotPosTable[2][0] = new PosIndex[] { new PosIndex(0, 0), new PosIndex(-1, 0), new PosIndex(+1, 0), new PosIndex(+1, +1) };
            rotPosTable[2][1] = new PosIndex[] { new PosIndex(0, 0), new PosIndex(0, +1), new PosIndex(0, -1), new PosIndex(+1, -1) };
            rotPosTable[2][2] = new PosIndex[] { new PosIndex(0, 0), new PosIndex(+1, 0), new PosIndex(-1, 0), new PosIndex(-1, -1) };
            rotPosTable[2][3] = new PosIndex[] { new PosIndex(0, 0), new PosIndex(0, -1), new PosIndex(0, +1), new PosIndex(-1, +1) };
            // O
            rotPosTable[3] = new PosIndex[4][];
            rotPosTable[3][0] = new PosIndex[] { new PosIndex(0, 0), new PosIndex(0, +1), new PosIndex(+1, +1), new PosIndex(+1, 0) };
            rotPosTable[3][1] = new PosIndex[] { new PosIndex(0, 0), new PosIndex(+1, 0), new PosIndex(+1, -1), new PosIndex(0, -1) };
            rotPosTable[3][2] = new PosIndex[] { new PosIndex(0, 0), new PosIndex(0, -1), new PosIndex(-1, -1), new PosIndex(-1, 0) };
            rotPosTable[3][3] = new PosIndex[] { new PosIndex(0, 0), new PosIndex(-1, 0), new PosIndex(-1, +1), new PosIndex(0, +1) };
            // S
            rotPosTable[4] = new PosIndex[4][];
            rotPosTable[4][0] = new PosIndex[] { new PosIndex(0, 0), new PosIndex(-1, 0), new PosIndex(0, +1), new PosIndex(+1, +1) };
            rotPosTable[4][1] = new PosIndex[] { new PosIndex(0, 0), new PosIndex(0, +1), new PosIndex(+1, 0), new PosIndex(+1, -1) };
            rotPosTable[4][2] = new PosIndex[] { new PosIndex(0, 0), new PosIndex(+1, 0), new PosIndex(0, -1), new PosIndex(-1, -1) };
            rotPosTable[4][3] = new PosIndex[] { new PosIndex(0, 0), new PosIndex(0, -1), new PosIndex(-1, 0), new PosIndex(-1, +1) };
            // T
            rotPosTable[5] = new PosIndex[4][];
            rotPosTable[5][0] = new PosIndex[] { new PosIndex(0, 0), new PosIndex(-1, 0), new PosIndex(0, +1), new PosIndex(+1, 0) };
            rotPosTable[5][1] = new PosIndex[] { new PosIndex(0, 0), new PosIndex(0, +1), new PosIndex(+1, 0), new PosIndex(0, -1) };
            rotPosTable[5][2] = new PosIndex[] { new PosIndex(0, 0), new PosIndex(+1, 0), new PosIndex(0, -1), new PosIndex(-1, 0) };
            rotPosTable[5][3] = new PosIndex[] { new PosIndex(0, 0), new PosIndex(0, -1), new PosIndex(-1, 0), new PosIndex(0, +1) };
            // Z
            rotPosTable[6] = new PosIndex[4][];
            rotPosTable[6][0] = new PosIndex[] { new PosIndex(0, 0), new PosIndex(+1, 0), new PosIndex(0, +1), new PosIndex(-1, +1) };
            rotPosTable[6][1] = new PosIndex[] { new PosIndex(0, 0), new PosIndex(0, -1), new PosIndex(+1, 0), new PosIndex(+1, +1) };
            rotPosTable[6][2] = new PosIndex[] { new PosIndex(0, 0), new PosIndex(-1, 0), new PosIndex(0, -1), new PosIndex(+1, -1) };
            rotPosTable[6][3] = new PosIndex[] { new PosIndex(0, 0), new PosIndex(0, +1), new PosIndex(-1, 0), new PosIndex(-1, -1) };
        }
        // 初始化 Kick 表
        {
            // JLSTZ 块 kick 检测偏移表
            checkTable[0] = new PosIndex[4][];
            checkTable[0][0] = new PosIndex[] { new PosIndex(+0, +0), new PosIndex(+0, +0), new PosIndex(+0, +0), new PosIndex(+0, +0), new PosIndex(+0, +0) };
            checkTable[0][1] = new PosIndex[] { new PosIndex(+0, +0), new PosIndex(+1, +0), new PosIndex(+1, -1), new PosIndex(+0, +2), new PosIndex(+1, +2) };
            checkTable[0][2] = new PosIndex[] { new PosIndex(+0, +0), new PosIndex(+0, +0), new PosIndex(+0, +0), new PosIndex(+0, +0), new PosIndex(+0, +0) };
            checkTable[0][3] = new PosIndex[] { new PosIndex(+0, +0), new PosIndex(-1, +0), new PosIndex(-1, -1), new PosIndex(+0, +2), new PosIndex(-1, +2) };
            // I 块 kick 检测偏移表
            checkTable[1] = new PosIndex[4][];
            checkTable[1][0] = new PosIndex[] { new PosIndex(+0, +0), new PosIndex(-1, +0), new PosIndex(+2, +0), new PosIndex(-1, +0), new PosIndex(+2, +0) };
            checkTable[1][1] = new PosIndex[] { new PosIndex(-1, +0), new PosIndex(+0, +0), new PosIndex(+0, +0), new PosIndex(+0, +1), new PosIndex(+0, -2) };
            checkTable[1][2] = new PosIndex[] { new PosIndex(-1, +1), new PosIndex(+1, +1), new PosIndex(-2, +1), new PosIndex(+1, +0), new PosIndex(-2, +0) };
            checkTable[1][3] = new PosIndex[] { new PosIndex(+0, +1), new PosIndex(+0, +1), new PosIndex(+0, +1), new PosIndex(+0, -1), new PosIndex(+0, +2) };
            // O 块 kick 检测偏移表（O 块不可踢墙，仅一次检测）
            checkTable[2] = new PosIndex[4][];
            checkTable[2][0] = new PosIndex[] { new PosIndex(+0, +0) };
            checkTable[2][1] = new PosIndex[] { new PosIndex(+0, -1) };
            checkTable[2][2] = new PosIndex[] { new PosIndex(-1, -1) };
            checkTable[2][3] = new PosIndex[] { new PosIndex(-1, +0) };
        }
        return;
    }

    public PosIndex[] GetTableElm(int _type, int bgn, int fns) 
    {
        if (_type == 0)
        {
            PosIndex[] rst = new PosIndex[5];
            for (int i = 0; i < 5; i++)
                rst[i] = checkTable[1][bgn][i] - checkTable[1][fns][i];
            return rst;
        }
        else if (_type == 3)
        {
            PosIndex[] rst = new PosIndex[1];
            rst[0] = checkTable[2][bgn][0] - checkTable[2][fns][0];
            return rst;
        }
        else
        {
            PosIndex[] rst = new PosIndex[5];
            for (int i = 0; i < 5; i++)
                rst[i] = checkTable[0][bgn][i] - checkTable[0][fns][i];
            return rst;
        }
    }
    
    public PosIndex[] GetPos(int _type, int _rot)
    {
        return rotPosTable[_type][_rot];
    }
}

public class Mino_Kick : MonoBehaviour
{
    public GameObject UI_Note;

    private GameObject field;
    private SRS srs;

    public bool Kick_Top(int _type, int _rot)
    {
        PosIndex P = GetComponent<Mino_Active>().P;
        PosIndex[] unitsP = srs.GetPos(_type, _rot);
        GameObject[][] taken = field.GetComponent<PlayfieldState>().takenBy;
        for (int i = 0; i < 4; i++)
        {
            PosIndex p = unitsP[i] + P;
            if (taken[p.x_id][p.y_id + 1] != null) return true;
        }
        return false;
    }

    public bool Kick_Ground(int _type, int _rot)
    {
        PosIndex P = GetComponent<Mino_Active>().P;
        PosIndex[] unitsP = srs.GetPos(_type, _rot);
        GameObject[][] taken = field.GetComponent<PlayfieldState>().takenBy;
        for (int i = 0; i < 4; i++)
        {
            PosIndex p = unitsP[i] + P;
            if (taken[p.x_id][p.y_id - 1] != null) return true;
        }
        return false;
    }

    public bool Kick_Left(int _type, int _rot)
    {
        PosIndex P = GetComponent<Mino_Active>().P;
        PosIndex[] unitsP = srs.GetPos(_type, _rot);
        GameObject[][] taken = field.GetComponent<PlayfieldState>().takenBy;
        for (int i = 0; i < 4; i++)
        {
            PosIndex p = unitsP[i] + P;
            if (taken[p.x_id - 1][p.y_id] != null) return true;
        }
        return false;
    }

    public bool Kick_Right(int _type, int _rot)
    {
        PosIndex P = GetComponent<Mino_Active>().P;
        PosIndex[] unitsP = srs.GetPos(_type, _rot);
        GameObject[][] taken = field.GetComponent<PlayfieldState>().takenBy;
        for (int i = 0; i < 4; i++)
        {
            PosIndex p = unitsP[i] + P;
            if (taken[p.x_id + 1][p.y_id] != null) return true;
        }
        return false;
    }

    // 踢墙检测
    public PosIndex Kick_Check(int _t, int bgn, int fns)
    {
        PosIndex P = GetComponent<Mino_Active>().P;
        PosIndex[] table = srs.GetTableElm(_t, bgn, fns);
        PosIndex[] unitsP = srs.GetPos(_t, fns);
        GameObject[][] taken = field.GetComponent<PlayfieldState>().takenBy;
        for (int i = 0; i < table.Length; i++)
        {
            bool kicked = false;
            for (int j = 0; j < 4; j++)
            {
                // 获取子块在踢墙后的战场坐标
                PosIndex p_id = unitsP[j] + table[i] + P;
                kicked = true;
                if (p_id.x_id < 1 || p_id.x_id > 10) break;
                if (p_id.y_id < 1 || p_id.y_id > 50) break;
                if (taken[p_id.x_id][p_id.y_id] != null) break;
                kicked = false;
            }
            if (!kicked) {
                if (i > 0)
                {
                    field.GetComponent<S_VisualEffect>().Anim_KickRotate(true);
                    GameObject kickNote = Instantiate(UI_Note);
                    kickNote.transform.parent = field.transform.parent;
                    kickNote.transform.localScale = Vector3.one;
                    kickNote.transform.position = transform.position;
                    kickNote = kickNote.transform.GetChild(1).gameObject;
                    kickNote.GetComponent<S_UINote>().Init("Kick", new Color(1, 0.8039216f, 0, 1), 32, false);
                }
                return table[i] + GetComponent<Mino_Active>().P;
            }
        }
        field.GetComponent<S_VisualEffect>().Anim_FailedToRotate(true);
        return new PosIndex(0, 0);
    }

    // T-spin 检测
    public bool Spin_Check(int _type, PosIndex p_id)
    {
        // 检测是否为 T 块
        if (_type != 5) return false;
        // 检测最后一次非 HardDrop 操作是否为旋转
        ref List<Operations> lst = ref BattleRecords.operatesOrder;
        for (int i = lst.Count - 1; i >= 0; i--)
        {
            if (lst[i].opt != 6)
            {
                if (lst[i].opt != 4 && lst[i].opt != 5) return false;
                break;
            }
        }
        // 三角检测（不含墙壁与地板）
        int cnt = 0;
        GameObject[][] taken = field.GetComponent<PlayfieldState>().takenBy;
        if (taken[p_id.x_id + 1][p_id.y_id + 1] != null && taken[p_id.x_id + 1][p_id.y_id + 1].name != "EmptyWall") cnt++;
        if (taken[p_id.x_id + 1][p_id.y_id - 1] != null && taken[p_id.x_id + 1][p_id.y_id - 1].name != "EmptyWall") cnt++;
        if (taken[p_id.x_id - 1][p_id.y_id + 1] != null && taken[p_id.x_id - 1][p_id.y_id + 1].name != "EmptyWall") cnt++;
        if (taken[p_id.x_id - 1][p_id.y_id - 1] != null && taken[p_id.x_id - 1][p_id.y_id - 1].name != "EmptyWall") cnt++;

        // 检测结束
        return cnt >= 3;
    }

    // 其他块的 Spin 检测（无得分，仅用于生成效果）
    public bool X_Spin_Check(int _type, int _rot)
    {
        if (!Kick_Top(_type, _rot)) return false;
        if (!Kick_Ground(_type, _rot)) return false;
        if (!Kick_Left(_type, _rot)) return false;
        if (!Kick_Right(_type, _rot)) return false;
        return true;
    }

    void Start()
    {
        field = GameObject.FindGameObjectWithTag("Field");
        srs = new SRS();
        srs.Init();
    }
}
