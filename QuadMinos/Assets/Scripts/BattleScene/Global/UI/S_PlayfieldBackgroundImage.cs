using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Rendering;
using UnityEditor;

public class S_PlayfieldBackgroundImage : MonoBehaviour
{
    private float ratio;
    private Image comp_image;
    private AspectRatioFitter comp_ARF;
    private Sprite backgroundImage = null;

    // Start is called before the first frame update
    void Start()
    {
        InitBackground();
        CopyDefaultBackgroundImageFiles(11);
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

    // 将 StreamingAssets 文件夹内的 _tot 个文件复制到 PersistentData
    private void CopyDefaultBackgroundImageFiles(int _tot)
    {
        for (int i = 0; i < _tot; i++)
        {
            string _path = "/BackgroundImages/";
            string _name = "bkg_" + i.ToString() + ".jpg";
            GetComponent<StreamingAssetsFiles>().CopyFileToPersistentDataPath(_path, _name);
        }
    }

    private IEnumerator GetBackgroundImage()
    {
        FileInfo tgtFile = null;
        while (tgtFile == null)
        {
            tgtFile = GetTargetFileInfo(Application.persistentDataPath + "/BackgroundImages/");
            yield return null;
        }

        using(UnityWebRequest UWR_file = UnityWebRequestTexture.GetTexture("file://" + tgtFile.FullName))
        {
            yield return UWR_file.SendWebRequest();
            Texture2D texture = DownloadHandlerTexture.GetContent(UWR_file);
            backgroundImage = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
        }

        StartCoroutine(SetImage());

        yield break;
    }

    private FileInfo GetTargetFileInfo(string path)
    {
        DirectoryInfo dirInfo = new DirectoryInfo(Application.persistentDataPath + "/BackgroundImages");
        FileInfo[] filesInfo = dirInfo.GetFiles("*", SearchOption.AllDirectories);

        if (filesInfo.Length == 0) return null;

        int tgt = Random.Range(0, filesInfo.Length);
        for (int i = 0, j = 0; i <= filesInfo.Length; i++)
        {
            if (i == filesInfo.Length) i = 0;   // 为防止越界访问，环形遍历
            if (filesInfo[i].Name.EndsWith(".meta")) continue;
            if (j == tgt)
            {
                return filesInfo[i];
            }
            j++;
        }

        return null;
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
        while (_alpha < 0.65f)
        {
            comp_image.color = new Color(1, 1, 1, _alpha);
            _alpha += Time.deltaTime * 0.2f;
            yield return null;
        }

        yield break;
    }
}