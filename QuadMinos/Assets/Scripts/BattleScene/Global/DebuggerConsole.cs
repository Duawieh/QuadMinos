using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;

public class DebuggerConsole : MonoBehaviour
{
    Text txt;

    // Start is called before the first frame update
    void Start()
    {
        txt = GetComponent<Text>();
        txt.text = "DebuggerConsole:";
    }

    public void Log(string msg)
    {
        string _time = "[" + DateTime.Now.ToString() + "] ";
        txt.text += "\n";
        txt.text += _time;
        txt.text += msg;
        return;
    }
}
