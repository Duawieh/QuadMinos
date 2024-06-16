using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Anim_MainMenu : MonoBehaviour
{
    public AudioClip clear;
    public GameObject buttons;
    public GameObject[] minos;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(WaitForBegin());
    }

    private void MinosRainGenerate()
    {
        GameObject _drop = Instantiate(minos[Random.Range(0, 7)]);
        _drop.transform.localScale = Vector3.one;
        _drop.transform.localEulerAngles = new Vector3(0, 0, Random.Range(0.0f, 360.0f));
        _drop.transform.position = Camera.main.ScreenToWorldPoint(new Vector2(Random.Range(0.0f, Screen.width), Screen.height * 1.2f));
        _drop.transform.position = new Vector3(_drop.transform.position.x, _drop.transform.transform.position.y, 0);
        _drop.transform.parent = transform.parent;
        _drop.GetComponent<Mino_Active>().enabled = false;
        _drop.GetComponent<Mino_Kick>().enabled = false;
        _drop.AddComponent<Anim_MinosRainDrop>();
        return;
    }

    private IEnumerator WaitForBegin()
    {
        // 游戏开始前首先初始化设置信息
        StartCoroutine(File_Settings.GetSettings());

        while (true)
        {
            if (Time.deltaTime * 24.0f < 1.0f)
            {
                StartCoroutine(Anim_TitleBegin());
                yield break;
            }
            yield return null;
        }
    }

    private IEnumerator Anim_TitleBegin()
    {
        GetComponent<SpriteRenderer>().enabled = true;
        float _t = 0.0f;

        float _tgtScale = ScaleInScreen.Get_Scale(0.35f, gameObject);
        float _tgtPosY = ScaleInScreen.Get_PosY(0.05f, _tgtScale, gameObject);
        float _bgnScale = transform.localScale.x;
        while (_t <= 0.5f)
        {
            _t += Time.deltaTime;
            float _s = Functions.F_paraFadein(_t, 0.5f, _bgnScale, _tgtScale);
            float _y = Functions.F_paraFadein(_t, 0.5f, 0.0f, _tgtPosY);
            float _a = Functions.F_paraFadein(_t, 0.5f, 0.0f, 1.0f);
            transform.localScale = new Vector3(_s, _s, _s);
            transform.localPosition = new Vector3(0, _y, 0);
            GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, _a);
            yield return null;
        }
        transform.localScale = new Vector3(_tgtScale, _tgtScale, _tgtScale);
        transform.localPosition = new Vector3(0, _tgtPosY, 0);
        GetComponent<SpriteRenderer>().color = Color.white;

        GameObject buttonBox = Instantiate(buttons, transform.parent);
        buttonBox.transform.localScale = Vector3.one;
        buttonBox.transform.position = Vector3.zero;
        GetComponent<Effect_Audio>().PlayAudio(clear, 1.0f, 1.0f);
        GetComponent<AudioSource>().volume = GameSettings.MusicVolume;
        GetComponent<AudioSource>().Play();
        StartCoroutine(GenerateMinosRain());

        _t = 0.0f;
        Vector3 posCenter = transform.localPosition;
        float _posFlag = transform.localPosition.y;
        while (_t <= 0.5f)
        {
            _t += Time.deltaTime;
            float _deltaPos = Functions.F_triBounce(_t, 0.5f, 0.0f, _posFlag * 0.1f, 5);
            transform.localPosition = posCenter + new Vector3(_deltaPos, _deltaPos, 0);
            yield return null;
        }
        transform.localPosition = posCenter;

        yield break;
    }

    private IEnumerator GenerateMinosRain()
    {
        int _velo = 64;
        float _wait = 0.15f;
        while (true)
        {
            for (int i = 1; i <= _velo; i++) MinosRainGenerate();
            // 如果帧率过低，加快生成量衰减的速率，减少 mino 的数量
            if (Time.deltaTime * 30 > 1) _velo >>= 1;
            _velo >>= 1; if (_velo < 1) _velo = 1;
            _wait += Time.deltaTime;
            if (_wait > 0.5f) _wait = 0.5f;
            yield return new WaitForSeconds(_wait);
        }
    }
}
