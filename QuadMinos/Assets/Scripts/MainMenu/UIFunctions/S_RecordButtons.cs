using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class S_RecordButtons : MonoBehaviour
{
    private GameObject buttonDelete;
    public string recordName = "";
    public bool isDeleted = false;
    public int recordGameMode = 0;
    public GameObject panelSOLO;
    public Sprite icon_Delete;
    public Sprite icon_Restore;

    public void ClickButton_RecordButton() {
        panelSOLO.GetComponent<S_PanelSOLO>().recordsPanel.GetComponent<Anim_RecordPanel>().ConfirmToDelete();
        StartCoroutine(panelSOLO.GetComponent<Anim_Panels>().StartGameAnimation());
        LoadInfo.SceneName = "BattleScene";
        BattleInfo.GameMode = recordGameMode;
        BattleInfo.Gravity = 0.0f;
        BattleInfo.LockTime = 1.0f;
        BattleInfo.GarbageProb = 0.0f;
        BattleInfo.GarbageRatio = 0.0f;
        BattleInfo.RecordMode = false;
        BattleInfo.ReviewMode = true;
        BattleInfo.RecordName = recordName;
        BattleScore.Init();
        return;
    }

    public void ClickButton_DeleteButton() {
        buttonDelete = transform.GetChild(1).gameObject;
        isDeleted = !isDeleted;
        if (isDeleted) {
            GetComponent<Button>().interactable = false;
            buttonDelete.GetComponent<Image>().sprite = icon_Restore;
            buttonDelete.GetComponent<Image>().color = Color.green;
        }
        else {
            GetComponent<Button>().interactable = true;
            buttonDelete.GetComponent<Image>().sprite = icon_Delete;
            buttonDelete.GetComponent<Image>().color = Color.white;
        }
        return;
    }
}
