using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class S_ChangeSettings : MonoBehaviour
{
    /// <summary>
    /// 将数字字符串转换为浮点数，当无法转换时返回 -1
    /// </summary>
    /// <param name="_s">要进行转换的字符串，需要保证其表示的数字非负</param>
    /// <returns>由字符串转换得到的浮点数，或 -1.0f 表示非法字符串</returns>
    private float StringToFloat(string _s)
    {
        // 整数部分的值
        float _integerValue = 0.0f;
        // 小数部分的值
        float _floatValue = 0.0f;
        // 小数部分最低位的指数
        float _floatTimes = 1.0f;
        // 小数点所在字符串内的位置（初始化为无穷大）
        int _point = (int)1e9;
        for (int i = 0; i < _s.Length; i++)
        {
            if (_s[i] >= '0' && _s[i] <= '9')
            {
                if (i < _point)
                {
                    _integerValue *= 10;
                    _integerValue += _s[i] - '0';
                } 
                else
                {
                    _floatTimes /= 10.0f;
                    _floatValue += (_s[i] - '0') * _floatTimes;
                }
            }
            else if (_s[i] == '.')
            {
                // 若小数点在开头，认为省去了前文的 0，不报错
                if (_point == 1e9)
                {
                    _point = i;
                }
                // 如果字符串内包含超过一个小数点，报错
                else return -1;
            }
            else if (_s[i] == 'f')
            {
                // 如果 f 出现在末尾，认为它是 float 数的单位，不报错
                if (i != _s.Length - 1) return -1;
            }
            else return -1;
        }
        return _integerValue + _floatValue;
    }

    public void OnEndEdit_Inputfield_Gravity()
    {
        string _input = GetComponent<S_File_Settings>().Inputfield_Gravity.GetComponent<InputField>().text;
        float _value = StringToFloat(_input);
        if (_value != -1.0f)
        {
            // 重力的合法范围在 0.0 ~ 20.0
            if (_value >= 0.0f && _value <= 20.0f)
            {
                GameSettings.Gravity = _value;
                File_Settings.SaveSettings();
            }
        }
        GetComponent<S_File_Settings>().UpdateGUI();
        return;
    }

    public void OnEndEdit_Inputfield_LockTime()
    {
        string _input = GetComponent<S_File_Settings>().Inputfield_LockTime.GetComponent<InputField>().text;
        float _value = StringToFloat(_input);
        if (_value != -1.0f)
        {
            // 锁定延迟的合法范围在 0.25 ~ 0x7fffffff
            if (_value >= 0.25f)
            {
                GameSettings.LockTime = _value;
                File_Settings.SaveSettings();
            }
        }
        GetComponent<S_File_Settings>().UpdateGUI();
        return;
    }

    public void OnEndEdit_Inputfield_GarbageRatio()
    {
        string _input = GetComponent<S_File_Settings>().Inputfield_GarbageRatio.GetComponent<InputField>().text;
        float _value = StringToFloat(_input);
        if (_value != -1.0f)
        {
            // 垃圾行伤害倍率的合法范围在 0.0 ~ 8.0
            if (_value >= 0.0f && _value <= 8.0f)
            {
                GameSettings.GarbageRatio = _value;
                File_Settings.SaveSettings();
            }
        }
        GetComponent<S_File_Settings>().UpdateGUI();
        return;
    }

    public void OnEndEdit_Inputfield_GarbageProb()
    {
        string _input = GetComponent<S_File_Settings>().Inputfield_GarbageProb.GetComponent<InputField>().text;
        float _value = StringToFloat(_input);
        if (_value != -1.0f)
        {
            // 垃圾行添加概率的合法范围在 0.0 ~ 1.0
            if (_value >= 0.0f && _value <= 1.0f)
            {
                GameSettings.GarbageProb = _value;
                File_Settings.SaveSettings();
            }
        }
        GetComponent<S_File_Settings>().UpdateGUI();
        return;
    }

    public void OnEndEdit_Inputfield_MinRAS()
    {
        string _input = GetComponent<S_File_Settings>().Inputfield_MinRAS.GetComponent<InputField>().text;
        float _value = StringToFloat(_input);
        if (_value != -1.0f)
        {
            // 最小 RAS 的合法范围在 0.0 ~ 1.0
            if (_value >= 0.0f && _value <= 1.0f)
            {
                GameSettings.OperationRAS = _value;
                File_Settings.SaveSettings();
            }
        }
        GetComponent<S_File_Settings>().UpdateGUI();
        return;
    }

    public void OnEndEdit_Inputfield_MaxVARR()
    {
        string _input = GetComponent<S_File_Settings>().Inputfield_MaxVARR.GetComponent<InputField>().text;
        float _value = StringToFloat(_input);
        if (_value != -1.0f)
        {
            // 最大 VARR 的合法范围在 0.01 ~ 20.0
            if (_value >= 0.01f && _value <= 20.0f)
            {
                GameSettings.OperationVARR = _value;
                File_Settings.SaveSettings();
            }
        }
        GetComponent<S_File_Settings>().UpdateGUI();
        return;
    }

    public void OnEndEdit_Inputfield_MaxHARR()
    {
        string _input = GetComponent<S_File_Settings>().Inputfield_MaxHARR.GetComponent<InputField>().text;
        float _value = StringToFloat(_input);
        if (_value != -1.0f)
        {
            // 最大 HARR 的合法范围在 0.01 ~ 10.0
            if (_value >= 0.01f && _value <= 10.0f)
            {
                GameSettings.OperationHARR = _value;
                File_Settings.SaveSettings();
            }
        }
        GetComponent<S_File_Settings>().UpdateGUI();
        return;
    }

    public void OnEndEdit_Inputfield_PlayerID()
    {
        string _input = GetComponent<S_File_Settings>().Inputfield_PlayerID.GetComponent<InputField>().text;
        GameSettings.PlayerID = _input;
        File_Settings.SaveSettings();
        GetComponent<S_File_Settings>().UpdateGUI();
        return;
    }

    public void OnChange_Dropdown_AttackMode()
    {
        GameSettings.AttackMode = GetComponent<S_File_Settings>().Dropdown_AttackMode.GetComponent<Dropdown>().value + 1;
        File_Settings.SaveSettings();
        return;
    }

    public void OnChange_Slider_MusicVolume()
    {
        GameSettings.MusicVolume = GetComponent<S_File_Settings>().Slider_MusicVolume.GetComponent<Slider>().value;
        File_Settings.SaveSettings();
        // 拖动音量条的同时改变主菜单背景音乐音量大小，让玩家对音量有直观感受
        GameObject.FindGameObjectWithTag("Field").GetComponent<AudioSource>().volume = GameSettings.MusicVolume;
        return;
    }
    
    public void OnChange_Slider_EffectVolume()
    {
        GameSettings.EffectVolume = GetComponent<S_File_Settings>().Slider_EffectVolume.GetComponent<Slider>().value;
        File_Settings.SaveSettings();
        // 拖动完成后播放一个音效，让玩家对音量有直观感受
        // 注意：主菜单场景中不可出现同名对象
        // 注意：Setting Panel 打开时会播放一次，无伤大雅。
        AudioSource tester = GameObject.Find("EffectAudioPlayer").GetComponent<AudioSource>();
        tester.volume = GameSettings.EffectVolume;
        tester.Play();
        return;
    }

    public void OnChange_Toggle_ShowHUD()
    {
        GameSettings.ShowHUD = GetComponent<S_File_Settings>().Toggle_ShowHUD.GetComponent<Toggle>().isOn;
        File_Settings.SaveSettings();
        return;
    }

    public void OnChange_Toggle_ShowShadowBlock()
    {
        GameSettings.ShowShadowblock = GetComponent<S_File_Settings>().Toggle_ShowShadowBlock.GetComponent<Toggle>().isOn;
        File_Settings.SaveSettings();
        return;
    }
}
