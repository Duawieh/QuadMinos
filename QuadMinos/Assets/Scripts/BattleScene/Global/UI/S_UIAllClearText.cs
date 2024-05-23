using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class S_UIAllClearText : MonoBehaviour
{
    public GameObject txt;         // 文字特效
    public GameObject flr;         // 背光特效

    float t;

    // Start is called before the first frame update
    void Start()
    {
        t = 0.0f;
        flr.transform.localScale = new Vector3(0, 1024, 1);
    }

    // Update is called once per frame
    void Update()
    {
        t += Time.deltaTime;
        if (t < 0.1f)
        {
            txt.transform.localEulerAngles += new Vector3(0, 0, Random.Range(0, 136));
            flr.transform.localScale += new Vector3(20480 * Time.deltaTime, 0, 0);
        }
        else if (t < 2.1f)
        {
            txt.transform.localEulerAngles = Vector3.zero;
            flr.transform.localScale = new Vector3(2048, 1024, 1);
            txt.transform.localScale += new Vector3(0.1f * Time.deltaTime, 0, 0);
            flr.GetComponent<SpriteRenderer>().color = new Color(1, 0.8039216f, 0, Functions.F_paraFadein(t - 0.1f, 2.0f, 1, 0));
            txt.GetComponent<Text>().color = new Color(1, 1, 1, Functions.F_paraFadein(t - 0.1f, 2.0f, 1, 0));
        }
        else Destroy(gameObject);
    }
}
