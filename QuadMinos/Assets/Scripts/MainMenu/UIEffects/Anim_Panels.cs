using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Anim_Panels : MonoBehaviour
{
    private float scale;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(InitAnimation());
    }

    public void ClickButton_Close()
    {
        StartCoroutine(CloseAnimation());
    }

    private IEnumerator InitAnimation()
    {
        scale = ScaleInScreen.Get_Scale(1.0f, gameObject);
        transform.localScale = new Vector3(0, scale, scale);

        float _t = 0.0f;
        while (_t < 0.2f)
        {
            _t += Time.deltaTime;
            float _scaleX = Functions.F_paraFadeout(_t, 0.2f, 0.0f, scale);
            transform.localScale = new Vector3(_scaleX, scale, scale);
            yield return null;
        }

        transform.localScale = new Vector3(scale, scale, scale);
        yield break;
    }

    private IEnumerator CloseAnimation()
    {
        float _t = 0.0f;
        while (_t < 0.2f)
        {
            _t += Time.deltaTime;
            float _scaleX = Functions.F_paraFadein(_t, 0.2f, scale, 0.0f);
            transform.localScale = new Vector3(_scaleX, scale, scale);
            yield return null;
        }
        Destroy(gameObject);
        yield break;
    }

    public IEnumerator StartGameAnimation()
    {
        StartCoroutine(CloseAnimation());

        GameObject mask = GameObject.Find("ScreenMask");
        GameObject field = GameObject.FindGameObjectWithTag("Field");
        mask.GetComponent<Image>().color = new Color(0, 0, 0, 0);
        mask.GetComponent<Canvas>().sortingOrder = 100;

        float _t = 0.0f;
        while (_t < 0.2f)
        {
            _t += Time.deltaTime;
            mask.GetComponent<Image>().color += new Color(0, 0, 0, Time.deltaTime / 0.2f);
            field.GetComponent<AudioSource>().volume -= Time.deltaTime / 0.2f;
            yield return null;
        }

        SceneManager.LoadScene(1);
        yield break;
    }
}
