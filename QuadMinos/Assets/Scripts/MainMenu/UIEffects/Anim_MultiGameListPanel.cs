using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Anim_MultiGameListPanel : MonoBehaviour
{
    private int panelMode = 0;              // 选择面板模式，0 表示未进入选择面板、1 表示在选择列表中、2 表示在房间中
    private List<string> recordsFile;       // 回放文件列表
    private List<GameObject> recordButtons; // 已实例化的按钮列表
    public GameObject listPanel;            // 用于摆放按钮的区域
    public GameObject roomListUnitButton;   // 用于实例化的按钮单元
    public GameObject button0, button1;

    private void UpdateButtons()
    {
        switch (panelMode)
        {
            case 0:
                button0.transform.GetChild(0).GetComponent<Text>().text = "创建房间";
                button1.transform.GetChild(0).GetComponent<Text>().text = "加入房间";
                button0.GetComponent<Button>().interactable = true;
                button1.GetComponent<Button>().interactable = true;
                break;
            case 1:
                button0.transform.GetChild(0).GetComponent<Text>().text = "退出列表";
                button1.transform.GetChild(0).GetComponent<Text>().text = "-";
                button0.GetComponent<Button>().interactable = true;
                button1.GetComponent<Button>().interactable = false;
                break;
            case 2:
                button0.transform.GetChild(0).GetComponent<Text>().text = "退出房间";
                button1.transform.GetChild(0).GetComponent<Text>().text = "准备";
                button0.GetComponent<Button>().interactable = true;
                button1.GetComponent<Button>().interactable = true;
                break;
        }
        return;
    }

    public void OnClick_Button0()
    {
        switch (panelMode)
        {
            case 0:
                ChangeMode(2);
                break;
            case 1:
                ChangeMode(0);
                break;
            case 2:
                ChangeMode(0);
                break;
        }
        UpdateButtons();
        return;
    }

    public void OnClick_Button1()
    {
        switch (panelMode)
        {
            case 0:
                ChangeMode(1);
                break;
            case 2:
                // 准备游戏
                break;
        }
        UpdateButtons();
        return;
    }

    public void ChangeMode(int _changeTo)
    {
        if (_changeTo == panelMode) return;
        StartCoroutine(Anim_ChangeMode(_changeTo));
        panelMode = _changeTo;
        return;
    }

    private IEnumerator Anim_ChangeMode(int _changeTo)
    {

        float t = 0.0f;
        while (transform.localScale.x > 0)
        {
            t += Time.deltaTime;
            float scaleX = Functions.F_paraFadeout(t, 0.15f, 1, -0.05f);
            transform.localScale = new Vector3(scaleX, 1, 1);
            yield return null;
        }
        transform.localScale = new Vector3(0, 1, 1);

        // 在面板展开的同时获取记录文件并展开列表
        // 由于每次收起面板都要清空列表，因此要在 yield break 之前开启协程
        StopCoroutine(nameof(Anim_RecordsButton));
        StartCoroutine(Anim_RecordsButton(_changeTo));

        if (_changeTo == 0) yield break;

        t = 0.0f;
        while (transform.localScale.x < 1)
        {
            t += Time.deltaTime;
            float scaleX = Functions.F_paraFadeout(t, 0.15f, 0, 1.05f);
            transform.localScale = new Vector3(scaleX, 1, 1);
            yield return null;
        }
        transform.localScale = new Vector3(1, 1, 1);

        yield break;
    }

    /// <summary>
    /// 回放列表列出动画，为每个回放创建一个按钮，逐个拉下
    /// </summary>
    /// <remarks>
    /// 此协程会首先获取回放记录，然后再播放动画，因此记录较多时会进入等待
    /// </remarks>
    private IEnumerator Anim_RecordsButton(int _gameMode)
    {
        // 清空记录列表
        listPanel.GetComponent<RectTransform>().sizeDelta *= new Vector2(1, 0);

        foreach (GameObject button in recordButtons) Destroy(button);
        recordButtons.Clear();

        if (_gameMode == 0) yield break;

        // 获取对应模式下的所有记录
        List<string> recordFiles = RecordFileOperations.GetAllRecords(_gameMode);

        yield return new WaitForSecondsRealtime(0.025f);

        foreach (string record in recordFiles)
        {
            // 延长绘制区域 为每条记录分配绘制长度
            Vector2 panelSizeDelta = listPanel.GetComponent<RectTransform>().sizeDelta;
            panelSizeDelta += new Vector2(0, 144);
            listPanel.GetComponent<RectTransform>().sizeDelta = panelSizeDelta;

            // 实例化按钮，设定按钮对应的记录信息
            GameObject recordButton = Instantiate(roomListUnitButton);
            recordButton.transform.parent = listPanel.transform;
            recordButton.transform.GetChild(0).GetComponent<Text>().text = record;
            recordButton.GetComponent<S_RecordButtons>().recordGameMode = _gameMode;
            recordButton.GetComponent<S_RecordButtons>().recordName = record;
            recordButton.GetComponent<S_RecordButtons>().panelSOLO = transform.parent.gameObject;
            recordButton.transform.localScale = Vector3.one;

            // 设定按钮位置
            int posY = recordButtons.Count * 144 + 000;
            RectTransform rectTransform = recordButton.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0, 1);
            rectTransform.anchorMax = new Vector2(1, 1);
            rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, posY, 128);
            rectTransform.offsetMin = new Vector2(+8, rectTransform.offsetMin.y);
            rectTransform.offsetMax = new Vector2(-8, rectTransform.offsetMax.y);

            recordButtons.Add(recordButton);

            yield return new WaitForSecondsRealtime(0.025f);
        }

        yield break;
    }

    // Start is called before the first frame update
    void Start()
    {
        transform.localScale = new Vector3(0, 1, 1);
        recordButtons = new();
        UpdateButtons();
    }
}
