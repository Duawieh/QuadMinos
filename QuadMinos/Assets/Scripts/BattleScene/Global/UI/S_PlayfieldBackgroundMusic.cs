using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class S_PlayfieldBackgroundMusic : MonoBehaviour
{
    private AudioClip backgroundMusic = null;
    private AudioSource ads;

    // Start is called before the first frame update
    void Start()
    {
        ads = GetComponent<AudioSource>();

        CopyDefaultBackgroundMusicFiles(4);
        StartCoroutine(GetBackgroundMusic());
    }

    // 将 StreamingAssets 文件夹内的 _tot 个文件复制到 PersistentData
    private void CopyDefaultBackgroundMusicFiles(int _tot)
    {
        for (int i = 0; i < _tot; i++)
        {
            string _path = "/BattleMusic/";
            string _name = "bm_" + i.ToString() + ".mp3";
            GetComponent<StreamingAssetsFiles>().CopyFileToPersistentDataPath(_path, _name);
        }
        return;
    }

    private IEnumerator GetBackgroundMusic()
    {
        string[] filesPath = new string[0];
        while (filesPath.Length <= 0)
        {
            filesPath = Directory.GetFiles(Application.persistentDataPath + "/BattleMusic", "*.mp3");
            yield return 0;
        }

        int tgt = Random.Range(0, filesPath.Length);

        using(UnityWebRequest UWR_file = UnityWebRequestMultimedia.GetAudioClip("file://" + filesPath[tgt], AudioType.MPEG))
        {
            yield return UWR_file.SendWebRequest();
            backgroundMusic = DownloadHandlerAudioClip.GetContent(UWR_file);
        }

        yield break;
    }

    public IEnumerator AudioPlay()
    {
        while (ads.clip == null) {
            if (ads.enabled)
            {
                ads.clip = backgroundMusic;
                ads.volume = GameSettings.MusicVolume;
                ads.loop = true;
                ads.Play();
            }
            yield return 0;
        }
        yield break;
    }

    public IEnumerator AudioStop()
    {
        while (ads.volume > 0)
        {
            ads.volume -= Time.deltaTime;
            yield return null;
        }
        yield break;
    }
}
