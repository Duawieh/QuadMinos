using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_ThunderRing : MonoBehaviour
{
    SpriteRenderer SPR;

    // Start is called before the first frame update
    void Start()
    {
        SPR = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        SPR.color += new Color(0, 0, 0, -1 * Time.deltaTime / 0.1f);
        transform.localScale += 7 * Vector3.one * Time.deltaTime / 0.1f;
    }
}
