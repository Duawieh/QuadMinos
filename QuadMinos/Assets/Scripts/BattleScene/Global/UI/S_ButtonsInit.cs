using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_ButtonsInit : MonoBehaviour
{
    public GameObject ButtonsLeft;
    public GameObject ButtonsRight;

    private void ButtonsPositonInit()
    {
        int W = Screen.width;
        int H = Screen.height;
        const int ButtonsWidth = 250;

        float ScaleRatio = (H * 0.5f) / ButtonsWidth;
        ButtonsLeft.transform.localScale = new Vector3(ScaleRatio, ScaleRatio, ScaleRatio);
        ButtonsRight.transform.localScale = new Vector3(ScaleRatio, ScaleRatio, ScaleRatio);

        ButtonsLeft.GetComponent<RectTransform>().anchoredPosition = new Vector2(W * 0.1f + ButtonsWidth * 0.3f * ScaleRatio, H * 0.25f + ButtonsWidth * 0.25f * ScaleRatio);
        ButtonsRight.GetComponent<RectTransform>().anchoredPosition = new Vector2(W * -0.15f - ButtonsWidth * 0.33f * ScaleRatio, H * 0.2f + ButtonsWidth * 0.5f * ScaleRatio);

        return;
    }

    // Start is called before the first frame update
    void Start()
    {
        transform.localScale = Vector3.one;
        GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);
        GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);
        ButtonsPositonInit();
    }
}
