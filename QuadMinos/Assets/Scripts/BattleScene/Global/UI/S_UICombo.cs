using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class S_UICombo : MonoBehaviour
{
    private Text T;
    private float t;

    // Start is called before the first frame update
    void Start()
    {
        T = GetComponent<Text>();
        transform.localScale = new Vector3(2, 0, 1);
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
            T.color = new Color(T.color.r, T.color.g, T.color.b, Functions.F_paraBounce(t, 2.0f, 0.1f, 0, 1));
        }
        else Start();
        t += Time.deltaTime;
    }

    public void Init(int combos)
    {
        if (combos <= 1) return;
        transform.localScale = new Vector3(2, 0, 1);
        T.color = new Color(1, 1, 1, 0);
        T.text = combos + " Combo";
        t = 0.0f;
        return;
    }
}
