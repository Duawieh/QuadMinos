using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Anim_MainButtons : MonoBehaviour
{
    public GameObject[] buttons;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(InitAnimation());
    }

    private IEnumerator InitAnimation()
    {
        // 根据屏幕尺寸变换按钮大小
        // 初始化按钮尺寸为按钮高度占屏幕高度 8%
        float buttonsScaleRate = ScaleInScreen.Get_Scale(0.08f, buttons[0]);
        foreach (GameObject button in buttons) {
            button.transform.localScale = new Vector3(buttonsScaleRate, buttonsScaleRate, 1);
        }
        // 计算变换尺寸后的按钮在屏幕上所占高度
        // 初始化按钮位置在屏幕正下方
        float buttonsScreenHeight = ScaleInScreen.Get_Height(buttons[0]);
        Vector3 underScreenCenter = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2.0f, -0.5f * buttonsScreenHeight, 0));
        foreach (GameObject button in buttons) {
            button.transform.position = underScreenCenter;
        }

        float _t = 0.0f;
        
        // 第一阶段动画：一个按钮从屏幕下方进入，到达屏幕中央，动画曲线淡出
        while (_t < 0.5f)
        {
            _t += Time.deltaTime;
            float PosY = Functions.F_paraFadeout(_t, 0.5f, underScreenCenter.y, 0.0f);
            buttons[0].transform.position = new Vector3(0, PosY, 0);
            yield return null;
        }

        // 初始化所有按钮位置为第一个按钮的位置，即屏幕中央
        // 第二阶段动画：其他按钮从上一个按钮下方列出，动画曲线线性
        foreach (GameObject button in buttons)
            button.transform.position = Vector3.zero;
        while (buttons[0].transform.localPosition.y - buttons[4].transform.localPosition.y < buttonsScreenHeight * 4) {
            for (int i = 0; i < 5; i++) {
                if (buttons[i].transform.localPosition.y <= -buttonsScreenHeight * i) {
                    buttons[i].transform.localPosition = new Vector3(0, -buttonsScreenHeight * i, 0);
                    continue;
                }
                buttons[i].transform.localPosition += new Vector3(0, -buttonsScreenHeight * 4 * Time.deltaTime, 0);
            }
            yield return null;
        }

        yield break;
    }

    public IEnumerator ExitAnimation()
    {
        StopAllCoroutines();

        float _t = 0.0f;
        float tgtPosX_Left = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width * -0.5f, 0)).x;
        float tgtPosX_Right = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width * 1.5f, 0)).x;

        GameObject mask = GameObject.Find("ScreenMask");
        GameObject field = GameObject.FindGameObjectWithTag("Field");
        mask.GetComponent<Image>().color = new Color(0, 0, 0, 0);
        mask.GetComponent<Canvas>().sortingOrder = 100;

        while (_t < 0.5f)
        {
            _t += Time.deltaTime;
            for (int i = 0; i < 5; i += 2)
            {
                float posX = Functions.F_paraFadein(_t, 0.5f, 0.0f, tgtPosX_Left);
                buttons[i].transform.position = new Vector3(posX, buttons[i].transform.position.y, 0);
            }
            for (int i = 1; i < 5; i += 2)
            {
                float posX = Functions.F_paraFadein(_t, 0.5f, 0.0f, tgtPosX_Right);
                buttons[i].transform.position = new Vector3(posX, buttons[i].transform.position.y, 0);
            }
            field.GetComponent<AudioSource>().volume -= 2 * Time.deltaTime;
            float _alpha = Functions.F_paraFadein(_t, 0.5f, 0.0f, 1.0f);
            mask.GetComponent<Image>().color = new Color(0, 0, 0, _alpha);
            yield return null;
        }

        Application.Quit();
        yield break;
    }
}
