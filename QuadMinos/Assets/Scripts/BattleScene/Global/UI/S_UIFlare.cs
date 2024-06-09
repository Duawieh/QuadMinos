using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class S_UIFlare : MonoBehaviour
{
    private Image img;              // 图像组件
    private float M;                // 最大亮度 (0, 1]
    private float t;

    // Start is called before the first frame update
    void Start()
    {
        transform.localScale = new Vector3(3, 1, 1);
    }

    // Update is called once per frame
    void Update()
    {
        t += Time.deltaTime;
        img.color = new Color(img.color.r, img.color.g, img.color.b, Functions.F_paraBounce(t, 2.0f, 0.1f, 0, M));
        if (t >= 2.0f) Destroy(gameObject);
    }

    public void Init(Color _c, float _M, Vector3 _p)
    {
        img = GetComponent<Image>();
        img.color = new Color(_c.r, _c.g, _c.b, 0);
        transform.position = _p;
        M = _M;
        t = 0.0f;
        return;
    }
}
