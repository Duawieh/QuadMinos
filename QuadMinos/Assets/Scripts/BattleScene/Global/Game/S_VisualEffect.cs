using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.SceneManagement;

// 动画曲线函数
public class Functions
{
    /// <summary>
    /// 以二次函数为基础 生成逐渐减缓的变化
    /// </summary>
    public static float F_paraFadeout(float t, float T_tot, float B, float M)
    {
        float rst;
        float _a = (B - M) / (T_tot * T_tot);
        float _b = -2 * _a * T_tot;
        rst = _a * t * t + _b * t + B;
        return rst;
    }

    /// <summary>
    /// 以二次函数为基础 生成逐渐加快的变化
    /// </summary>
    public static float F_paraFadein(float t, float T_tot, float B, float M)
    {
        float rst;
        float _a = (M - B) / (T_tot * T_tot);
        rst = _a * t * t + B;
        return rst;
    }

    /// <summary>
    /// 以二次函数为基础 生成淡入淡出的变化
    /// </summary>
    public static float F_paraFadeinout(float t, float T_tot, float B, float M)
    {
        float rst;
        float midT = T_tot/ 2;
        float midY = (B + M) / 2;
        if (t < midT) rst = F_paraFadein(t, midT, B, midY);
        else rst = F_paraFadeout(t - midT, midT, midY, M);
        return rst;
    }

    /// <summary>
    /// 以二次函数为基础 生成弹力的变化
    /// </summary>
    public static float F_paraBounce(float t, float T_tot, float T_mid, float B, float M)
    {
        float rst;
        if (t < T_mid) rst = F_paraFadeout(t, T_mid, B, M);
        else rst = F_paraFadeinout(t - T_mid, T_tot - T_mid, M, B);
        return rst;
    }

    /// <summary>
    /// 以一次函数为基础 生成线性的变化
    /// </summary>
    public static float F_lineFade(float t, float T_tot, float B, float M)
    {
        return (M - B) / T_tot * t + B;
    }

    /// <summary>
    /// 以三角函数和一次函数为基础 生成弹簧简谐运动并逐渐停止的动画
    /// </summary>
    /// <param name="B">简谐平衡位置</param>
    /// <param name="M">最大振幅位置，与 B 的大小关系决定第一次振动时的方向</param>
    /// <param name="_Times">简谐振动的周期数</param>
    public static float F_triBounce(float t, float T_tot, float B, float M, float _Times)
    {
        float _omega = 2 * Mathf.PI / T_tot * _Times;
        float rst = Mathf.Sin(_omega * t);
        rst *= F_lineFade(t, T_tot, M - B, 0);
        return rst + B;
    }
}

public class S_VisualEffect : MonoBehaviour
{
    /**************************
     * 效果编号对照表
     * 
     * 0 - Lock
     * 1 - FailedToRotate
     * 2 - KickRotate
     * 3 - Garbage
     * 4 - Clear
     * 5 - AllClear
     * 6 - Warning
     * 7 - Spin
     * 8 - Failed
     * 9 - Finish
     * 
     * 
     * ************************/

    public AudioClip[] clips;               // 音效（仅用于调用 S_AudioEffect 类的成员函数，不允许在此类内播放）

    private bool[] animPlaying;             // 动画是否正在播放（由动画函数控制，首次调用时开启，动画结束后关闭）
    private float[] animBegin;              // 动画开始的时间

    public Vector3 originPosition;          // 默认 Transform.localPosition
    public Vector3 originRotation;          // 默认 Transform.localRotate
    public Vector3 originScaltion;          // 默认 Transform.localScale

    private void ResetTransform()
    {
        transform.localPosition = originPosition;
        transform.localEulerAngles = originRotation;
        transform.localScale = originScaltion;
        return;
    }

    // Start is called before the first frame update
    void Start()
    {
        animPlaying = new bool[10];
        animBegin = new float[10];
    }

    // Update is called once per frame
    void Update()
    {
        if (animPlaying[0]) Anim_Lock(false);
        if (animPlaying[1]) Anim_FailedToRotate(false);
        if (animPlaying[2]) Anim_KickRotate(false);
        if (animPlaying[3]) Anim_Garbage(false);
        if (animPlaying[4]) Anim_Clear(false);
        if (animPlaying[5]) Anim_AllClear(false);
        if (animPlaying[6]) Anim_Warning(false);
        if (animPlaying[7]) Anim_Spin(false);
        if (animPlaying[8]) Anim_Failed(false);
        if (animPlaying[9]) Anim_Finish(false);
        State_Warning();
        State_Tilt();
    }

    // Animation_0 - Lock
    public void Anim_Lock(bool AnimBegin)
    {
        float T = 0.1f;
        float T_mid = 0.05f;
        float t = Time.time;

        float B = originPosition.y;
        float M = originPosition.y - Screen.height * 0.003f;

        if (AnimBegin)
        {
            GetComponent<S_AudioEffect>().PlayAudio(clips[0], 1.0f, 0.5f);
            animBegin[0] = t - Time.deltaTime;
            animPlaying[0] = true;
        }
        
        t -= animBegin[0];
        float posY = Functions.F_paraBounce(t, T, T_mid, B, M);
        transform.localPosition = new Vector3(0, posY, 0);

        if (t > T) {
            ResetTransform();
            animPlaying[0] = false;
        }
        return;
    }

    // Animation_1 - FailedToRotate
    private int rot_dir_1;
    public void Anim_FailedToRotate(bool AnimBegin)
    {
        float T = 0.15f;
        float T_mid = 0.05f;
        float t = Time.time;

        float B = originRotation.z;
        float M = originRotation.z - 5.0f * rot_dir_1;

        if (AnimBegin)
        {
            Operations opt = BattleRecords.operatesOrder[^1];
            if (opt.opt == 4) rot_dir_1 = -1;
            if (opt.opt == 5) rot_dir_1 = +1;

            GetComponent<S_AudioEffect>().PlayAudio(clips[1], 1.0f, 0.2f);
            animBegin[1] = t - Time.deltaTime;
            animPlaying[1] = true;
        }

        t -= animBegin[1];
        float eulerZ = Functions.F_paraBounce(t, T, T_mid, B, M);
        transform.localEulerAngles = new Vector3(0, 0, eulerZ);

        if (t > T)
        {
            ResetTransform();
            animPlaying[1] = false;
        }
        return;
    }

    // Animation_2 - KickRotate
    private int rot_dir_2;
    public void Anim_KickRotate(bool AnimBegin)
    {
        float T = 0.3f;
        float T_mid = 0.05f;
        float t = Time.time;

        float B = originRotation.z;
        float M = originRotation.z - 0.5f * rot_dir_2;

        if (AnimBegin)
        {
            Operations opt = BattleRecords.operatesOrder[^1];
            if (opt.opt == 4) rot_dir_2 = -1;
            if (opt.opt == 5) rot_dir_2 = +1;

            GetComponent<S_AudioEffect>().PlayAudio(clips[2], 1.0f, 0.2f);
            animBegin[2] = t - Time.deltaTime;
            animPlaying[2] = true;
        }

        t -= animBegin[2];
        float eulerZ = Functions.F_paraBounce(t, T, T_mid, B, M);
        transform.localEulerAngles = new Vector3(0, 0, eulerZ);

        if (t > T)
        {
            ResetTransform();
            animPlaying[2] = false;
        }
        return;
    }

    // Animation_3 - Garbage
    private int addedLines;
    public void Anim_Garbage(bool AnimBegin)
    {
        float T = 0.15f;
        float T_mid = 0.067f;
        float t = Time.time;

        float B = originPosition.y;
        float M = originPosition.y + Screen.height * 0.0048f * addedLines;

        if (AnimBegin)
        {
            addedLines = GetComponent<S_Score>().addedLines;

            GetComponent<S_AudioEffect>().PlayAudio(clips[3], 1.0f, 1.0f);
            animBegin[3] = t - Time.deltaTime;
            animPlaying[3] = true;
        }

        t -= animBegin[3];
        float posY = Functions.F_paraBounce(t, T, T_mid, B, M);
        transform.localPosition = new Vector3(0, posY, 0);

        if (t > T)
        {
            ResetTransform();
            animPlaying[3] = false;
        }
        return;
    }

    // Animation_4 - Clear
    private int clearedLines;
    public void Anim_Clear(bool AnimBegin)
    {
        float T = 0.2f;
        float T_mid = 0.067f;
        float t = Time.time;

        float B = originPosition.y;
        float M = originPosition.y - Screen.height * 0.0072f * clearedLines;

        if (AnimBegin)
        {
            clearedLines = GetComponent<S_Score>().clearedLines;

            GetComponent<S_AudioEffect>().PlayAudio(clips[4], 1.0f, 1.0f);
            animBegin[4] = t - Time.deltaTime;
            animPlaying[4] = true;
        }

        t -= animBegin[4];
        float posY = Functions.F_paraBounce(t, T, T_mid, B, M);
        transform.localPosition = new Vector3(0, posY, 0);

        if (t > T)
        {
            ResetTransform();
            animPlaying[4] = false;
        }
        return;
    }

    public GameObject UI_AllClearText;
    public void Anim_AllClear(bool AnimBegin)
    {
        float T = 0.5f;
        float T_mid = 0.1f;
        float t = Time.time;

        float B = originScaltion.x;
        float M = originScaltion.x * 0.9f;

        if (AnimBegin)
        {
            GetComponent<S_AudioEffect>().PlayAudio(clips[5], 1.0f, 0.5f);
            Instantiate(UI_AllClearText, transform.parent);
            animBegin[5] = t - Time.deltaTime;
            animPlaying[5] = true;
        }

        t -= animBegin[5];
        float sclX = Functions.F_paraBounce(t, T, T_mid, B, M);
        transform.localScale = new Vector3(sclX, sclX, 1);

        if (t > T)
        {
            ResetTransform();
            animPlaying[5] = false;
        }
        return;
    }

    // Animation_6 - Warning
    private Color warnColor_fns = Color.white;
    private Color warnColor_bgn = Color.white;
    private GameObject[] ButtonsUI;
    public GameObject[] SpritesUI;
    public void Anim_Warning(bool AnimBegin)
    {
        float T = 0.2f;
        float t = Time.time;

        float B;
        float M;

        float _r, _g, _b;

        if (AnimBegin)
        {
            ButtonsUI = GameObject.FindGameObjectsWithTag("GameController");
            warnColor_bgn = SpritesUI[0].GetComponent<SpriteRenderer>().color;
            animBegin[6] = t - Time.deltaTime;
            animPlaying[6] = true;
        }

        t -= animBegin[6];

        if (warnColor_bgn.r != warnColor_fns.r)
        {
            B = warnColor_bgn.r;
            M = warnColor_fns.r;
            _r = Functions.F_paraFadeout(t, T, B, M);
        }
        else _r = warnColor_fns.r;
        if (warnColor_bgn.g != warnColor_fns.g)
        {
            B = warnColor_bgn.g;
            M = warnColor_fns.g;
            _g = Functions.F_paraFadeout(t, T, B, M);
        }
        else _g = warnColor_fns.g;
        if (warnColor_bgn.b != warnColor_fns.b)
        {
            B = warnColor_bgn.b;
            M = warnColor_fns.b;
            _b = Functions.F_paraFadeout(t, T, B, M);
        }
        else _b = warnColor_fns.b;

        // 更改棋盘、HOLD区、NEXT区、按钮 UI 颜色
        foreach (GameObject item in SpritesUI)
            item.GetComponent<SpriteRenderer>().color = new Color(_r, _g, _b);
        foreach (GameObject item in ButtonsUI)
            item.GetComponent<Image>().color = new Color(_r, _g, _b);

        if (t > T)
        {
            foreach (GameObject item in SpritesUI)
                item.GetComponent<SpriteRenderer>().color = warnColor_fns;
            foreach (GameObject item in ButtonsUI)
                item.GetComponent<Image>().color = warnColor_fns;
            animPlaying[6] = false;
        }
        return;
    }

    // Animation_7 - Spin
    private int rot_dir_3;
    public void Anim_Spin(bool AnimBegin)
    {
        float T = 0.5f;
        float T_mid = 0.05f;
        float t = Time.time;

        float B = originRotation.z;
        float M = originRotation.z - 3f * rot_dir_3;

        if (AnimBegin)
        {
            Operations opt = BattleRecords.operatesOrder[^1];
            if (opt.opt == 4) rot_dir_3 = -1;
            if (opt.opt == 5) rot_dir_3 = +1;

            GetComponent<S_AudioEffect>().PlayAudio(clips[7], 1.0f, 0.2f);
            animBegin[7] = t - Time.deltaTime;
            animPlaying[7] = true;
        }

        t -= animBegin[7];
        float eulerZ = Functions.F_paraBounce(t, T, T_mid, B, M);
        transform.localEulerAngles = new Vector3(0, 0, eulerZ);

        if (t > T)
        {
            ResetTransform();
            animPlaying[7] = false;
        }
        return;
    }

    // Animation_8 - Failed
    private float rot_Z_tilted;
    private GameObject screenMask;
    public void Anim_Failed(bool AnimBegin)
    {
        float T = 1.0f;
        float t = Time.time;

        float B = originPosition.y;
        float M = originPosition.y - Screen.height * 1.2f;

        if (AnimBegin)
        {
            screenMask = GameObject.Find("ScreenMask");
            rot_Z_tilted = Random.Range(-20.0f, 20.0f);
            Option_FailedClear();

            GetComponent<S_AudioEffect>().PlayAudio(clips[8], 1.0f, 2.0f);
            screenMask.GetComponent<Canvas>().sortingOrder = 100;
            screenMask.GetComponent<Image>().color = new Color(0, 0, 0, 0);
            animBegin[8] = t - Time.deltaTime;
            animPlaying[8] = true;
        }

        t -= animBegin[8];
        float posY = Functions.F_paraFadein(t, T, B, M);
        transform.localPosition = new Vector3(0, posY, 0);
        transform.localEulerAngles += new Vector3(0, 0, rot_Z_tilted * Time.deltaTime);
        screenMask.GetComponent<Image>().color += new Color(0, 0, 0, Time.deltaTime);

        if (t > T)
        {
            animPlaying[8] = false;
            LoadInfo.SceneName = "MainMenu";
            SceneManager.LoadScene(1);
        }
        return;
    }

    // Animation_9 - Finish
    public void Anim_Finish(bool AnimBegin)
    {
        float T = 3.0f;
        float t = Time.time;

        if (AnimBegin)
        {
            screenMask = GameObject.Find("ScreenMask");
            GetComponent<S_AudioEffect>().PlayAudio(clips[9], 1.0f, 3.0f);
            screenMask.GetComponent<Canvas>().sortingOrder = 100;
            screenMask.GetComponent<Image>().color = new Color(0, 0, 0, 0);
            animBegin[9] = t - Time.deltaTime;
            animPlaying[9] = true;
        }

        t -= animBegin[9];
        screenMask.GetComponent<Image>().color += new Color(0, 0, 0, Time.deltaTime / 3.0f);

        if (t > T)
        {
            animPlaying[9] = false;
            SceneManager.LoadScene("ScoreScene");
        }
        return;
    }

    // 检查当前堆叠高度+伤害条高度，改变战场颜色
    private int blk_height;
    public GameObject FireWarning;
    private GameObject cur_fw;
    private void State_Warning()
    {
        blk_height = GetComponent<GameProcess>().blockHeight + GetComponent<S_Battle>().DMG_Height;
        if (blk_height > 12)
        {
            if (blk_height <= 15)
            {
                Color warnColor = new Color(1, 1, 1 - (blk_height - 12) / 3.0f);
                if (warnColor_fns != warnColor)
                {
                    warnColor_fns = warnColor;
                    Anim_Warning(true);
                }
            }
            else if (blk_height <= 18)
            {
                Color warnColor = new Color(1, 1 - (blk_height - 15) / 3.0f, 0);
                if (warnColor_fns != warnColor)
                {
                    warnColor_fns = warnColor;
                    Anim_Warning(true);
                }
            } 
            else
            {
                Color warnColor = new Color(1, 0, 0);
                if (warnColor_fns != warnColor)
                {
                    warnColor_fns = warnColor;
                    Anim_Warning(true);
                }
            } 
        } 
        else
        {
            Color warnColor = new Color(1, 1, 1);
            if (warnColor_fns != warnColor)
            {
                warnColor_fns = warnColor;
                Anim_Warning(true);
            }
        }

        State_Danger(blk_height);
    }

    // 判断是否为极危情况，并展示或关闭对应效果
    public bool dangerState = false;        // 当前是否为极危状态（用于其他脚本引用决定是否显示警示信息）
    private void State_Danger(int blk_height)
    {
        if (blk_height >= 20)
        {
            // 生成火焰喷射粒子效果
            if (cur_fw == null)
            {
                cur_fw = Instantiate(FireWarning, transform);
                int[] nxt_id = GetComponent<GenerateMinos>().GetOrder(2);
                GetComponent<DrawNewMinos>().DrawForbiddenCross(nxt_id[1], true);
            }
            dangerState = true;
        }
        else
        {
            // 关闭火焰喷射粒子效果
            if (cur_fw != null)
            {
                cur_fw.GetComponent<S_UIWarning>().Relive();
                cur_fw = null;
            }
            dangerState = false;
        }
        return;
    }

    // 检查当前堆叠高度，堆叠过高时战场随 Mino 的位置倾斜
    private void State_Tilt()
    {
        blk_height = GetComponent<GameProcess>().blockHeight;
        GameObject actMino = GameObject.FindGameObjectWithTag("ActMino");
        if (actMino == null) return;
        if (blk_height <= 15) return;

        float tilt_x = -actMino.transform.position.x;
        float _w = blk_height / 40.0f;
        float tgtRot = tilt_x * _w;
        transform.localEulerAngles = new Vector3(0, 0, tgtRot);
        return;
    }

    // 游戏结束，清除所有视觉效果
    private void Option_FailedClear()
    {
        for (int i = 0; i < 8; i++) animPlaying[i] = false;
        GetComponent<GameProcess>().blockHeight = 0;
        GetComponent<S_Battle>().DMG_Height = 0;
    }
}
