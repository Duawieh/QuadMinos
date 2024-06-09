using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class S_UITimeShower : MonoBehaviour
{
    private GameObject field;

    private string GetTime(float sec)
    {
        string rst = "";

        int _min = (int)sec / 60;

        if (_min < 10) rst += "0";
        if (_min < 1) rst += "0";
        else rst += _min.ToString();
        
        rst += ":";
        sec %= 60.0f;

        if (sec < 10) rst += "0";
        rst += sec.ToString();

        if (rst.Length == 5) rst += ".";
        if (rst.Length > 9) rst = rst.Substring(0, 9);
        while (rst.Length < 9) rst += "0";
        return rst;
    }

    public void updateUItimer() {
        float T = BattleScore._Time;
        GetComponent<Text>().text = GetTime(T);
    }

    // Start is called before the first frame update
    void Start()
    {
        field = GameObject.FindGameObjectWithTag("Field");
        GetComponent<Text>().text = "00:00.000";
    }

    // Update is called once per frame
    void Update()
    {
        if (field.GetComponent<S_Score>().BEGIN_TIME == -1) return;
        updateUItimer();
        return;
    }
}
