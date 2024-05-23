using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Operations
{
    public int opt;        // 操作的种类
    public float _t;       // 操作的时间

    public Operations(int opt, float _t)
    {
        this.opt = opt;
        this._t = _t;
    }
}

/// <summary>
/// 在主菜单选择游戏模式时存入信息，加载好战斗场景后读取信息
/// </summary>
public class BattleInfo
{
    /// <summary>
    /// 游戏模式（1-禅模式 2-40行 3-闪电战 4-马拉松 5-玩家对战）
    /// </summary>
    public static int GameMode = 1;
    public static float Gravity = 0.0156f;
    public static float LockTime = 1.0f;
    public static float GarbageProb = 0.0f;
    public static float GarbageRatio = 0.0f;
}

public class GameProcess : MonoBehaviour
{
    public int AttackMode;          // 攻击模式
    public int GameMode;            // 游戏模式
    public int LockReset;           // 锁定延迟重置次数上限
    public float Gravity;           // 场地重力(G)
    public float LockTime;          // 锁定延迟(秒)
    public float GarbageRatio;      // 垃圾行添加权重，此值应属于 [0, 1]
    public float GarbageProb;       // 垃圾行添加概率，此值应属于 [0, 1]

    public GameObject UI_Background;        // UI - 背景（控制背景图像和背景音乐）
    public GameObject UI_StartDownTimer;    // UI - 开局倒计时
    public GameObject UI_PlayerButtons;     // UI - 玩家操作块
    private GameObject UI_prepareText;      // UI - 开局前字幕

    public List<Operations> operations = new List<Operations>();    // 玩家的操作序列
    public int blockHeight = 0;                                     // 玩家堆叠方块的高度
    public bool finished = false;                                   // 玩家是否达成目标

    // 游戏开始前获取战斗信息
    private void GetBattleInfo()
    {
        AttackMode = GameSettings.AttackMode;
        GameMode = BattleInfo.GameMode;
        Gravity = BattleInfo.Gravity;
        LockTime = BattleInfo.LockTime;
        GarbageProb = BattleInfo.GarbageProb;
        GarbageRatio = BattleInfo.GarbageRatio;
        return;
    }

    public void GAME_START() {
        BattleScore.Init(GameMode);
        GetComponent<S_Score>().BEGIN_TIME = Time.time;
        UI_Background.GetComponent<S_PlayfieldBackgroundMusic>().AudioPlay();
        NEXT_MINO(true);
    }

    public void NEXT_MINO(bool holdable)
    {
        int[] que = GetComponent<GenerateMinos>().GetOrder(5);
        GetComponent<DrawNewMinos>().DrawMino(que[0], holdable);
        GetComponent<DrawNewMinos>().DrawForbiddenCross(que[1], GetComponent<S_VisualEffect>().dangerState);
        GetComponent<GenerateMinos>().NextOrder();
        for (int i = 1; i < que.Length; i++)
        {
            // 显示 NEXT 列表的 mino
            GetComponent<DrawNewMinos>().DrawNextMinos(que[i], i - 1);
        }
        return;
    }

    public void GAME_OVER() {
        StopDrawMinos();
        if (finished) GetComponent<S_VisualEffect>().Anim_Finish(true);
        else GetComponent<S_VisualEffect>().Anim_Failed(true);
        StartCoroutine(UI_Background.GetComponent<S_PlayfieldBackgroundMusic>().AudioStop());
        return;
    }

    private void GAME_PREPARE()
    {
        GetBattleInfo();
        if (GameMode == 2) StartCoroutine(GetComponent<S_ProcessAlert>().Process_40Line());
        if (GameMode == 3) StartCoroutine(GetComponent<S_ProcessAlert>().Process_Blitz());
        if (GameMode == 4) StartCoroutine(GetComponent<S_ProcessAlert>().Process_Marathon());

        GameObject downTimer = Instantiate(UI_StartDownTimer);
        downTimer.transform.parent = transform.parent;

        GameObject buttons = Instantiate(UI_PlayerButtons);
        buttons.transform.parent = transform.parent;

        int[] que = GetComponent<GenerateMinos>().GetOrder(4);
        for (int i = 0; i < que.Length; i++)
            GetComponent<DrawNewMinos>().DrawNextMinos(que[i], i);

        Destroy(UI_prepareText);
        return;
    }

    // Start is called before the first frame update
    void Start()
    {
        transform.localScale = new Vector3(0.0001f, 0.0001f, 0.0001f);
        UI_prepareText = GameObject.Find("Text_Prepare");

        StartCoroutine(WaitFor_GAME_PREPARE());
    }

    // 保证帧率达到 60fps 左右再开始游戏
    private IEnumerator WaitFor_GAME_PREPARE()
    {
        Application.targetFrameRate = 60;   // 设置帧率为 60
        yield return null;

        while (Time.deltaTime * 50 >= 1.0f) yield return null;

        GAME_PREPARE();
        yield break;
    }

    // 在游戏结束时调用，清除场上未锁定的 mino 并禁止生成新的 mino，同时停止计分
    private void StopDrawMinos()
    {
        GetComponent<DrawNewMinos>().enabled = false;
        GetComponent<S_Score>().enabled = false;

        GameObject _mino = GameObject.FindGameObjectWithTag("ActMino");
        if (_mino != null ) Destroy(_mino);

        return;
    }
}
