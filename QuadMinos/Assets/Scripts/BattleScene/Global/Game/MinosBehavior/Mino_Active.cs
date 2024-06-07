using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Mino_Active : MonoBehaviour
{
    public GameObject shadowMino;
    public GameObject UI_Note;
    public GameObject UI_SpinStar;
    public GameObject UI_XSpinStar;
    public GameObject UI_ThunderRing;
    public AudioClip Clip_Hold;
    public AudioClip Clip_Thunder;

    private GameObject field;
    private GameObject[] units;
    private GameObject[] shadows;
    private PosIndex[] shadowsP;
    private SRS srs;            // SRS 旋转系统旋转、踢墙表
    private float G;            // 从 GameProcess 中读取的重力大小
    private float L;            // 从 GameProcess 中读取的锁定延迟
    private float T;            // 上一次下坠至今的时间
    private float C;            // 锁定计时器
    private int R;              // 锁定重置上限
    private int cnt;            // 锁定重置次数

    public int MinoType;        // mino 的形状
    public int RotFlag = 0;     // mino 的旋转状态（按照 SRS 旋转系统规定的方向）
    public bool Holdable;       // 可进行 HOLD 操作
    public PosIndex P;          // mino 的旋转中心的战场位置（所有位移操作用 PosIndex 类完成，减少浮点精度问题）

    private void RefreshPosition()
    {
        transform.localPosition = P.GetPosition();          // 更新旋转中心相对位置
        PosIndex[] localP = srs.GetPos(MinoType, RotFlag);  // 获取当前旋转状态下的子块相对战场坐标
        transform.localRotation= Quaternion.identity;
        for (int i = 0; i < units.Length; i++)
        {
            units[i].GetComponent<Mino_Locked>().RefreshPosition(localP[i]);
            units[i].transform.localRotation = Quaternion.identity;
        }
        return;
    }

    private void AutoDrop()
    {
        T += G;
        bool droped = false;
        while (T >= 1.0f)
        {
            if (!GetComponent<Mino_Kick>().Kick_Ground(MinoType, RotFlag)) {
                P = P + new PosIndex(0, -1);
                C = 0.0f;
                droped = true;
            }
            T -= 1.0f;
        }
        if (droped) field.GetComponent<GameProcess>().operations.Add(new Operations(2, Time.time));
        return;
    }

    private void RotateClockwise()
    {
        RotFlag++;
        if (RotFlag > 3) RotFlag = 0;
        return;
    }

    private void RotateAntiClockwise()
    {
        RotFlag--;
        if (RotFlag < 0) RotFlag = 3;
        return;
    }

    // 非固定时的旋转若满足 T-SPIN，产生相应效果（_dir 为方向，0为逆时针，1为顺时针）
    private void SpinEffect(bool _dir)
    {
        // 生成 T-Spin 粒子特效并调用踢墙扭转动画（此处 T-spin 与踢墙扭转共用同一动画）
        GameObject _star = Instantiate(UI_SpinStar, field.transform.parent);
        _star.transform.position = transform.position;
        // 若旋转方向为逆时针，粒子效果反向
        if (!_dir)
        {
            ParticleSystem.VelocityOverLifetimeModule VOL = _star.GetComponent<ParticleSystem>().velocityOverLifetime;
            VOL.orbitalY = -5.0f;
        }
        field.GetComponent<S_VisualEffect>().Anim_Spin(true);
        return;
    }

    private void ClearEffect(int _score)
    {
        field.GetComponent<S_VisualEffect>().Anim_Clear(true);
        GameObject scoreNote = Instantiate(UI_Note);
        scoreNote.transform.parent = field.transform.parent;
        scoreNote.transform.localScale = Vector3.one;
        scoreNote.transform.position = transform.position;
        scoreNote = scoreNote.transform.GetChild(1).gameObject;
        // 得分分层设色
        if (_score <= 300) scoreNote.GetComponent<S_UINote>().Init(_score.ToString(), new Color(1, 1, 1), 48, false);
        else if (_score <= 600) scoreNote.GetComponent<S_UINote>().Init(_score.ToString(), new Color(1, 0.8039216f, 0), 72, false);
        else if (_score <= 900) scoreNote.GetComponent<S_UINote>().Init(_score.ToString(), new Color(1, 0.6352941f, 0), 128, false);
        else
        {
            GameObject _Ring = Instantiate(UI_ThunderRing);
            _Ring.transform.position = transform.position;
            _Ring.transform.localScale = Vector3.one;
            field.GetComponent<S_AudioEffect>().PlayAudio(Clip_Thunder, 1.0f, 10.0f);   // 高伤害暴击音效
            if (_score <= 1200) scoreNote.GetComponent<S_UINote>().Init(_score.ToString(), new Color(1, 0, 0, 0.5f), 196, true);
            else scoreNote.GetComponent<S_UINote>().Init(_score.ToString(), new Color(0.8274511f, 0.2196079f, 1, 0.5f), 256, true);
        }
        return;
    }

    // 锁定并绘制下一个 mino
    private void Lock()
    {
        // 记录操作序列（操作 6-Lock）
        field.GetComponent<GameProcess>().operations.Add(new Operations(6, Time.time));
        // 获取和更新子块位置，执行子块锁定初始化函数
        PosIndex[] unitsP = srs.GetPos(MinoType, RotFlag);
        for (int i = 0; i < 4; i++)
            units[i].GetComponent<Mino_Locked>().Lock(P + unitsP[i], MinoType);
        // 检验 T-SPIN
        bool T_SPIN = GetComponent<Mino_Kick>().Spin_Check(MinoType, P);
        // 递归清除填满的行并计算消行数
        int cleared = field.GetComponent<PlayfieldState>().Clear(1, 0);
        int scr = 0;
        // 传递得分计算系统参数
        if (cleared > 0)
        {
            // 分析消除类型并计算得分，同时调用相应效果
            scr = field.GetComponent<S_Score>().Cleared(cleared, T_SPIN);
            ClearEffect(field.GetComponent<S_Score>().thunderComboScore);
            field.GetComponent<S_Battle>().Attack(scr / 100, gameObject);
        }
        else
        {
            // 如果符合 T-SPIN，调用 T-SPIN MINI 效果（不产生得分，但不会生成垃圾行，也不打断连击）
            if (T_SPIN) field.GetComponent<S_Score>().MiniSpin();
            // 未造成消除，且未进行 T-SPIN，将收到的伤害转化为垃圾行，同时打断连击
            else
            {
                field.GetComponent<DrawNewMinos>().DrawGarbageLines();
                field.GetComponent<S_Score>().thunderComboTimer = 0;
                field.GetComponent<S_Score>().clearedCombo = 0;
            }

        }
        // 记录表现情况
        field.GetComponent<S_Score>().Pieces++;
        field.GetComponent<S_Score>().Lines += cleared;
        field.GetComponent<S_Score>().Attacked += scr / 100;
        // 调用锁定效果
        field.GetComponent<S_VisualEffect>().Anim_Lock(true);
        // 删除当前 mino 并绘制下一 mino
        DestroyShadows();
        Destroy(gameObject);
        field.GetComponent<GameProcess>().NEXT_MINO(true);
        return;
    }

    // 锁定延迟计时
    private void LockTimer()
    {
        if (!GetComponent<Mino_Kick>().Kick_Ground(MinoType, RotFlag))
        {
            C = 0.0f;
            return;
        }
        C += Time.deltaTime;
        if (C > L) Lock();
        return;
    }

    // 检测阴影块位置是否与地形冲突
    private bool ShadowKick()
    {
        for (int i = 0; i < 4; i++)
        {
            PosIndex p_id = shadowsP[i] + P;
            if (field.GetComponent<PlayfieldState>().takenBy[p_id.x_id][p_id.y_id - 1] != null) return true;
        }
        return false;
    }

    private void DestroyShadows()
    {
        foreach (GameObject item in shadows)
        {
            if (item == null) continue;
            Destroy(item);
        }
        return;
    }

    private void DrawShadowMino()
    {
        if (!GameSettings.ShowShadowblock) return;
        DestroyShadows();
        for (int i = 0; i < 4; i++)
        {
            shadows[i] = Instantiate(shadowMino);
            shadows[i].transform.parent = transform;
            shadowsP[i] = units[i].GetComponent<Mino_Locked>().p_id;
        }
        while (!ShadowKick())
        {
            for (int i = 0; i < 4; i++)
            {
                shadowsP[i] = shadowsP[i] + new PosIndex(0, -1);
            }
        }
        for (int i = 0; i < 4; i++)
        {
            shadows[i].transform.localPosition = shadowsP[i].GetPosition() + new Vector2(1.76f, 3.36f);
            shadows[i].transform.localRotation = Quaternion.identity;
            shadows[i].transform.localScale = Vector3.one;
        }
        return;
    }

    // 锁定重置
    private void LockReset()
    {
        if (C <= 0.00005f) return;
        if (cnt >= R) return;
        cnt++;
        C = 0;
        return;
    }

    // 为子对象指定相应 tag
    private void TagInit()
    {
        Transform[] sons = GetComponentsInChildren<Transform>();
        if (tag == "ActMino")
        {
            foreach (Transform item in sons)
            {
                if (item == transform) continue;
                item.tag = "ActUnit";
            }
        }
        else
        {
            foreach (Transform item in sons) item.tag = tag;
        }
        return;
    }

    // Start is called before the first frame update
    void Start()
    {
        TagInit();
        field = GameObject.FindGameObjectWithTag("Field");
        units = GameObject.FindGameObjectsWithTag("ActUnit");
        srs = new SRS();
        srs.Init();
        G = field.GetComponent<GameProcess>().Gravity;
        L = field.GetComponent<GameProcess>().LockTime;
        R = field.GetComponent<GameProcess>().LockReset;
        T = 0.0f;
        C = 0.0f;
        cnt = 0;
        shadows = new GameObject[4];
        shadowsP = new PosIndex[4];
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.tag == "UIMino") return;
        if (units[0].tag == "LockUnit") return;
        AutoDrop();
        RefreshPosition();
        DrawShadowMino();
        LockTimer();
    }

    //**************************************************
    //  玩家操作动作
    //**************************************************

    // 左移
    public void Operate_L()
    {
        if (GetComponent<Mino_Kick>().Kick_Left(MinoType, RotFlag)) return;
        field.GetComponent<GameProcess>().operations.Add(new Operations(0, Time.time));
        P += new PosIndex(-1, 0);
        RefreshPosition();
        LockReset();
        return;
    }

    // 右移
    public void Operate_R()
    {
        if (GetComponent<Mino_Kick>().Kick_Right(MinoType, RotFlag)) return;
        field.GetComponent<GameProcess>().operations.Add(new Operations(1, Time.time));
        P += new PosIndex(+1, 0);
        RefreshPosition();
        LockReset();
        return;
    }

    // 软降
    public void Operate_D()
    {
        if (GetComponent<Mino_Kick>().Kick_Ground(MinoType, RotFlag)) return;
        field.GetComponent<GameProcess>().operations.Add(new Operations(2, Time.time));
        P = P + new PosIndex(0, -1);
        RefreshPosition();
        T--; if (T < 0) T = 0;
        return;
    }

    // 换块
    public void Operate_H()
    {
        if (!Holdable) return;
        field.GetComponent<GameProcess>().operations.Add(new Operations(3, Time.time));
        DestroyShadows();
        DestroyImmediate(gameObject);
        field.GetComponent<PlayfieldState>().OperateHold(MinoType);
        field.GetComponent<S_AudioEffect>().PlayAudio(Clip_Hold, 1.0f, 0.2f);
        return;
    }

    // 逆旋
    public void Operate_A()
    {
        field.GetComponent<GameProcess>().operations.Add(new Operations(4, Time.time));
        int bgn = RotFlag;
        RotateAntiClockwise();
        int fns = RotFlag;
        PosIndex p = GetComponent<Mino_Kick>().Kick_Check(MinoType, bgn, fns);
        if (p.x_id == 0)
        {
            RotateClockwise();
            return;
        }
        P = p;
        RefreshPosition();
        if (GetComponent<Mino_Kick>().Spin_Check(MinoType, p)) SpinEffect(false);
        // 检查是否满足 X-Spin，若是，展示粒子效果
        if (GetComponent<Mino_Kick>().X_Spin_Check(MinoType, RotFlag))
        {
                GameObject _star = Instantiate(UI_XSpinStar);
                _star.transform.localScale = Vector3.one;
                _star.transform.position = transform.position;
        }
        LockReset();
        return;
    }

    // 正旋
    public void Operate_C()
    {
        field.GetComponent<GameProcess>().operations.Add(new Operations(5, Time.time));
        int bgn = RotFlag;
        RotateClockwise();
        int fns = RotFlag;
        PosIndex p = GetComponent<Mino_Kick>().Kick_Check(MinoType, bgn, fns);
        if (p.x_id == 0)
        {
            RotateAntiClockwise();
            return;
        }
        P = p;
        RefreshPosition();
        if (GetComponent<Mino_Kick>().Spin_Check(MinoType, p)) SpinEffect(true);
        // 检查是否满足 X-Spin，若是，展示粒子效果
        if (GetComponent<Mino_Kick>().X_Spin_Check(MinoType, RotFlag))
        {
            GameObject _star = Instantiate(UI_XSpinStar);
            _star.transform.localScale = Vector3.one;
            _star.transform.position = transform.position;
        }
        LockReset();
        return;
    }

    // 硬降
    public void Operate_HD()
    {
        bool droped = false;
        while (!GetComponent<Mino_Kick>().Kick_Ground(MinoType, RotFlag))
        {
            P += new PosIndex(0, -1);
            droped = true;
        }
        if (droped) field.GetComponent<GameProcess>().operations.Add(new Operations(2, Time.time));
        RefreshPosition();
        Lock();
        return;
    }
}
