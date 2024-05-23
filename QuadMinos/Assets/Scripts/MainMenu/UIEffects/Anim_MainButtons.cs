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
        Vector3 UnderScreenCenter = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2.0f, -0.2f * Screen.height, 0));
        foreach (GameObject button in buttons)
            button.transform.position = UnderScreenCenter;

        float _t = 0.0f;

        while (_t < 0.5f)
        {
            _t += Time.deltaTime;
            float PosY = Functions.F_paraFadeout(_t, 0.5f, UnderScreenCenter.y, 0.0f);
            buttons[0].transform.position = new Vector3(0, PosY, 0);
            yield return null;
        }

        buttons[0].transform.position = Vector3.zero;
        foreach (GameObject button in buttons)
            button.transform.position = buttons[0].transform.position;

        while (buttons[0].transform.localPosition.y - buttons[4].transform.localPosition.y < 96 * 4)
        {
            for (int i = 0; i < 5; i++)
            {
                if (buttons[i].transform.localPosition.y <= -96 * i)
                {
                    buttons[i].transform.localPosition = new Vector3(0, -96 * i, 0);
                    continue;
                }

                buttons[i].transform.localPosition += new Vector3(0, -96 * 4 * Time.deltaTime, 0);
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
