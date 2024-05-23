using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_AudioEffect : MonoBehaviour
{
    public void PlayAudio(AudioClip clip, float volume, float T)
    {
        GameObject audioPlayer = new GameObject("AudioPlayer");
        AudioSource ads = audioPlayer.AddComponent<AudioSource>();
        ads.volume = volume * GameSettings.EffectVolume;
        ads.clip = clip;
        ads.Play();
        S_DestroyTimer bomb = audioPlayer.AddComponent<S_DestroyTimer>();
        bomb.T = T;
        return;
    }
}
