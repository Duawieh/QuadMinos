using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class S_PanelSOLO : MonoBehaviour
{
    public void ClickButton_ZEN()
    {
        StartCoroutine(GetComponent<Anim_Panels>().StartGameAnimation());
        LoadInfo.SceneName = "BattleScene";
        BattleInfo.GameMode = 1;
        BattleInfo.Gravity = GameSettings.Gravity;
        BattleInfo.LockTime = GameSettings.LockTime;
        BattleInfo.GarbageProb = GameSettings.GarbageProb;
        BattleInfo.GarbageRatio = GameSettings.GarbageRatio;
        BattleScore.Init(1);
    }

    public void ClickButton_40LINE()
    {
        StartCoroutine(GetComponent<Anim_Panels>().StartGameAnimation());
        LoadInfo.SceneName = "BattleScene";
        BattleInfo.GameMode = 2;
        BattleInfo.Gravity = 0.0167f;
        BattleInfo.LockTime = 1.0f;
        BattleInfo.GarbageProb = 0.0f;
        BattleInfo.GarbageRatio = 0.0f;
        BattleScore.Init(2);
    }

    public void ClickButton_BLITZ()
    {
        StartCoroutine(GetComponent<Anim_Panels>().StartGameAnimation());
        LoadInfo.SceneName = "BattleScene";

        BattleInfo.GameMode = 3;
        BattleInfo.Gravity = 0.07f;
        BattleInfo.LockTime = 0.5f;
        BattleInfo.GarbageProb = 0.0f;
        BattleInfo.GarbageRatio= 0.0f;
        BattleScore.Init(3);
    }

    public void ClickButton_MARATHON()
    {
        StartCoroutine(GetComponent<Anim_Panels>().StartGameAnimation());
        LoadInfo.SceneName = "BattleScene";
        BattleInfo.GameMode = 4;
        BattleInfo.Gravity = 0.0167f;
        BattleInfo.LockTime = 1.0f;
        BattleInfo.GarbageProb = 0.0f;
        BattleInfo.GarbageRatio = 0.0f;
        BattleScore.Init(4);
    }

    // TODO：此代码仅用于测试使用，正式脚本不应包含 Start() 函数
    private void Start() {
        BattleRecords.operatesOrder = new();
        BattleRecords.minosOrder = new();
        BattleRecords.attackOrder = new();
        BattleRecords.reviewOperatesOrder = new();
        BattleRecords.reviewMinosOrder = new();
        BattleRecords.reviewAttackOrder = new();
        BattleRecords.GameMode = 1;
        BattleRecords.operatesIndex = 0;
        BattleRecords.minosIndex = 0;
        BattleRecords.attackIndex = 0;
        return;
    }
}
