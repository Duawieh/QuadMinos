using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MinosInitP
{
    public PosIndex[][] initPosIndex;
    public Vector2[] centerPivot;

    public void Init()
    {
        initPosIndex = new PosIndex[7][];
        initPosIndex[0] = new PosIndex[4];   // I Mino
        initPosIndex[0][0] = new PosIndex(4, 22);
        initPosIndex[0][1] = new PosIndex(5, 22);
        initPosIndex[0][2] = new PosIndex(6, 22);
        initPosIndex[0][3] = new PosIndex(7, 22);
        initPosIndex[1] = new PosIndex[4];   // J Mino
        initPosIndex[1][0] = new PosIndex(4, 22);
        initPosIndex[1][1] = new PosIndex(5, 22);
        initPosIndex[1][2] = new PosIndex(6, 22);
        initPosIndex[1][3] = new PosIndex(4, 23);
        initPosIndex[2] = new PosIndex[4];   // L Mino
        initPosIndex[2][0] = new PosIndex(4, 22);
        initPosIndex[2][1] = new PosIndex(5, 22);
        initPosIndex[2][2] = new PosIndex(6, 22);
        initPosIndex[2][3] = new PosIndex(6, 23);
        initPosIndex[3] = new PosIndex[4];   // O Mino
        initPosIndex[3][0] = new PosIndex(5, 23);
        initPosIndex[3][1] = new PosIndex(5, 22);
        initPosIndex[3][2] = new PosIndex(6, 22);
        initPosIndex[3][3] = new PosIndex(6, 23);
        initPosIndex[4] = new PosIndex[4];   // S Mino
        initPosIndex[4][0] = new PosIndex(5, 23);
        initPosIndex[4][1] = new PosIndex(5, 22);
        initPosIndex[4][2] = new PosIndex(4, 22);
        initPosIndex[4][3] = new PosIndex(6, 23);
        initPosIndex[5] = new PosIndex[4];   // T Mino
        initPosIndex[5][0] = new PosIndex(4, 22);
        initPosIndex[5][1] = new PosIndex(5, 22);
        initPosIndex[5][2] = new PosIndex(6, 22);
        initPosIndex[5][3] = new PosIndex(5, 23);
        initPosIndex[6] = new PosIndex[4];   // Z Mino
        initPosIndex[6][0] = new PosIndex(4, 23);
        initPosIndex[6][1] = new PosIndex(5, 22);
        initPosIndex[6][2] = new PosIndex(6, 22);
        initPosIndex[6][3] = new PosIndex(5, 23);
        // 七种 Mino 的几何中心相对旋转中心的偏移（方块边长为 0.32f）
        centerPivot = new Vector2[7];
        centerPivot[0] = new Vector2(+0.16f, +0.00f);
        centerPivot[1] = new Vector2(+0.00f, +0.16f);
        centerPivot[2] = new Vector2(+0.00f, +0.16f);
        centerPivot[3] = new Vector2(+0.16f, +0.16f);
        centerPivot[4] = new Vector2(+0.00f, +0.16f);
        centerPivot[5] = new Vector2(+0.00f, +0.16f);
        centerPivot[6] = new Vector2(+0.00f, +0.16f);
    }


}

public class DrawNewMinos : MonoBehaviour
{
    public GameObject[] actMinos = new GameObject[7];
    public GameObject[] unitMinos = new GameObject[7];
    public GameObject mino_Forbidden = null;
    public GameObject mino_Garbage = null;
    public GameObject mino_Shade = null;

    private readonly MinosInitP mip = new MinosInitP();
    private GameObject[] nextMinoCases;
    private GameObject holdMinoCase;
    private GameObject[] nextMino;
    private GameObject holdMino;
    private GameObject[] crossSymbles;

    // Start is called before the first frame update
    void Start()
    {
        holdMinoCase = GameObject.Find("HoldMinoCase");
        nextMinoCases = new GameObject[4];
        nextMinoCases[0] = GameObject.Find("NextMinoCase_0");
        nextMinoCases[1] = GameObject.Find("NextMinoCase_1");
        nextMinoCases[2] = GameObject.Find("NextMinoCase_2");
        nextMinoCases[3] = GameObject.Find("NextMinoCase_3");
        nextMino = new GameObject[4];
        crossSymbles = new GameObject[4];
        mip.Init();
    }

    private bool Check_Draw(int _id)
    {
        PosIndex[] p_id = mip.initPosIndex[_id];
        for (int i = 0; i < 4; i++)
        {
            GameObject taken = GetComponent<PlayfieldState>().takenBy[p_id[i].x_id][p_id[i].y_id];
            if (taken != null) return true;
        }
        return false;
    }

    // 检查是否有空间生成新的 Mino，如果有绘制新的 Mino，否则执行 GAME_OVER()
    // 基本的绘制，用于在第 22 行中央绘制新下落的操作块
    public void DrawMino(int _id, bool _hold)
    {
        if (Check_Draw(_id))
        {
            GetComponent<GameProcess>().GAME_OVER();
        }
        else
        {
            GameObject newMino = Instantiate(actMinos[_id]);
            newMino.transform.parent = transform;
            newMino.transform.localPosition = new Vector2(-0.16f, 3.68f);   // 初始生成位置
            newMino.transform.localScale = Vector3.one;
            newMino.GetComponent<Mino_Active>().Holdable = _hold;
            newMino.GetComponent<Mino_Active>().MinoType = _id;
            newMino.GetComponent<Mino_Active>().P = new PosIndex(5, 22);
            newMino.tag = "ActMino";
        }
    }

    // 绘制禁区的红色叉号，用于提示玩家会导致游戏失败的区域
    public void DrawForbiddenCross(int _id, bool _enabled)
    {
        for (int i = 0; i < 4; i++)
        {
            if (crossSymbles[i] != null) Destroy(crossSymbles[i]);
            if (_enabled)
            {
                crossSymbles[i] = Instantiate(mino_Forbidden, transform);
                crossSymbles[i].transform.localPosition = mip.initPosIndex[_id][i].GetPosition();
            }
        }
        return;
    }

    // 绘制 NEXT 区域的 mino
    public void DrawNextMinos(int _type, int i)
    {
        if (nextMino[i] != null) { DestroyImmediate(nextMino[i]); }  // 清除原有方块
        nextMino[i] = Instantiate(actMinos[_type]);
        nextMino[i].transform.parent = nextMinoCases[i].transform;
        nextMino[i].transform.localScale = new Vector3(0.47f, 0.47f, 0.47f);
        nextMino[i].transform.localPosition = mip.centerPivot[_type] * -0.47f;
        nextMino[i].tag = "UIMino";
        return;
    }

    // 绘制 HOLD 区域的 mino
    public void DrawHoldMino(int _type)
    {
        if (holdMino != null) { DestroyImmediate(holdMino); }       // 清除原有方块
        holdMino = Instantiate(actMinos[_type]);
        holdMino.transform.parent = holdMinoCase.transform;
        holdMino.transform.localScale = new Vector3(0.47f, 0.47f, 0.47f);
        holdMino.transform.localPosition = mip.centerPivot[_type] * -0.47f;
        holdMino.tag = "UIMino";
    }

    // 绘制垃圾行
    public void DrawGarbageLines()
    {
        ref List<GameObject> dmgUI = ref GetComponent<S_Battle>().DMG;
        if (dmgUI.Count == 0) { return; }

        List<int> emptyBlocks = new List<int>();    // 自下而上记录即将出现的垃圾行的空位战场坐标

        // 从最下方的伤害条开始，将伤害条包含的信息转化为要添加的垃圾行的空列编号
        foreach (GameObject bar in dmgUI)
        {
            int _dmg = bar.GetComponent<S_UIDamage>().DMG;
            int _ept = bar.GetComponent<S_UIDamage>().DMG;
            for (int i = 1; i <= _dmg; i++) emptyBlocks.Add(_ept);
        // 将伤害条队列清空
            Destroy(bar);
        }
        dmgUI.Clear();
        GetComponent<S_Battle>().DMG_Height = 0;

        int _lines = emptyBlocks.Count;             // 要添加的垃圾行行数
        GetComponent<S_Score>().addedLines= _lines;
        GetComponent<S_VisualEffect>().Anim_Garbage(true);

        // 将堆叠上移并在下方绘制垃圾行
        GetComponent<PlayfieldState>().Add(1, _lines);
        int j = 1;
        foreach (int _ept in emptyBlocks)
        {
            for (int i = 1; i <= 10; i++)
            {
                if (i == _ept) continue;
                GameObject garbageMino = Instantiate(mino_Garbage, transform);
                garbageMino.GetComponent<Mino_Locked>().Lock(new PosIndex(i, j), 7);
            }
            j++;
        }

        return;
    }
}
