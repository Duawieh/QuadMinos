using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 玩家在创建房间前设置的房间信息
/// </summary>
public static class MultipleGameSettings
{
    public static string roomName;

    public static int maxPlayers;
    public static int serverTick;
    public static int port;

    public static float Gravity;
    public static float GravRate;
    public static float LockTime;

    public static void Init()
    {
        maxPlayers = 32;
        serverTick = 32;
        port = 32123;
        Gravity = 0.0167f;
        GravRate = 0.0005f;
        LockTime = 0.5f;

        IPAddress[] ipadrs = Dns.GetHostAddresses(Dns.GetHostName());
        foreach (IPAddress ip in ipadrs)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                roomName = ip.ToString() + ":" + port.ToString();
                break;
            }
        }

        return;
    }
}

public class S_CostumMultiPlayers : MonoBehaviour
{
    public GameObject Inputfield_RoomName;
    public GameObject Inputfield_MaxPlayers;
    public GameObject Inputfield_ServerTick;
    public GameObject Inputfield_Port;
    public GameObject Inputfield_Gravity;
    public GameObject Inputfield_GravRate;
    public GameObject Inputfield_LockTime;

    private void Start()
    {
        UpdateGUI();
    }

    private void UpdateGUI()
    {
        File_Settings.SaveSettings();
        Inputfield_RoomName.GetComponent<InputField>().text = MultipleGameSettings.roomName;
        Inputfield_MaxPlayers.GetComponent<InputField>().text = MultipleGameSettings.maxPlayers.ToString("D1");
        Inputfield_Port.GetComponent<InputField>().text = MultipleGameSettings.port.ToString("D1");
        Inputfield_ServerTick.GetComponent<InputField>().text = MultipleGameSettings.serverTick.ToString("D1");
        Inputfield_Gravity.GetComponent<InputField>().text = MultipleGameSettings.Gravity.ToString("F4");
        Inputfield_GravRate.GetComponent<InputField>().text = MultipleGameSettings.GravRate.ToString("F4");
        Inputfield_LockTime.GetComponent<InputField>().text = MultipleGameSettings.LockTime.ToString("F4");
        return;
    }

    public void OnEndEdit_Inputfield_RoomName()
    {
        string _input = Inputfield_RoomName.GetComponent<InputField>().text;
        if (_input != "") MultipleGameSettings.roomName = _input;
        UpdateGUI();
    }

    public void OnEndEdit_Inputfield_MaxPlayers()
    {
        string _input = Inputfield_MaxPlayers.GetComponent<InputField>().text;
        int _value;
        if (int.TryParse(_input, out _value))
        {
            // 最大人数的合法范围在 2 ~ 32
            if (_value >= 2 && _value <= 32)
            {
                MultipleGameSettings.maxPlayers = _value;
            }
        }
        UpdateGUI();
    }

    public void OnEndEdit_Inputfield_ServerTick()
    {
        string _input = Inputfield_ServerTick.GetComponent<InputField>().text;
        int _value;
        if (int.TryParse(_input, out _value))
        {
            // 服务器帧数的合法范围在 8 ~ 256
            if (_value >= 8 && _value <= 256)
            {
                MultipleGameSettings.serverTick = _value;
            }
        }
        UpdateGUI();
    }

    public void OnEndEdit_Inputfield_Port()
    {
        string _input = Inputfield_Port.GetComponent<InputField>().text;
        int _value;
        if (int.TryParse(_input, out _value))
        {
            // 端口号的合法范围在 1024~65535
            if (_value >= 1024 && _value < 65536)
            {
                MultipleGameSettings.port = _value;
            }
        }
        UpdateGUI();
    }

    public void OnEndEdit_Inputfield_Gravity()
    {
        string _input = Inputfield_Gravity.GetComponent<InputField>().text;
        float _value;
        if (float.TryParse(_input, out _value))
        {
            // 重力的合法范围在 0.0 ~ 20.0
            if (_value >= 0.0f && _value <= 20.0f)
            {
                MultipleGameSettings.Gravity = _value;
            }
        }
        UpdateGUI();
    }

    public void OnEndEdit_Inputfield_GravRate()
    {
        string _input = Inputfield_GravRate.GetComponent<InputField>().text;
        float _value;
        if (float.TryParse(_input, out _value))
        {
            // 重力增长率的合法范围在 0.0 ~ 1.0
            if (_value >= 0.0f && _value <= 1.0f)
            {
                MultipleGameSettings.GravRate = _value;
            }
        }
        UpdateGUI();
    }

    public void OnEndEdit_Inputfield_LockTime()
    {
        string _input = Inputfield_LockTime.GetComponent<InputField>().text;
        float _value;
        if (float.TryParse(_input, out _value))
        {
            // 锁定延迟的合法范围在 0.25 ~ INF
            if (_value >= 0.25f)
            {
                MultipleGameSettings.LockTime = _value;
            }
        }
        UpdateGUI();
    }
}
