using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class S_PlayfieldBackgroundMusic : MonoBehaviour
{
    public AudioClip backgroundMusic_BLITZ;                         // 闪电战模式专属背景音乐
    private AudioClip backgroundMusic = null;
    private AudioSource ads;
    private string[] enabledFileTypes = { ".wav", ".mp3", ".ogg" }; // 受支持的背景音乐文件格式

    // Start is called before the first frame update
    void Start()
    {
        ads = GetComponent<AudioSource>();
        StreamingAssetsToPersistenceData.CopyFilesFromSA2PD
            ("/BattleMusic", "/BattleMusic", enabledFileTypes, gameObject);
        StartCoroutine(GetBackgroundMusic());
        return;
    }

    private IEnumerator GetBackgroundMusic()
    {
        // 若游戏模式为 BLITZ，音乐固定为 《神游行列间》
        if (BattleInfo.GameMode == 3) {
            backgroundMusic = backgroundMusic_BLITZ;
            yield break;
        }

        FolderFilesNameInfo files = new();
        while (files.filesInfoList.Count <= 0)
        {
            files = StreamingAssetsToPersistenceData.GetFilesNameInFolderByTypes
                (Application.persistentDataPath + "/BattleMusic", enabledFileTypes);
            yield return null;
        }

        // 随机产生本局音乐
        int tgt;
        tgt = Random.Range(0, files.filesInfoList.Count);
        string fileName = "file://" + Application.persistentDataPath + "/BattleMusic/" + files.filesInfoList[tgt].fileName;
        AudioType audioType = AudioType.MPEG;
        if (files.filesInfoList[tgt].fileType == ".mp3") audioType = AudioType.MPEG;
        if (files.filesInfoList[tgt].fileType == ".wav") audioType = AudioType.WAV;
        if (files.filesInfoList[tgt].fileType == ".ogg") audioType = AudioType.OGGVORBIS;

        using(UnityWebRequest UWR_file = UnityWebRequestMultimedia.GetAudioClip(fileName, audioType))
        {
            yield return UWR_file.SendWebRequest();
            if (UWR_file.result != UnityWebRequest.Result.Success) yield break;
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
            yield return null;
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
