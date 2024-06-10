using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_UIDamage : MonoBehaviour
{
    public int DMG = 0;         // 造成的伤害行数
    public int EPT = 0;         // 产生垃圾行时空出的列

    // 在 0.1s 内完成长度变化
    private IEnumerator Anim_ChangeLength(int _from, int _to)
    {
        float len = transform.localScale.y;
        float origin_len = len;
        float _t = 0.0f;
        while (_t <= 0.1f)
        {
            _t += Time.deltaTime;
            len = Functions.F_paraFadeout(_t, 0.1f, origin_len, _to - 0.1f);
            transform.localScale = new Vector3(1, len, 1);
            yield return null;
        }
        yield break;
    }

    // 在删除自身前的 0.1s 内展示消失动画
    private IEnumerator Anim_Disappear()
    {
        float len = transform.localScale.y;
        float origin_len = len;
        float _t = 0.0f;
        while (_t <= 0.1f)
        {
            _t += Time.deltaTime;
            float width = Functions.F_paraFadein(_t, 0.1f, 1.0f, 0.0f);
            len = Functions.F_paraFadein(_t, 0.1f, origin_len, origin_len + 1.0f);
            transform.localScale = new Vector3(width, len, 1);
            yield return null;
        }
        Destroy(gameObject);
        yield break;
    }

    /// <summary>
    /// 初始化伤害条，包括播放伤害条积攒的动画，计算空缺列的位置
    /// </summary>
    /// <param name="_DMG">传入的伤害量行数</param>
    /// <param name="_EPT">传入的列数，若有记录则传入记录，无记录则传入 0 表示随机生成</param>
    /// <returns>返回空缺的列数用于记录</returns>
    public int Init(int _DMG, int _EPT)
    {
        transform.localScale = new Vector3(1, 0, 1);
        StopAllCoroutines();
        StartCoroutine(Anim_ChangeLength(0, _DMG));

        DMG = _DMG;
        if (_EPT != 0) EPT = _EPT;
        else EPT = Random.Range(1, 11);
        
        return EPT;
    }

    public void ChangeLength(int _DMG)
    {
        StopAllCoroutines();
        StartCoroutine(Anim_ChangeLength(DMG, _DMG));
        DMG = _DMG;
        return;
    }

    public void Disappear()
    {
        StopAllCoroutines();
        StartCoroutine(Anim_Disappear());
        return;
    }
}
