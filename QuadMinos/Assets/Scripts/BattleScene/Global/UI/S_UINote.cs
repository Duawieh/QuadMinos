using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class S_UINote : MonoBehaviour
{
    public GameObject UI_Flare;

    private float vecX;
    private float vecY;

    private float T;
    private bool flashEphasis = false;
    private Color originColor = Color.white;

    // Start is called before the first frame update
    void Start()
    {
        T = 0.0f;
        vecX = Random.Range(-0.5f, 0.5f);
        vecY = Mathf.Sqrt(1 - vecX * vecX);
    }

    // Update is called once per frame
    void Update()
    {
        if (flashEphasis) FlashEmphasis();
        else
        {
            Vector3 v = new Vector3(vecX, vecY, 0) * 0.05f * Screen.height;
            transform.localPosition += v * Time.deltaTime;

            GetComponent<Text>().color += new Color(0, 0, 0, -Time.deltaTime);
        }
    }

    private void FlashEmphasis()
    {
        T += Time.deltaTime;

        Text _T = GetComponent<Text>();
        Image _img = UI_Flare.GetComponent<Image>();
        if (T < 0.03f)
        {
            _T.color = Color.black;
            _img.color = Color.white;
        } 
        else if (T < 0.06f)
        {
            _T.color = Color.white;
            _img.color = Color.black;
        }
        else if (T < 0.09f)
        {
            _T.color = Color.black;
            _img.color = Color.white;
        }
        else if (T < 0.15f)
        {
            _T.color = originColor;
            _img.color = Color.white;
            transform.parent.GetComponent<S_DestroyTimer>().T = 2.15f;
        } 
        else
        {
            Vector3 v = new Vector3(vecX, vecY, 0) * 0.05f * Screen.height;
            transform.localPosition += v * Time.deltaTime;

            float text_a = Functions.F_paraFadeout(T - 0.15f, 2.0f, 1.0f, 0.0f);
            float flare_a = Functions.F_paraFadeout(T - 0.15f, 1.2f, 1.0f, 0.0f);
            if (T - 0.15f > 1.2f) flare_a = 0;
            GetComponent<Text>().color = Vector4.Scale(originColor, new Vector4(1, 1, 1, 0)) + new Vector4(0, 0, 0, text_a);
            UI_Flare.GetComponent<Image>().color = new Color(1, 1, 1, flare_a);
        }
        return;
    }

    private IEnumerator NoteSize() {
        float t = 0.0f;
        float initialSize = 1.0f;
        float targetSize = 0.30f;

        while (t < 0.5f) {
            float curSize = Functions.F_paraFadeout(t, 0.5f, initialSize, targetSize);
            transform.parent.localScale = new Vector3(curSize, curSize, curSize);
            t += Time.deltaTime;
            yield return null;
        }

        transform.parent.localScale = new Vector3(targetSize, targetSize, targetSize);
        yield break;
    }

    public void Init(string _text, Color _color, int _size, bool _flash)
    {
        Text _T = GetComponent<Text>();
        _T.text = _text;
        _T.color = _color;
        _T.fontSize = _size;
        flashEphasis = _flash;
        // 记录设定的颜色，用于在闪烁强调后恢复色彩
        originColor = _color;
        // 默认设置闪光关闭
        UI_Flare.GetComponent<Image>().color = new Color(0, 0, 0, 0);

        if (_size >= 128) StartCoroutine(nameof(NoteSize));
        return;
    }
}
