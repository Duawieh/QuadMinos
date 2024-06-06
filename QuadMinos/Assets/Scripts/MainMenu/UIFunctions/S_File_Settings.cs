using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// 玩家在 Setting Panel 设置的信息
/// </summary>
public class GameSettings
{
    public static float Gravity;
    public static float LockTime;
    public static float GarbageProb;
    public static float GarbageRatio;

    /// <summary>
    /// 攻击模式（1-随机目标 2-歼灭模式 3-反击模式）
    /// </summary>
    public static int AttackMode;
    public static string PlayerID;

    public static float MusicVolume;
    public static float EffectVolume;
    public static bool ShowHUD;
    public static bool ShowShadowblocks;

    public static float OperationVARR;  // 垂直操作最大重复速率（即摇杆拖动到最大时的 ARR）
    public static float OperationHARR;  // 水平操作最大重复速率（即摇杆拖动到最大时的 ARR）
    public static float OperationRAS;   // 自动重复操作最小距离（拖动达此距离后才开始自动重复操作，1 表示拖动到底）

    public static void Init()
    {
        Gravity = 0.0167f;
        LockTime = 1.0f;
        GarbageProb = 0.5f;
        GarbageRatio = 1.0f;
        AttackMode = 1;
        PlayerID = "unkown tourist";
        MusicVolume = 1.0f;
        EffectVolume = 1.0f;
        ShowHUD = true;
        ShowShadowblocks = true;
        OperationVARR = 1.0f;
        OperationHARR = 1.0f;
        OperationRAS = 0.5f;
        return;
    }
}


[Serializable]
class SettingsData
{
    public float Gravity;
    public float LockTime;
    public float GarbageProb;
    public float GarbageRatio;

    public int AttackMode;
    public string PlayerID;

    public float MusicVolume;
    public float EffectVolume;
    public bool ShowHUD;
    public bool ShowShadowblocks;

    public float OperationVARR;
    public float OperationHARR;
    public float OperationRAS;

    public SettingsData()
    {
        Gravity = GameSettings.Gravity;
        LockTime = GameSettings.LockTime;
        GarbageProb = GameSettings.GarbageProb;
        GarbageRatio = GameSettings.GarbageRatio;
        AttackMode = GameSettings.AttackMode;
        PlayerID = GameSettings.PlayerID;
        MusicVolume = GameSettings.MusicVolume;
        EffectVolume = GameSettings.EffectVolume;
        ShowHUD = GameSettings.ShowHUD;
        ShowShadowblocks = GameSettings.ShowShadowblocks;
        OperationVARR = GameSettings.OperationVARR;
        OperationHARR = GameSettings.OperationHARR;
        OperationRAS = GameSettings.OperationRAS;
        return;
    }

    public void UpdateGameSettings()
    {
        GameSettings.Gravity = Gravity;
        GameSettings.LockTime = LockTime;
        GameSettings.GarbageProb = GarbageProb;
        GameSettings.GarbageRatio = GarbageRatio;
        GameSettings.AttackMode = AttackMode;
        GameSettings.PlayerID = PlayerID;
        GameSettings.MusicVolume = MusicVolume;
        GameSettings.EffectVolume = EffectVolume;
        GameSettings.ShowHUD = ShowHUD;
        GameSettings.ShowShadowblocks = ShowShadowblocks;
        GameSettings.OperationVARR = OperationVARR;
        GameSettings.OperationHARR = OperationHARR;
        GameSettings.OperationRAS = OperationRAS;
        return;
    }
}


public class File_Settings {
    /// <summary>
    /// 从 persistentDataPath 中读取设置文件并存入类内
    /// </summary>
    public static IEnumerator GetSettings()
    {
        string pth = Application.persistentDataPath + "/Settings/";
        if (!Directory.Exists(pth)) Directory.CreateDirectory(pth);
        pth += "settings.json";

        if (!File.Exists(pth))
        {
            GameSettings.Init();    // 在迭代器首次退出前就要完成初始化，以防止调用到空信息
            SaveSettings();
            yield return null;
        }

        SettingsData _data;
        string info_js = "";

        using (StreamReader _streamReader = new StreamReader(pth))
        {
            info_js = _streamReader.ReadToEnd();
            _streamReader.Close();
            _streamReader.Dispose();
        }

        _data = JsonUtility.FromJson<SettingsData>(info_js);
        _data.UpdateGameSettings();

        yield break;
    }

    public static void SaveSettings()
    {
        string _dataPathName = Application.persistentDataPath + "/Settings/settings.json";

        SettingsData _data = new();
        string info_js = JsonUtility.ToJson(_data);

        using (StreamWriter _streamWriter = new StreamWriter(_dataPathName))
        {
            _streamWriter.WriteLine(info_js);
            _streamWriter.Flush();
            _streamWriter.Close();
            _streamWriter.Dispose();
        }

        return;
    }
}

public class S_File_Settings : MonoBehaviour
{
    public GameObject Inputfield_Gravity;
    public GameObject Inputfield_LockTime;
    public GameObject Inputfield_GarbageProb;
    public GameObject Inputfield_GarbageRatio;
    public GameObject Dropdown_AttackMode;
    public GameObject Slider_MusicVolume;
    public GameObject Slider_EffectVolume;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(File_Settings.GetSettings());
        UpdateGUI();
        return;
    }

    public void UpdateGUI()
    {
        Inputfield_Gravity.GetComponent<InputField>().text = GameSettings.Gravity.ToString("f4");
        Inputfield_LockTime.GetComponent<InputField>().text = GameSettings.LockTime.ToString("f4");
        Inputfield_GarbageProb.GetComponent<InputField>().text = GameSettings.GarbageProb.ToString("f4");
        Inputfield_GarbageRatio.GetComponent<InputField>().text = GameSettings.GarbageRatio.ToString("f4");

        Dropdown_AttackMode.GetComponent<Dropdown>().value = GameSettings.AttackMode - 1;

        Slider_MusicVolume.GetComponent<Slider>().value = GameSettings.MusicVolume;
        Slider_EffectVolume.GetComponent<Slider>().value = GameSettings.EffectVolume;
        return;
    }
}
