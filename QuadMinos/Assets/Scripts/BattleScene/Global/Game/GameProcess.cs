using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// 本局游戏的设置信息
/// </summary>
public class BattleInfo
{
    /// <summary>
    /// 游戏模式信息 1-禅模式 2-40行 3-闪电战 4-马拉松 5-多人模式
    /// </summary>
    public static int GameMode = 1;
    /// <summary>
    /// 是否观看录像模式
    /// </summary>
    public static bool ReviewMode = true;
    /// <summary>
    /// 是否观看录像模式
    /// </summary>
    public static bool RecordMode = false;
    public static string RecordName = "2024.6.11 19-30-58";
    public static float Gravity = 0.0156f;
    public static float LockTime = 1.0f;
    public static float GarbageProb = 0.0f;
    public static float GarbageRatio = 0.0f;
}


public class GameProcess : MonoBehaviour
{
    public int AttackMode;          // 玩家进攻策略（多人模式可用）
    public int GameMode;            // 游戏模式
    public int LockReset;           // 最大锁定重置次数
    public bool ReviewMode;         // 是否观看录像模式
    public bool RecordMode;         // 是否记录录像模式
    public float Gravity;           // 重力(G)
    public float LockTime;          // 锁定延迟(秒)
    public float GarbageRatio;      // 垃圾行攻击比率 [0, 1]（仅禅模式可用）
    public float GarbageProb;       // 垃圾行添加概率 [0, 1]（仅禅模式可用）

    public GameObject UI_Background;        // UI - 游戏背景图像
    public GameObject UI_StartDownTimer;    // UI - 游戏开始倒计时
    public GameObject UI_PlayerButtons;     // UI - 玩家操作HUD
    private GameObject UI_prepareText;      // UI - 游戏准备提示

    public int blockHeight = 0;             // 方块堆叠高度
    public bool finished = false;           // 是否游戏结束

    // 获取本局游戏设置信息
    private void GetBattleInfo()
    {
        AttackMode = GameSettings.AttackMode;
        ReviewMode = BattleInfo.ReviewMode;
        RecordMode = BattleInfo.RecordMode;
        GameMode = BattleInfo.GameMode;
        Gravity = BattleInfo.Gravity;
        LockTime = BattleInfo.LockTime;
        GarbageProb = BattleInfo.GarbageProb;
        GarbageRatio = BattleInfo.GarbageRatio;
        return;
    }

    // 获取录像游戏设置信息
    private void GetReviewInfo() {
        if (!ReviewMode) return;
        GameMode = BattleRecords.GameMode;
        GarbageProb = 0.0f;
        GarbageRatio = 0.0f;
    }

    public void GAME_START() {
        BattleScore.Init();
        if (RecordMode) BattleRecords.GameMode = GameMode;
        GetComponent<S_Score>().BEGIN_TIME = Time.time;
        StartCoroutine(UI_Background.GetComponent<S_PlayfieldBackgroundMusic>().AudioPlay());
        NEXT_MINO(true);
    }

    public void NEXT_MINO(bool holdable) {
        int[] que;

        // 观看录像模式从录像块序中取出 mino 序列
        if (ReviewMode) {
            que = GetComponent<S_ReviewGenerateMinos>().GetOrder(5, ref BattleRecords.reviewMinosOrder);
            GetComponent<S_ReviewGenerateMinos>().NextOrder();
        }
        // 非观看录像模式随机计算 mino 序列
        else {
            que = GetComponent<GenerateMinos>().GetOrder(5);
            GetComponent<GenerateMinos>().NextOrder();
        }

        // 记录录像模式记录块序
        if (RecordMode) {
            BattleRecords.minosOrder.Add(que[^1]);
        }

        GetComponent<DrawNewMinos>().DrawMino(que[0], holdable);
        GetComponent<DrawNewMinos>().DrawForbiddenCross(que[1], GetComponent<S_VisualEffect>().dangerState);
        // 在 NEXT 区域绘制 mino
        for (int i = 1; i < que.Length; i++) {
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
        if (GameMode == 2) StartCoroutine(GetComponent<S_ProcessAlert>().Process_40Line());
        if (GameMode == 3) StartCoroutine(GetComponent<S_ProcessAlert>().Process_Blitz());
        if (GameMode == 4) StartCoroutine(GetComponent<S_ProcessAlert>().Process_Marathon());

        GameObject downTimer = Instantiate(UI_StartDownTimer);
        downTimer.transform.parent = transform.parent;

        GameObject buttons = Instantiate(UI_PlayerButtons);
        buttons.transform.parent = transform.parent;

        int[] que;
        // 播放记录模式下将从记录中读取块序表，而不是随机生成
        if (ReviewMode) {
            que = GetComponent<S_ReviewGenerateMinos>().GetOrder(4, ref BattleRecords.reviewMinosOrder);
        }
        else que = GetComponent<GenerateMinos>().GetOrder(4);

        // 录制记录模式下将首先随机产生的四个 NEXT minos 存入记录表
        if (RecordMode) {
            for (int i = 0; i < que.Length; i++) {
                BattleRecords.minosOrder.Add(que[i]);
            }
        }

        for (int i = 0; i < que.Length; i++) {
            GetComponent<DrawNewMinos>().DrawNextMinos(que[i], i);
        }

        Destroy(UI_prepareText);
        return;
    }

    // Start is called before the first frame update
    void Start() {
        BattleScore.Init();
        transform.localScale = new Vector3(0.0001f, 0.0001f, 0.0001f);
        UI_prepareText = GameObject.Find("Text_Prepare");
        StartCoroutine(WaitFor_GAME_PREPARE());
    }

    // 强制设定 60fps 帧率，帧率到达设定帧率后再开始游戏
    // 若为观看录像模式，加载录像，录像加载完成后再开始播放
    private IEnumerator WaitFor_GAME_PREPARE() {
        Application.targetFrameRate = 60;   // 设定帧率为 60

        GetBattleInfo();
        GetReviewInfo();
        GetComponent<S_ReviewRecordOperations>().enabled = ReviewMode;

        yield return null;

        // 记录未加载好或帧率不超过 50 时不能开始游戏
        while (Time.deltaTime * 50 >= 1.0f) yield return null;

        GAME_PREPARE();
        yield break;
    }

    // 当游戏结束时停止绘制 mino 并且删除场上正在落下的 mino
    private void StopDrawMinos() {
        GetComponent<DrawNewMinos>().enabled = false;
        GetComponent<S_Score>().enabled = false;

        GameObject _mino = GameObject.FindGameObjectWithTag("ActMino");
        if (_mino != null ) Destroy(_mino);

        return;
    }
}
