using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Rendering;
using UnityEditor;
using System.Runtime.CompilerServices;


public class S_PlayfieldBackgroundImage : MonoBehaviour
{
    private float ratio;
    private Image comp_image;
    private AspectRatioFitter comp_ARF;
    private Sprite backgroundImage = null;
    private string[] enabledFileTypes = {".jpg", ".png"}; // 受支持的背景图像文件格式

    // Start is called before the first frame update
    void Start()
    {
        InitBackground();
        // 将 Streaming Assets 中的背景图片复制到 persistenceDataPath 中
        StreamingAssetsToPersistenceData.CopyFilesFromSA2PD
            ("/BackgroundImages", "/BackgroundImages", enabledFileTypes, gameObject);
        StartCoroutine(GetBackgroundImage());
        return;
    }

    // 初始化背景为黑色，在背景加载未完成或加载失败时展示为纯黑
    private void InitBackground()
    {
        comp_image = GetComponent<Image>();
        comp_image.color = Color.black;
        return;
    }

    private IEnumerator GetBackgroundImage()
    {
        List<string> filesPath = new List<string>();
        while (filesPath.Count <= 0)
        {
            filesPath = StreamingAssetsToPersistenceData.getFilesNameInFolderByTypes
                (Application.persistentDataPath + "/BackgroundImages", enabledFileTypes);
            yield return null;
        }

        int tgt = Random.Range(0, filesPath.Count);
        string fileFullName = Application.persistentDataPath + "/BackgroundImages/" + filesPath[tgt];

# if UNITY_EDITOR
        using(UnityWebRequest UWR_file = UnityWebRequestTexture.GetTexture(fileFullName))
# elif UNITY_ANDROID
        using(UnityWebRequest UWR_file = UnityWebRequestTexture.GetTexture("file://" + fileFullName))
# endif
        {
            yield return UWR_file.SendWebRequest();
            if (UWR_file.result != UnityWebRequest.Result.Success) yield break;
            Texture2D image = ((DownloadHandlerTexture)UWR_file.downloadHandler).texture;
            backgroundImage = Sprite.Create(image, new Rect(0, 0, image.width, image.height), Vector2.zero);
        }

        // 能运行到此处则可以确保 UWR_file != null
        StartCoroutine(SetImage());
        yield break;
    }

    // 设置背景图像
    private IEnumerator SetImage()
    {
        // 动态调整图像尺寸
        ratio = 1.0f * backgroundImage.texture.width / backgroundImage.texture.height;
        comp_image = GetComponent<Image>();
        comp_ARF = GetComponent<AspectRatioFitter>();

        comp_image.sprite = backgroundImage;
        comp_ARF.aspectRatio = ratio;

        // 设置图像淡入效果
        float _alpha = 0.0f;
        while (_alpha < 0.45f)
        {
            comp_image.color = new Color(1, 1, 1, _alpha);
            _alpha += Time.deltaTime * 0.2f;
            yield return null;
        }

        yield break;
    }
}