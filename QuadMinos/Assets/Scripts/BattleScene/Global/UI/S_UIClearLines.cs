using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Rendering;

public class S_UIClearLines : MonoBehaviour
{
    private float t;
    private Text T;

    private string[] linesString = new string[] { "MINI", "SINGLE", "DOUBLE", "TRIPLE", "QUAD" };

    // Start is called before the first frame update
    void Start()
    {
        T = GetComponent<Text>();
        T.color = new Color(0, 0, 0, 0);
        T.text = "";
        transform.localScale = new Vector3(2, 0, 0);
        t = 1000f;
    }

    // Update is called once per frame
    void Update()
    {
        if (t >= 0 && t <= 2.0f)
        {
            if (t < 0.1f)
            {
                float v = Time.deltaTime / 0.1f;
                transform.localScale += new Vector3(-v, v, 0);
            }
            else
            {
                float scl = Functions.F_paraFadeout(t, 2, 1, 1.27f);
                transform.localScale = new Vector3(scl, scl, 1);
            }
            T.color = new Color(T.color.r, T.color.g, T.color.b, Functions.F_paraBounce(t, 2.0f, 0.1f, 0, 1));
        }
        else Start();
        t += Time.deltaTime;
    }

    public void Init(int lines)
    {
        transform.localScale = new Vector3(2, 0, 1);
        T.color = new Color(1, 1, 1, 0);
        T.text = linesString[lines];
        t = 0.0f;
        return;
    }
}
