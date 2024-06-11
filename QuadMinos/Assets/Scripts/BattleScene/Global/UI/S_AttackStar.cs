using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_AttackStar : MonoBehaviour
{
    public AudioClip attackSound;
    private GameObject field;

    private IEnumerator Move(Vector3 _from, Vector3 _to)
    {
        float _t = 0.0f;
        float _x = _from.x;
        float _y = _from.y;
        bool flg = Random.Range(0, 2) == 1;     // һ����ǣ����ڿ����ƶ�·������͹�����°�
        while (_t < 0.2f)
        {
            _t += Time.deltaTime;
            if (flg)
            {
                _x = Functions.F_paraFadeout(_t, 0.2f, _from.x, _to.x);
                _y = Functions.F_paraFadeinout(_t, 0.2f, _from.y, _to.y);
            }
            else
            {
                _x = Functions.F_paraFadeinout(_t, 0.2f, _from.x, _to.x);
                _y = Functions.F_paraFadeout(_t, 0.2f, _from.y, _to.y);
            }
            transform.position = new Vector2(_x, _y);
            yield return null;
        }
        yield break;
    }

    // 玩家产生伤害时专用
    public void Init(ref GameObject mino, Vector3 tgt)
    {
        field = GameObject.FindGameObjectWithTag("Field");
        ParticleSystem.MainModule m_main = GetComponent<ParticleSystem>().main;
        m_main.startColor = MinoColors.minoColor[mino.GetComponent<Mino_Active>().MinoType];
        StartCoroutine(Move(mino.transform.position, tgt));
        field.GetComponent<S_AudioEffect>().PlayAudio(attackSound, 1.0f, 0.5f);
        return;
    }

    // 敌人产生伤害时专用
    public void Init(Vector3 frm, Vector3 tgt)
    {
        field = GameObject.FindGameObjectWithTag("Field");
        ParticleSystem.MainModule m_main = GetComponent<ParticleSystem>().main;
        m_main.startColor = Color.red;
        StartCoroutine(Move(frm, tgt));
        field.GetComponent<S_AudioEffect>().PlayAudio(attackSound, 1.0f, 0.5f);
        return;
    }
}
