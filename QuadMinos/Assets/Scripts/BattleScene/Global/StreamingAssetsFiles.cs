using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering;

/// <summary>
/// 关于 StreamingAssetsFiles 文件的处理
/// </summary>
public class StreamingAssetsFiles : MonoBehaviour
{
    // 将文件从 _frm 复制到 _to
    private IEnumerator Copy(string _frm, string _to, string _name)
    {
        // 若 persistentDataPath 下已包含该文件，返回
        if (File.Exists(_to + _name)) yield break;
        Directory.CreateDirectory(_to);

        UnityWebRequest UWR_file = UnityWebRequest.Get(_frm + _name);
        yield return UWR_file.SendWebRequest();

        if (UWR_file.error != null) yield break;
        if (UWR_file.isDone)
        {
            string fullName = _to + _name;
            FileStream newFile = File.Create(fullName);

            byte[] _data = UWR_file.downloadHandler.data;
            newFile.Write(_data, 0, _data.Length);

            newFile.Flush();
            newFile.Close();
        }

        UWR_file.Dispose();

        yield break;
    }

    /// <summary>
    /// 传入以 StreamingAssets 文件夹为根目录的文件相对路径，将文件复制到 persistentDataPath 下
    /// </summary>
    public void CopyFileToPersistentDataPath(string _pth, string fileName)
    {
        string per_path = Application.persistentDataPath;
        string str_path = Application.streamingAssetsPath;
        StartCoroutine(Copy(str_path + _pth, per_path + _pth, fileName));
        return;
    }
}
