using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_FinishUI : MonoBehaviour
{
    private float _scale;

    // Start is called before the first frame update
    void Start()
    {
        _scale = ScaleInScreen.Get_Scale(0.3f, gameObject);
        StartCoroutine(InitAnimation());
    }

    private IEnumerator InitAnimation()
    {
        float _t = 0.0f;
        while (_t < 0.1f)
        {
            _t += Time.deltaTime;
            float scaleY = Functions.F_paraFadeout(_t, 0.1f, 0.0f, _scale);
            transform.localScale = new Vector3(_scale, scaleY, _scale);
            yield return null;
        }
        yield break;
    }
}
