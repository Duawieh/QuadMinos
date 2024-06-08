using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 战斗表现数据，用于结算画面展示和个人纪录储存
/// </summary>
public class BattleScore
{
    /// <summary>
    /// 游戏模式:
    ///     1 - 禅模式
    ///     2 - 40行
    ///     3 - 闪电战
    ///     4 - 马拉松
    ///     5 - 玩家对战
    /// </summary>
    public static int GameMode = 0;
    /// <summary>
    /// 最终得分
    /// </summary>
    public static int _Score = 0;
    /// <summary>
    /// 最高得分
    /// </summary>
    public static int _Score_Max = 0;
    /// <summary>
    /// 游戏时长（单位为秒）
    /// </summary>
    public static float _Time = 0.0f;
    /// <summary>
    /// 总共固定的 mino 数量
    /// </summary>
    public static int _Locked = 0;
    /// <summary>
    /// 总共造成的攻击力（单位为行）
    /// </summary>
    public static int _Attacked = 0;
    /// <summary>
    /// 总共消除的行数
    /// </summary>
    public static int _Lines = 0;

    /// <summary>
    /// 初始化所有表现分（游戏开始时调用）
    /// </summary>
    public static void Init(int _mode)
    {
        GameMode = _mode;
        _Score = 0;
        _Score_Max = 0;
        _Time = 0.0f;
        _Locked = 0;
        _Attacked = 0;
        _Lines = 0;
        return;
    }
}

public class S_Score : MonoBehaviour
{
    public AudioClip[] comboClips;
    public GameObject scorePanel;

    public float BEGIN_TIME = -1;   // GAME_START 时的时间
    public int SCORE = 0;           // 总得分
    public int Pieces = 0;          // 总固定数
    public int Attacked = 0;        // 总攻击量
    public int Lines = 0;           // 总消行数
    public float thunderComboTimer; // 闪电连击倒计时（连击中的相邻次数消除时间间隔在 1.0s 内视为闪电连击）
    public int thunderComboScore;   // 闪电连击得分（为满足闪电连击的连击的总得分）
    public int addedLines;          // 若添加了垃圾行，此次添加的行数
    public int clearedLines;        // 若进行了消行，此次消掉的行数
    public int clearedCombo;        // 若进行了消行，消行的连击数（单独进行一次消行时记为 1）
    public int specialCombo;        // 若进行了特殊消行，连续 B2B 次数（单独进行一次高级消行时记为 1）
    public int TspinScore;          // 此次消行是否为 T-SPIN （是为 1 ，否则为 0）

    private void Update()
    {
        if (BEGIN_TIME == -1) return;
        if (GetComponent<GameProcess>().finished) return;
        SCORE_Down();
        ThunderCombo_Down();
        DataUpdate();
        return;
    }

    // 将战斗数据同步储存（内存空间）
    private void DataUpdate()
    {
        BattleScore._Score = SCORE;
        BattleScore._Score_Max = Mathf.Max(SCORE, BattleScore._Score_Max);
        BattleScore._Time = Time.time - BEGIN_TIME;
        BattleScore._Locked = Pieces;
        BattleScore._Attacked = Attacked;
        BattleScore._Lines = Lines;
        return;
    }

    // 得分随时间减少
    private float downScoreTimer = 0.0f;
    private void SCORE_Down()
    {
        downScoreTimer += Time.deltaTime;
        if (downScoreTimer >= 0.1f)
        {
            downScoreTimer -= 0.1f;
            SCORE -= 1;
        }
        if (SCORE < 0) SCORE = 0;
        return;
    }

    // 闪电连击判定倒计时
    private void ThunderCombo_Down()
    {
        thunderComboTimer-= Time.deltaTime;
        if (thunderComboTimer <= 0)
        {
            thunderComboTimer = 0.0f;
            thunderComboScore = 0;
        }
        return;
    }

    private int GET_SCORE()
    {
        float scr = (clearedLines + Mathf.Max(1, specialCombo) - 1) * (1 + 0.25f * (clearedCombo - 1) + TspinScore);
        scr = Mathf.Max(scr, GetComponent<PlayfieldState>().AllClear());
        return (int)(100 * scr);
    }

    private void Audio_Combo()
    {
        AudioClip clp;
        if (clearedCombo <= 8) clp = comboClips[clearedCombo - 1];
        else clp = comboClips[7];
        GetComponent<S_AudioEffect>().PlayAudio(clp, 1.0f, 1.5f);
        return;
    }

    // 处理消除数据，分析消除类型并调用相应效果，计算和返回得分
    public int Cleared(int lines, bool is_spin)
    {
        // 更新连击数据并播放对应音视频效果
        if (is_spin)
        {
            scorePanel.GetComponent<S_UIScore>().TSpin();
            specialCombo++;
        }
        else if (lines == 4) specialCombo++;
        else
        {
            if (specialCombo > 1) scorePanel.GetComponent<S_UIScore>().BreakBackToBack();
            specialCombo = 0;
        }
        thunderComboTimer = 1.0f;
        clearedLines = lines;
        clearedCombo++;
        scorePanel.GetComponent<S_UIScore>().Clear(lines, clearedCombo, specialCombo);
        Audio_Combo();
        // 计算并更新得分信息
        int GOT = GET_SCORE();
        GetComponent<S_Battle>().DamageDefense(GOT);
        SCORE += GOT;
        thunderComboScore += GOT;
        return GOT;
    }

    // 显示 SPIN MINI 效果并更新得分信息（无得分，连击清空）
    public void MiniSpin()
    {
        scorePanel.GetComponent<S_UIScore>().TSpin();
        scorePanel.GetComponent<S_UIScore>().Clear(0, 0, specialCombo);

        clearedLines = 0;
        clearedCombo = 0;
        TspinScore = 0;
        return;
    }
}
