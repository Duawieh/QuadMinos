using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Anim_MinosRainDrop : MonoBehaviour
{
    int sgn;

    void Start()
    {
        sgn = (int)(10000 * Time.deltaTime) & 1;
        if (sgn == 0) sgn = -1;
    }

    // Update is called once per frame
    void Update()
    {
        if (Camera.main.WorldToScreenPoint(transform.position).y < -Screen.height / 5.0f)
        {
            Destroy(gameObject);
            return;
        }

        float _v = -Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.height)).y * 2.0f;
        transform.position += new Vector3(0, _v * Time.deltaTime, 0);
        transform.localEulerAngles += new Vector3(0, 0, sgn * 30 * Time.deltaTime);

        return;
    }
}
