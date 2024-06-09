using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_ProcessAlert : MonoBehaviour
{
    public GameObject Alert_Finish;

    public GameObject Alert_40Line;
    public GameObject Target_40Line;
    public IEnumerator Process_40Line()
    {
        GameObject _title = Instantiate(Alert_40Line);
        _title.GetComponent<S_GameAlertUI>().SetText("尽快消除 40 行！");
        GameObject _tgtLine = Instantiate(Target_40Line, transform);
        _tgtLine.transform.localPosition = new Vector3(0, 9.6f, 0);
        while (true)
        {
            int _lines = BattleScore._Lines;
            _tgtLine.transform.localPosition = new Vector3(0, 9.6f - _lines * 0.32f, 0);

            if (_lines >= 40)
            {
                GetComponent<GameProcess>().finished = true;
                GetComponent<GameProcess>().GAME_OVER();
                Instantiate(Alert_Finish);
                yield break;
            }

            yield return null;
        }
    }

    public GameObject Alert_Blitz;
    public IEnumerator Process_Blitz()
    {
        GameObject _title = Instantiate(Alert_Blitz);
        _title.GetComponent<S_GameAlertUI>().SetText("尽可能在 2 分钟内拿到更高分数！");

        GameObject _alert;
        yield return new WaitForSeconds(58.0f);
        while (BattleScore._Time - 60 < 0) yield return null;

        _alert = Instantiate(Alert_Blitz);
        _alert.GetComponent<S_GameAlertUI>().SetText("剩下 1 分钟");

        yield return new WaitForSeconds(28.0f);
        while (BattleScore._Time - 90 < 0) yield return null;

        _alert = Instantiate(Alert_Blitz);
        _alert.GetComponent<S_GameAlertUI>().SetText("剩下 30 秒");

        yield return new WaitForSeconds(18.0f);
        while (BattleScore._Time - 110 < 0) yield return null;

        _alert = Instantiate(Alert_Blitz);
        _alert.GetComponent<S_GameAlertUI>().SetText("剩下 10 秒");

        yield return new WaitForSeconds(5.0f);
        while (BattleScore._Time - 120 < 0) yield return null;

        BattleScore._Time = 120.0f;
        GetComponent<GameProcess>().finished = true;
        GetComponent<GameProcess>().GAME_OVER();
        Instantiate(Alert_Finish);
    }

    public GameObject Alert_Marathon;
    private float[] levelGravity = new float[] { 0.0000f, 0.0167f, 0.0210f, 0.0270f, 0.0353f, 0.0469f, 0.0636f, 0.0879f, 0.1236f, 0.1775f, 0.2598f, 0.3880f, 0.5900f, 0.9200f, 1.4600f, 2.3600f, 3.9100f, 6.6100f, 11.430f, 20.000f , 20.000f};
    public IEnumerator Process_Marathon()
    {
        // 下面这句被注释掉的语句想不起来是因为什么加在这里的了，暂时保留为注释
        // GetComponent<GameProcess>().finished = true; 

        GameObject _title = Instantiate(Alert_Marathon);
        _title.GetComponent<S_GameAlertUI>().SetText("尽力达到更高目标！");

        int lastLevelLines = 0; // 已消除的所有行数
        int levelupLines = 10;  // 到达下一级还需要消除的行数
        int curLevel = 1;

        while (true)
        {
            if (BattleScore._Lines - lastLevelLines >= levelupLines)
            {
                lastLevelLines += levelupLines;
                curLevel++;
                if (curLevel >= 5) levelupLines = lastLevelLines;
                GameObject _alert = Instantiate(Alert_Marathon);
                _alert.GetComponent<S_GameAlertUI>().SetText("当前等级为 Lv." + curLevel.ToString());
            }
            GetComponent<GameProcess>().Gravity = levelGravity[Mathf.Min(curLevel, 20)];
            GetComponent<GameProcess>().LockTime = Mathf.Max(0.25f, 1 / levelGravity[Mathf.Min(curLevel, 20)] / 60);
            yield return null;
        }
    }
}
