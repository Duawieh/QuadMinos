using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class S_GameAlertUI : MonoBehaviour
{
    public GameObject son_text;
    private float _t = 0.0f;
    private float posY_begin, posY_end, _scale;
    private Color originColor;

    // Start is called before the first frame update
    void Start()
    {
        _scale = ScaleInScreen.Get_Scale(0.1f, gameObject);
        posY_begin = ScaleInScreen.Get_PosY(0.15f, _scale, gameObject);
        posY_end = ScaleInScreen.Get_PosY(0.20f, _scale, gameObject);
        originColor = GetComponent<Image>().color;
        GetComponent<Image>().color = new Color(originColor.r, originColor.g, originColor.b, 0);
        son_text.GetComponent<Text>().color = new Color(1, 1, 1, 0);
        transform.localScale = new Vector3(_scale, _scale, _scale);
    }

    // Update is called once per frame
    void Update()
    {
        _t += Time.deltaTime;
        if (_t < 0.2f)
        {
            GetComponent<Image>().color += new Color(0, 0, 0, Time.deltaTime / 0.2f);
            son_text.GetComponent<Text>().color += new Color(0, 0, 0, Time.deltaTime / 0.2f);
            float posY = Functions.F_paraFadeout(_t, 0.2f, posY_begin, posY_end);
            transform.position = new Vector3(0, posY, 0);
        }
        else if (_t < 2.8f)
        {
            GetComponent<Image>().color = originColor;
            son_text.GetComponent<Text>().color = Color.white;
            return;
        }
        else if (_t < 3.0f)
        {
            GetComponent<Image>().color -= new Color(0, 0, 0, Time.deltaTime / 0.2f);
            son_text.GetComponent<Text>().color -= new Color(0, 0, 0, Time.deltaTime / 0.2f);
            float posY = Functions.F_paraFadein(_t - 2.8f, 0.2f, posY_end, posY_begin);
            transform.position = new Vector3(0, posY, 0);
        }
        else Destroy(gameObject);
        return;
    }

    public void SetText(string text)
    {
        son_text.GetComponent<Text>().text = text;
        return;
    }
}
