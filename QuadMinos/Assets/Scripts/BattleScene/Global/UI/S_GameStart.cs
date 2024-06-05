using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScaleInScreen
{
    /// <summary>
    /// 根据传入的 _h 计算应选取的 LocalScale，其中 _h 范围在 [0.0f,1.0f] 表示宽度占屏幕的高度
    /// </summary>
    /// <param name="_h">表示占屏幕的高度，该值应在 [0.0f, 1.0f] 内</param>
    /// <param name="_gobj">表示要计算缩放值的对象，该对象应保证旋转为零</param>
    /// <returns></returns>
    public static float Get_Scale(float _h, GameObject _gobj)
    {
        Vector3[] corners = new Vector3[4]; // 获取四个角的屏幕坐标，顺序：左下、左上、右上、右下
        Vector3 originScale = _gobj.transform.localScale;
        _gobj.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        _gobj.GetComponent<RectTransform>().GetWorldCorners(corners);
        float ScreenHeight = Screen.height;
        float y_d = Camera.main.WorldToScreenPoint(corners[0]).y;
        float y_u = Camera.main.WorldToScreenPoint(corners[1]).y;
        float h = y_u - y_d;    // 用左上角纵坐标减左下角纵坐标得高度（需保证旋转为零）
        _gobj.transform.localScale = originScale;
        return _h / (h / ScreenHeight);
    }

    /// <summary>
    /// 根据传入的 GameObject 对象，求其在屏幕上的高度
    /// </summary>
    /// <param name="_gobj">表示要计算屏幕高度的对象，该对象应保证旋转为零</param>
    /// <returns>
    /// 返回给定 GameObject 在屏幕上的高度，以单位屏幕坐标为单位
    /// </returns>
    public static float Get_Height(GameObject _gobj) {
        Vector3[] corners = new Vector3[4]; // 获取四个角的屏幕坐标，顺序：左下、左上、右上、右下
        _gobj.GetComponent<RectTransform>().GetWorldCorners(corners);
        float y_d = Camera.main.WorldToScreenPoint(corners[0]).y;
        float y_u = Camera.main.WorldToScreenPoint(corners[1]).y;
        return y_u - y_d;    // 用左上角纵坐标减左下角纵坐标得高度（需保证旋转为零）
    }

    /// <summary>
    /// 根据传入的 _h 计算应选取的 localPosition.y，其中 _h 范围在 [0.0f, 1.0f] 表示顶部到屏幕顶部距离的占比
    /// </summary>
    /// <param name="_h">表示占屏幕的高度，该值应在 [0.0f, 1.0f] 内</param>
    /// <param name="TGT_S">表示计划的缩放值（此函数应在 Get_Scale 函数之后执行）</param>
    /// <param name="_gobj">表示要计算 y 位置的对象</param>
    /// <returns></returns>
    public static float Get_PosY(float _h, float TGT_S, GameObject _gobj)
    {
        Vector3 originScale = _gobj.transform.localScale;
        Vector3 originPos = _gobj.transform.localPosition;

        _gobj.transform.localPosition = new Vector3(0, 0, 0);
        _gobj.transform.localScale = new Vector3(TGT_S, TGT_S, TGT_S);
        Vector3[] corners = new Vector3[4]; // 获取四个角的屏幕坐标，顺序：左下、左上、右上、右下
        _gobj.GetComponent<RectTransform>().GetWorldCorners(corners);
        float zero_height = Camera.main.WorldToScreenPoint(corners[1]).y;

        _gobj.transform.localPosition = new Vector3(0, 1000, 0);
        _gobj.GetComponent<RectTransform>().GetWorldCorners(corners);
        float one_height = Camera.main.WorldToScreenPoint(corners[1]).y;

        float TGT_Height = Screen.height * (1 - _h);

        _gobj.transform.localPosition = originPos;
        _gobj.transform.localScale = originScale;

        return (TGT_Height - zero_height) / (one_height - zero_height) * 1000;
    }
}

public class S_GameStart : MonoBehaviour
{
    public AudioClip timerClip;             // 倒计时音效

    private Text T;
    private GameObject field;

    private float timer = 0.0f;
    private bool flg1, flg2;
    private bool GAME_START;

    void Anim_Timer()
    {
        T.fontSize += 1;
        T.color -= new Color(0, 0, 0, Time.deltaTime / 3);

        if (timer > 1.0f && flg1)
        {
            T.text = "1";
            T.fontSize = 256;
            T.color = Color.white;
            flg1 = false;
        }
        if (timer > 2.0f && flg2)
        {
            field.GetComponent<GameProcess>().GAME_START();
            T.text = " START!";
            T.fontSize = 96;
            T.color = new Color(0.95686f, 0.80392f, 0.00000f, 1.0f);
            flg2 = false;
        }
        if (timer > 2.0f)
        {
            if (T.fontSize > 128) T.fontSize = 128;
        }
        return;
    }

    void Anim_Field()
    {
        if (GAME_START) { return; }
        // 缩放至战斗区域占屏幕高度的 85%
        float TGT_S = ScaleInScreen.Get_Scale(0.85f, field);
        float cur_scale = Functions.F_paraFadeout(timer, 2.0f, ScaleInScreen.Get_Scale(0.2f, field), TGT_S);
        field.transform.localScale = new Vector3(cur_scale, cur_scale, cur_scale);
        // 位移至战斗区域上方处于屏幕高度的 11% 处
        float TGT_P = ScaleInScreen.Get_PosY(0.11f, TGT_S, field);
        float cur_posY = Functions.F_paraFadeout(timer, 2.0f, 0.0f, TGT_P);
        field.transform.localPosition = new Vector3(0, cur_posY, 0);
        if (timer > 2.0f)
        {
            GAME_START = true;
            field.transform.localPosition = new Vector3(0, TGT_P, 0);
            field.transform.localScale = new Vector3(TGT_S, TGT_S, TGT_S);

            field.GetComponent<S_VisualEffect>().originPosition = field.transform.localPosition;
            field.GetComponent<S_VisualEffect>().originRotation = field.transform.localEulerAngles;
            field.GetComponent<S_VisualEffect>().originScaltion = field.transform.localScale;
        }
        return;
    }

    // Start is called before the first frame update
    void Start()
    {
        // 获取对应组件 / 对象，以及初始化动画状态
        field = GameObject.FindGameObjectWithTag("Field");
        float fieldInitScale = ScaleInScreen.Get_Scale(0.2f, field);
        field.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
        field.transform.localScale = new Vector3(fieldInitScale, fieldInitScale, fieldInitScale);

        float thisInitScale = ScaleInScreen.Get_Scale(0.5f, gameObject);
        transform.localScale = new Vector3(thisInitScale, thisInitScale, thisInitScale);

        T = GetComponent<Text>();
        flg1 = flg2 = true;
        GAME_START = false;

        // 播放准备倒计时音效
        field.GetComponent<S_AudioEffect>().PlayAudio(timerClip, 1.0f, 5.0f);
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        Anim_Timer();
        Anim_Field();
        if (timer > 5.0f) Destroy(gameObject);
    }
}
