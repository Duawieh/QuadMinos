using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class S_playerOprate : MonoBehaviour
{
    private float[] clickTimer = new float[7];
    private bool[] clicked = new bool[7];

    /***********************
     * 按钮编号-功能对照表
     * 
     * 0 - 左平移
     * 1 - 右平移
     * 2 - 软降
     * 3 - 换块
     * 4 - 逆时针转
     * 5 - 顺时针转
     * 6 - 硬降
     * 
     * *********************/

    private const float LONG_PRESS = 0.20f;     // 长按判定时长

    public GameObject moveBackground;           // 触控遥感区域背景对象（既作为背景，也负责划定触控范围）
    public GameObject moveHandle;               // 触控遥感对象

    private GameObject mino;                    // 活动 mino
    private GameObject field;                   // 游戏场地

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 7; i++) {
            clickTimer[i] = 0.0f;
            clicked[i] = false;
        }
        field = GameObject.FindGameObjectWithTag("Field");
    }

    // Update is called once per frame
    void Update()
    {
        mino = GameObject.FindGameObjectWithTag("ActMino");
        TouchMoveHandle();
        OperateOnKeyboard();
    }

    // 键盘操作转换
    private void OperateOnKeyboard()
    {
        if (Input.GetKey(KeyCode.LeftArrow)) {
            Click_MoveLeft();
            clicked[0] = true;
        }
        else clicked[0] = false;
        if (Input.GetKey(KeyCode.RightArrow))
        {
            Click_MoveRight();
            clicked[1] = true;
        }
        else clicked[1] = false;
        if (Input.GetKey(KeyCode.DownArrow))
        {
            Click_Drop();
            clicked[2] = true;
        }
        else clicked[2] = false;
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.C)) {
            Click_Hold();
            clicked[3] = true;
        }
        else clicked[3] = false;
        if (Input.GetKey(KeyCode.Z)) {
            Click_TurnAnticlock();
            clicked[4] = true;
        }
        else clicked[4] = false;
        if (Input.GetKey(KeyCode.X)) {
            Click_TurnClock();
            clicked[5] = true;
        }
        else clicked[5] = false;
        if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.UpArrow)) {
            Click_HardDrop();
            clicked[6] = true;
        }
        else clicked[6] = false;
        LongPressOperations();
        return;
    }

    // 长按判定函数（仅键盘操作有效）
    private void LongPressOperations()
    {
        for (int i = 0; i < 7; i++)
            if (clicked[i]) LongPress(i);
            else clickTimer[i] = 0.0f;
        return;
    }

    // 长按（仅键盘操作下生效）
    private void LongPress(int buttonId)
    {
        if (clickTimer[buttonId] < LONG_PRESS) return;
        clicked[buttonId] = false;
        if (buttonId == 0) Click_MoveLeft();
        if (buttonId == 1) Click_MoveRight();
        if (buttonId == 2) Click_Drop();
        clicked[buttonId] = true;
        return;
    }

    //----------------------------------------------------------------------
    // 遥感触控
    //----------------------------------------------------------------------
    private float Get_Radius()
    {
        Vector3[] corners = new Vector3[4]; // 获取四个角的屏幕坐标，顺序：左下、左上、右上、右下
        moveBackground.GetComponent<RectTransform>().GetWorldCorners(corners);
        float y_u = Camera.main.WorldToScreenPoint(corners[1]).y;
        float y_d = Camera.main.WorldToScreenPoint(corners[0]).y;
        return (y_u - y_d) * (y_u - y_d);   // 用左上角纵坐标减左下角纵坐标得半径（需保证旋转为零）
    }

    // 将鼠标点击事件转化为 Touch 对象（仅用于在 PC 上进行测试）
    private Touch temp_p;
    private Touch TestMouseTouch(float R, Vector2 C)
    {
        Touch moveTouch = new();
        moveTouch.phase = TouchPhase.Canceled;
        // 将鼠标点击事件转化为 Touch 对象
        if (Input.GetMouseButton(0))
        {
            if (temp_p.phase == TouchPhase.Ended)
            {
                temp_p.rawPosition = Input.mousePosition;
                temp_p.phase = TouchPhase.Began;
            } 
            else
            {
                if (Input.GetAxis("Mouse X") != 0.0f || Input.GetAxis("Mouse Y") != 0.0f)
                {
                    temp_p.phase = TouchPhase.Moved;
                }
                else
                {
                    temp_p.phase = TouchPhase.Stationary;
                }
            }
            temp_p.position = Input.mousePosition;
        }
        else
        {
            temp_p.phase = TouchPhase.Ended;
        }
        // 将 Touch 事件赋值为 moveTouch 后返回
        if (temp_p.phase != TouchPhase.Ended && temp_p.phase != TouchPhase.Canceled)
        {
            if (temp_p.rawPosition.y <= C.y)
            {
                if ((temp_p.rawPosition - C).sqrMagnitude <= R) moveTouch = temp_p;
            }
            else
            {
                if (((temp_p.rawPosition - C) * 3).sqrMagnitude <= R) moveTouch = temp_p;
            }
        }
        return moveTouch;
    }

    // 由触摸点更新遥感位置
    private void HandlePositionUpdate(float R, Vector2 C, Touch moveTouch)
    {
        // 根据获取到的 moveTouch 更新手柄位置
        if (moveTouch.phase != TouchPhase.Ended && moveTouch.phase != TouchPhase.Canceled)
        {
            Vector2 vec = moveTouch.position - C;
            // 将遥感限制在触控检测区域内
            if (vec.y > 0) vec = new Vector2(vec.x, 0);
            if (vec.sqrMagnitude > R) vec = vec.normalized * Mathf.Sqrt(R);
            vec += C;
            Vector3 handlePos = Camera.main.ScreenToWorldPoint(vec);
            moveHandle.transform.position = new Vector3(handlePos.x, handlePos.y, 0);
        }
        else
        {
            moveHandle.transform.localPosition = Vector3.zero;
        }
        return;
    }

    // 摇杆操作
    private int moveHorizenTimer = 0;
    private int moveVertenTimer = 0;
    private void MoveOperations(float R, Vector2 C)
    {
        Vector2 P = Camera.main.WorldToScreenPoint(moveHandle.transform.position);
        R = Mathf.Sqrt(R);
        // 摇杆水平拖动超过五分之一半径时视为进行操作
        // 水平移动
        if (Mathf.Abs((P - C).x * 5) >= R)
        {
            if (moveHorizenTimer == 0)
            {
                if ((P - C).x < 0) Click_MoveLeft();
                else Click_MoveRight();
            }
            moveHorizenTimer++;
            if (moveHorizenTimer > ResetTime_Horizen(Mathf.Abs((P - C).x), R)) moveHorizenTimer = 0;
        } else moveHorizenTimer = 0;
        // 摇杆垂直拖动超过三分之一半径时视为进行操作
        // 垂直移动
        if ((C - P).y * 3f >= R)
        {
            if (moveVertenTimer == 0)
            {
                Click_Drop();
            }
            moveVertenTimer++;
            if (moveVertenTimer > ResetTime_Vertical((C - P).y, R)) moveVertenTimer = 0;
        }
        else moveVertenTimer = 0;
    }
    // 根据摇杆移动量计算下一次操作之间间隔的帧数（最慢 12 帧，最快 0 帧）
    private float ResetTime_Horizen(float _pivot, float R)
    {
        _pivot -= R / 5;
        R *= 0.8f;
        return Mathf.Ceil(-12 / R * _pivot + 12);
    }
    // 根据摇杆移动量计算下一次操作之间间隔的帧数（最慢 20 帧，最快 0 帧）
    private float ResetTime_Vertical(float _pivot, float R)
    {
        _pivot -= R / 3;
        R *= 0.666666666f;
        float _G = field.GetComponent<GameProcess>().Gravity;
        return Mathf.Ceil(-0.3f / _G / R * _pivot + 0.3f / _G);
    }

    private void TouchMoveHandle()
    {
        Touch moveTouch = new();
        moveTouch.phase = TouchPhase.Canceled;
        // 获取手柄触控区在屏幕上的半径的平方
        float R = Get_Radius();
        // 获取手柄触控区在屏幕上的中心坐标
        Vector2 C = Camera.main.WorldToScreenPoint(moveBackground.transform.position);
        // 获取（更新）位于手柄触控区内的 Touch 对象
        foreach (Touch p in Input.touches)
        {
            if (p.phase == TouchPhase.Ended || p.phase == TouchPhase.Canceled) continue;
            else { 
                if (p.rawPosition.y <= C.y)
                {
                    if ((p.rawPosition - C).sqrMagnitude <= R) moveTouch = p;
                } 
                else
                {
                    if (((p.rawPosition - C) * 3).sqrMagnitude <= R) moveTouch = p;
                }
            }
        }

        // 仅应在 PC 端调试时具有该语句
        //-------------------------------
        //moveTouch = TestMouseTouch(R, C);
        //-------------------------------

        HandlePositionUpdate(R, C, moveTouch);
        MoveOperations(R, C);
        return;
    }
    //---------------------------------------------------------------
    //---------------------------------------------------------------

    public void Click_MoveLeft()
    {
        clickTimer[0] += Time.deltaTime;
        if (clicked[0]) return;

        if (mino == null) return;
        mino.GetComponent<Mino_Active>().Operate_L();
    }

    public void Click_MoveRight()
    {
        clickTimer[1] += Time.deltaTime;
        if (clicked[1]) return;

        if (mino == null) return;
        mino.GetComponent<Mino_Active>().Operate_R();
    }

    public void Click_Drop()
    {
        clickTimer[2] += Time.deltaTime;
        if (clicked[2]) return;

        if (mino == null) return;
        mino.GetComponent<Mino_Active>().Operate_D();
    }

    public void Click_Hold()
    {
        clickTimer[3] += Time.deltaTime;
        if (clicked[3]) return;

        if (mino == null) return;
        mino.GetComponent<Mino_Active>().Operate_H();
    }

    public void Click_TurnAnticlock()
    {
        clickTimer[4] += Time.deltaTime;
        if (clicked[4]) return;

        if (mino == null) return;
        mino.GetComponent<Mino_Active>().Operate_A();
    }

    public void Click_TurnClock()
    {
        clickTimer[5] += Time.deltaTime;
        if (clicked[5]) return;

        if (mino == null) return;
        mino.GetComponent<Mino_Active>().Operate_C();
    }

    public void Click_HardDrop()
    {
        clickTimer[6] += Time.deltaTime;
        if (clicked[6]) return;

        if (mino == null) return;
        mino.GetComponent<Mino_Active>().Operate_HD();
    }
}
