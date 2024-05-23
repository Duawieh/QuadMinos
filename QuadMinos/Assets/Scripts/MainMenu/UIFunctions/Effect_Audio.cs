using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_Audio : MonoBehaviour
{
    public void PlayAudio(AudioClip _clip, float _volume, float _time)
    {
        GameObject audioPlayer = new GameObject("AudioPlayer");
        AudioSource ads = audioPlayer.AddComponent<AudioSource>();
        ads.volume = _volume * GameSettings.EffectVolume;
        ads.clip = _clip;
        ads.Play();
        S_DestroyTimer bomb = audioPlayer.AddComponent<S_DestroyTimer>();
        bomb.T = _time;
        return;
    }
}
