using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Operations
{
    public int opt;        // ����������
    public float _t;       // ������ʱ��

    public Operations(int opt, float _t)
    {
        this.opt = opt;
        this._t = _t;
    }
}

/// <summary>
/// �����˵�ѡ����Ϸģʽʱ������Ϣ�����غ�ս���������ȡ��Ϣ
/// </summary>
public class BattleInfo
{
    /// <summary>
    /// ��Ϸģʽ��1-��ģʽ 2-40�� 3-����ս 4-������ 5-��Ҷ�ս��
    /// </summary>
    public static int GameMode = 1;
    public static float Gravity = 0.0156f;
    public static float LockTime = 1.0f;
    public static float GarbageProb = 0.0f;
    public static float GarbageRatio = 0.0f;
}

public class GameProcess : MonoBehaviour
{
    public int AttackMode;          // ����ģʽ
    public int GameMode;            // ��Ϸģʽ
    public int LockReset;           // �����ӳ����ô�������
    public float Gravity;           // ��������(G)
    public float LockTime;          // �����ӳ�(��)
    public float GarbageRatio;      // ����������Ȩ�أ���ֵӦ���� [0, 1]
    public float GarbageProb;       // ���������Ӹ��ʣ���ֵӦ���� [0, 1]

    public GameObject UI_Background;        // UI - ���������Ʊ���ͼ��ͱ������֣�
    public GameObject UI_StartDownTimer;    // UI - ���ֵ���ʱ
    public GameObject UI_PlayerButtons;     // UI - ��Ҳ�����
    private GameObject UI_prepareText;      // UI - ����ǰ��Ļ

    public List<Operations> operations = new List<Operations>();    // ��ҵĲ�������
    public int blockHeight = 0;                                     // ��Ҷѵ�����ĸ߶�
    public bool finished = false;                                   // ����Ƿ���Ŀ��

    // ��Ϸ��ʼǰ��ȡս����Ϣ
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
        StartCoroutine(UI_Background.GetComponent<S_PlayfieldBackgroundMusic>().AudioPlay());
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
            // ��ʾ NEXT �б��� mino
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

    // ��֤֡�ʴﵽ 60fps �����ٿ�ʼ��Ϸ
    private IEnumerator WaitFor_GAME_PREPARE()
    {
        Application.targetFrameRate = 60;   // ����֡��Ϊ 60
        yield return null;

        while (Time.deltaTime * 50 >= 1.0f) yield return null;

        GAME_PREPARE();
        yield break;
    }

    // ����Ϸ����ʱ���ã��������δ������ mino ����ֹ�����µ� mino��ͬʱֹͣ�Ʒ�
    private void StopDrawMinos()
    {
        GetComponent<DrawNewMinos>().enabled = false;
        GetComponent<S_Score>().enabled = false;

        GameObject _mino = GameObject.FindGameObjectWithTag("ActMino");
        if (_mino != null ) Destroy(_mino);

        return;
    }
}
