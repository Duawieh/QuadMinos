using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class S_UIScore : MonoBehaviour
{
    private GameObject field;

    public GameObject UI_ClearType;
    public GameObject UI_ClearLines;
    public GameObject UI_ComboNum;
    public GameObject UI_B2BNum;
    public GameObject UI_Flare;
    public GameObject UI_Damage;
    public AudioClip Clip_BreakB2B;
    public AudioClip Clip_SPIN;
    public AudioClip Clip_QUAD;

    // Start is called before the first frame update
    void Start()
    {
        field = GameObject.FindGameObjectWithTag("Field");
    }

    // 调用 T-SPIN Clear 对应的 UI
    public void TSpin()
    {
        GameObject flr;
        // 生成 T-SPIN 提示和炫光效果，播放 T-SPIN 音效
        UI_ClearType.GetComponent<S_UIClearType>().Init();
        flr = Instantiate(UI_Flare, transform);
        flr.transform.SetAsFirstSibling();
        flr.GetComponent<S_UIFlare>().Init(Color.white, 0.7f, UI_ClearType.transform.position);
        field.GetComponent<S_AudioEffect>().PlayAudio(Clip_SPIN, 1.0f, 1.0f);
        return;
    }

    public void Clear(int _lines, int _combo, int _b2b)
    {
        UI_ClearLines.GetComponent<S_UIClearLines>().Init(_lines);
        // 消四 UI 效果和音效
        if (_lines == 4)
        {
            field.GetComponent<S_AudioEffect>().PlayAudio(Clip_QUAD, 1.0f, 2.0f);
            GameObject flr = Instantiate(UI_Flare, transform);
            flr.transform.SetAsFirstSibling();
            flr.GetComponent<S_UIFlare>().Init(new Color(1, 0.8039216f, 0), 0.6f, UI_ClearLines.transform.position);
        }
        // 连击 UI 效果和连击炫光
        if (_combo > 1)
        {
            UI_ComboNum.GetComponent<S_UICombo>().Init(_combo);
            GameObject flr = Instantiate(UI_Flare, transform);
            flr.transform.SetAsFirstSibling();
            flr.GetComponent<S_UIFlare>().Init(Color.white, 0.6f, UI_ComboNum.transform.position);
        }
        // B2B UI 效果（通过是否消行判断是否为 MINI SPIN）
        if (_b2b > 1 && _lines > 0)
        {
            UI_B2BNum.GetComponent<S_UIBackToBack>().Init(_b2b);
            GameObject flr = Instantiate(UI_Flare, transform);
            flr.transform.SetAsFirstSibling();
            flr.GetComponent<S_UIFlare>().Init(new Color(1, 0.8039216f, 0), 0.6f, UI_B2BNum.transform.position);
        }
    }

    public void BreakBackToBack()
    {
        field.GetComponent<S_AudioEffect>().PlayAudio(Clip_BreakB2B, 1.0f, 1.0f);
        UI_B2BNum.GetComponent<S_UIBackToBack>().Finish();
        return;
    }

    // 传入伤害量，实例化伤害条，并返回伤害条所在世界坐标
    public Vector3 DamageBar(int _dmg)
    {
        GameObject _bar = Instantiate(UI_Damage, field.transform);
        _bar.transform.localScale= Vector3.one;

        // 调整新生成的伤害条到伤害条队列最上方
        float _h = 0.0f;
        ref List<GameObject> dmgBars = ref field.GetComponent<S_Battle>().DMG;
        foreach (GameObject dmgBar in dmgBars)
        {
            int barHeight = dmgBar.GetComponent<S_UIDamage>().DMG;
            _h += dmgBar.GetComponent<RectTransform>().rect.height * barHeight;
        }
        _bar.transform.localPosition += new Vector3(0, _h, 0);

        if (field.GetComponent<GameProcess>().ReviewMode) {
            _bar.GetComponent<S_UIDamage>().Init();
        }
        else {
            // 调用初始化函数，将伤害条添加到伤害列表
            int _ept = _bar.GetComponent<S_UIDamage>().Init(_dmg, 0);
            // 若开启记录模式，保存伤害记录
            if (field.GetComponent<GameProcess>().RecordMode) {
                BattleRecords.garbageOrder.Add(new Vector3
                    (BattleRecords.operatesOrder.Count - 1, _ept, _dmg));
            }
        }

        if (field.GetComponent<GameProcess>().RecordMode) {

        }

        dmgBars.Add(_bar);
        field.GetComponent<S_Battle>().DMG_Height += _dmg;
        return _bar.transform.position;
    }
}
