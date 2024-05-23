using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class S_UIBackToBack : MonoBehaviour
{
    private float t;
    private Text T;

    private bool B2B;               //  «∑Òœ‘ æ B2B

    // Start is called before the first frame update
    void Start()
    {
        B2B = false;
        T = GetComponent<Text>();
        transform.localScale = new Vector3(2, 0, 1);
        t = 1000f;
    }

    // Update is called once per frame
    void Update()
    {
        if (B2B)
        {
            if (t <= 0.1f)
            {
                float v = Time.deltaTime / 0.1f;
                transform.localScale += new Vector3(-v, v, 0);
                float _alpha = Functions.F_paraFadeout(t, 0.1f, 0, 1);
                T.color = new Color(1, 0.8039216f, 0, _alpha);
            } 
            else
            {
                T.color = new Color(1, 0.8039216f, 0, 1);
                transform.localScale = Vector3.one;
            }
        } 
        else
        {
            if (t > 1.0f && t < 1.5f)
            {
                float _alpha = Functions.F_paraFadeinout(t - 1, 0.5f, 1, 0);
                T.color = new Color(1, 0, 0, _alpha);
            } 
            else if (t > 1.5f)
            {
                T.color = new Color(1, 0, 0, 0);
            }
        }
        t += Time.deltaTime;
    }

    public void Init(int B2B_times)
    {
        if (B2B_times <= 1) return;
        // …Ë÷√œ‘ æ◊÷∑˚¥Æ
        if (B2B_times == 2) T.text = "B2B";
        else T.text = "B2B °¡ " + (B2B_times - 1);

        transform.localScale = new Vector3(2, 0, 1);
        T.color = new Color(1, 0.8039216f, 0, 0);
        B2B = true;
        t = 0.0f;
        return;
    }

    public void Finish()
    {
        if (!B2B) return;
        T.text = "B2B °¡ 0";

        transform.localScale = Vector3.one;
        T.color = new Color(1, 0, 0, 1);
        B2B = false;
        t = 0.0f;
        return;
    }
}
