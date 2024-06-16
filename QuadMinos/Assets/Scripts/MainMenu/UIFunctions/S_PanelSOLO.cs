using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class S_PanelSOLO : MonoBehaviour
{
    private bool ReviewMode = false;
    private bool RecordMode = false;
    public GameObject recordsPanel;
    public GameObject recordsButtonText;
    public GameObject recordToggle;

    public void ClickButton_ZEN()
    {
        if (ReviewMode) {
            recordsPanel.GetComponent<Anim_RecordPanel>().ChangeMode(1);
            return;
        }
        StartCoroutine(GetComponent<Anim_Panels>().StartGameAnimation());
        LoadInfo.SceneName = "BattleScene";
        BattleInfo.GameMode = 1;
        BattleInfo.Gravity = GameSettings.Gravity;
        BattleInfo.LockTime = GameSettings.LockTime;
        BattleInfo.GarbageProb = GameSettings.GarbageProb;
        BattleInfo.GarbageRatio = GameSettings.GarbageRatio;
        BattleInfo.RecordMode = RecordMode;
        BattleInfo.ReviewMode = false;
        BattleScore.Init();
    }

    public void ClickButton_40LINE()
    {
        if (ReviewMode) {
            recordsPanel.GetComponent<Anim_RecordPanel>().ChangeMode(2);
            return;
        }
        StartCoroutine(GetComponent<Anim_Panels>().StartGameAnimation());
        LoadInfo.SceneName = "BattleScene";
        BattleInfo.GameMode = 2;
        BattleInfo.Gravity = 0.0167f;
        BattleInfo.LockTime = 1.0f;
        BattleInfo.GarbageProb = 0.0f;
        BattleInfo.GarbageRatio = 0.0f;
        BattleInfo.RecordMode = RecordMode;
        BattleInfo.ReviewMode = false;
        BattleScore.Init();
    }

    public void ClickButton_BLITZ()
    {
        if (ReviewMode) {
            recordsPanel.GetComponent<Anim_RecordPanel>().ChangeMode(3);
            return;
        }
        StartCoroutine(GetComponent<Anim_Panels>().StartGameAnimation());
        LoadInfo.SceneName = "BattleScene";

        BattleInfo.GameMode = 3;
        BattleInfo.Gravity = 0.07f;
        BattleInfo.LockTime = 0.5f;
        BattleInfo.GarbageProb = 0.0f;
        BattleInfo.GarbageRatio= 0.0f;
        BattleInfo.RecordMode = RecordMode;
        BattleInfo.ReviewMode = false;
        BattleScore.Init();
    }

    public void ClickButton_MARATHON()
    {
        if (ReviewMode) {
            recordsPanel.GetComponent<Anim_RecordPanel>().ChangeMode(4);
            return;
        }
        StartCoroutine(GetComponent<Anim_Panels>().StartGameAnimation());
        LoadInfo.SceneName = "BattleScene";
        BattleInfo.GameMode = 4;
        BattleInfo.Gravity = 0.0167f;
        BattleInfo.LockTime = 1.0f;
        BattleInfo.GarbageProb = 0.0f;
        BattleInfo.GarbageRatio = 0.0f;
        BattleInfo.RecordMode = RecordMode;
        BattleInfo.ReviewMode = false;
        BattleScore.Init();
    }

    public void ClickButton_RECORDS() {
        if (ReviewMode) {
            ReviewMode = false;
            RecordMode = recordToggle.GetComponent<Toggle>().isOn;
            recordsButtonText.GetComponent<Text>().text = "回看录像";
            recordsPanel.GetComponent<Anim_RecordPanel>().ChangeMode(0);
        }
        else {
            ReviewMode = true;
            RecordMode = false;
            recordsButtonText.GetComponent<Text>().text = "返回列表";
            recordsPanel.GetComponent<Anim_RecordPanel>().ChangeMode(1);
        }
        return;
    }

    public void CheckToggle_RECORDING() {
        if (recordToggle.GetComponent<Toggle>().isOn) {
            RecordMode = true;
        }
        else {
            RecordMode = false;
        }
        return;
    }
}
