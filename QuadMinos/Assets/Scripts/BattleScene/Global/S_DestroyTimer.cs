using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_DestroyTimer : MonoBehaviour
{
    public float T;
    private float t;

    // Start is called before the first frame update
    void Start()
    {
        t = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        t += Time.deltaTime;
        if (t >= T) Destroy(gameObject);
    }
}
