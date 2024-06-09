using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mino_Locked : MonoBehaviour
{
    public PosIndex p_id;

    public GameObject UI_LockStar;
    public GameObject UI_ClearStar;

    private GameObject field;
    private int minoType = 7;   // 默认为 7，即认为是垃圾行方块

    // 实例化粒子系统，展示锁定粒子效果
    private void LockStar(int _type)
    {
        if (_type >= 7) return;
        GameObject _star = Instantiate(UI_LockStar, field.transform.parent);
        _star.GetComponent<S_ParticleStar>().Init(_type);
        _star.transform.position = transform.position;
        minoType = _type;
        return;
    }

    // 实例化粒子系统，展示消除粒子效果
    private void ClearStar(int _type)
    {
        GameObject _star = Instantiate(UI_ClearStar, field.transform.parent);
        _star.GetComponent<S_ParticleStar>().Init(_type);
        _star.transform.position = transform.position;
        return;
    }

    // 由战场坐标转化为 transform 相对坐标，此函数仅应在方块活动时被调用
    public void RefreshPosition(PosIndex localP)
    {
        p_id = localP;
        transform.localPosition = p_id.GetPosition() + new Vector2(1.76f, 3.36f);
        return;
    }
    
    // 被消除时执行
    public void Clear()
    {
        ClearStar(minoType);
        Destroy(gameObject);
        return;
    }

    // 锁定初始化
    public void Lock(PosIndex p, int _type)
    {
        field = GameObject.FindGameObjectWithTag("Field");
        transform.parent = field.transform;
        transform.tag = "LockUnit";
        p_id = p;

        // 因为某些未知原因的 BUG 不得不添加了下面的内容，用于规范锁定的方块的位置
        //-------------------------------------------------------------
        transform.localPosition = p_id.GetPosition();
        transform.localEulerAngles = Vector3.zero;
        transform.localScale = Vector3.one;
        //-------------------------------------------------------------

        field.GetComponent<PlayfieldState>().takenBy[p_id.x_id][p_id.y_id] = gameObject;

        // 显示锁定粒子特效
        LockStar(_type);

        return;
    }
}
