using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_UIWarning : MonoBehaviour
{
    float t;
    bool _enabled;

    // Start is called before the first frame update
    void Start()
    {
        t = 0.0f;
        _enabled = true;
        GetComponent<AudioSource>().volume = GameSettings.EffectVolume;
    }

    // Update is called once per frame
    void Update()
    {
        t += Time.deltaTime;
        if (!_enabled)
        {
            if (t > 1.0f) Destroy(gameObject);
        }
        return;
    }

    // 警告状态解除，停止警告效果并在一定时间后删除自身
    public void Relive() {
        _enabled = false;
        t = 0.0f;

        // 停止警报声
        GetComponent<AudioSource>().enabled = false;

        // 停止火焰喷射效果
        int i = -1;
        while (++i < transform.childCount)
        {
            GameObject ch = transform.GetChild(i).gameObject;
            ParticleSystem.EmissionModule em = ch.GetComponent<ParticleSystem>().emission;
            em.enabled = false;
        }
        return;
    }
}