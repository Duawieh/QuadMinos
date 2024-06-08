using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering;


/// <summary>
/// 文件操作协程及其调用函数
/// </summary>
public class FilesCoroutineOperations : MonoBehaviour {
    /// <summary>
    /// 传入以 StreamingAssets 文件夹为根目录的文件相对路径，将文件复制到 persistentDataPath 下
    /// </summary>
    public void StartCoroutine_Copy(string _SApath, string _PDpath, string fileName)
    {
        StartCoroutine(StreamingAssetsToPersistenceData.Copy(_SApath, _PDpath, fileName, this));
        return;
    }
}


/// <summary>
/// 与 StreamingAssets 和 PersistenceData 文件夹内文件操作相关的函数类
/// </summary>
/// <notes>
/// SA 缩写表示 StreamingAssets
/// PD 缩写表示 persistenceData
/// </notes>
public class StreamingAssetsToPersistenceData : MonoBehaviour {
    /// <summary>
    /// 获取文件夹下的所有文件名（相当于 dir 命令）
    /// </summary>
    /// <param name="_folderPath">给定的路径</param>
    /// <returns>给定文件夹下的所有文件的文件名，不包含路径下的文件夹名及其下文件</returns>
    public static List<string> getFilesNameInFolder(string _folderPath) {
        List<string> filesName = new();
        if (!Directory.Exists(_folderPath)) return new List<string>();
        string[] files = Directory.GetFiles(_folderPath);

        // 直接取出所有文件的不含路径的文件名
        foreach (string f in files) { filesName.Add(Path.GetFileName(f)); }
        return filesName;
    }

    /// <summary>
    /// 获取文件夹下的所有符合类型要求的文件名
    /// </summary>
    /// <param name="_folderPath">给定的路径</param>
    /// <param name="_fileType">给定的类型表</param>
    /// <returns>给定文件夹下的所有符合类型要求的文件的文件名，不包含路径下的文件夹名及其下文件</returns>
    public static List<string> getFilesNameInFolderByTypes(string _folderPath, string[] _fileType) {
        List<string> filesName = new();
        if (!Directory.Exists(_folderPath)) return new List<string>();
        string[] files = Directory.GetFiles(_folderPath);

        // 取出所有格式受支持的不含路径的文件名
        foreach (string f in files) 
            foreach (string t in _fileType) 
                if (f.EndsWith(t)) filesName.Add(Path.GetFileName(f)); 
        return filesName;
    }

    /// <summary>
    /// 将 StreamingAssets 文件夹内的所有受支持的文件复制到 PersistentData，由于 StartCoroutine() 需要一个实例化的对象来调用，因此必须传入一个 handle
    /// </summary>
    /// <param name="_SApath">在 streamingAssets 文件夹内的相对路径</param>
    /// <param name="_PDpath">在 persistenceData 文件夹内的相对路径</param>
    /// <param name="_handleObj">负责开启复制操作协程的对象</param>
    /// <param name="_fileType">所有受支持的文件格式，以字符 . 开头</param>
    public static void CopyFilesFromSA2PD(string _SApath, string _PDpath, string[] _fileType, GameObject _handleObj)
    {
        _SApath = Application.streamingAssetsPath + _SApath;
        _PDpath = Application. persistentDataPath + _PDpath;
        List<string> filesName = getFilesNameInFolder(_SApath);

        // 排除不支持的文件，并将受支持的文件复制到 PD 内
        foreach (string fileName in filesName) {
            bool legalFile = false;
            foreach (string type in _fileType) {
                if (fileName.EndsWith(type)) {
                    legalFile = true;
                    break;
                }
            }
            if (!legalFile) continue;
            _handleObj.AddComponent<FilesCoroutineOperations>()
                .StartCoroutine_Copy(_SApath, _PDpath, fileName);
        }

        return;
    }

    // 异步将文件从 _frm 复制到 _to
    public static IEnumerator Copy(string _frm, string _to, string _name, FilesCoroutineOperations _handleObj)
    {
        // 若 persistentDataPath 下已包含该文件，返回
        if (File.Exists(_to + "/" + _name)) {
            Destroy(_handleObj);
            yield break;
        }
        Directory.CreateDirectory(_to);

        UnityWebRequest UWR_file = UnityWebRequest.Get(_frm + "/" + _name);
        yield return UWR_file.SendWebRequest();

        if (UWR_file.error != null) {
            Destroy(_handleObj);
            yield break;
        }
        if (UWR_file.isDone)
        {
            string fullName = _to + "/" + _name;
            FileStream newFile = File.Create(fullName);

            byte[] _data = UWR_file.downloadHandler.data;
            newFile.Write(_data, 0, _data.Length);

            newFile.Flush();
            newFile.Close();
        }

        Destroy(_handleObj);
        UWR_file.Dispose();

        yield break;
    }
}
