using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering;
using UnityEngine.UIElements.Experimental;


[Serializable]
public class FileNameInfo {
    public string fileName;
    public string fileType;
}

[Serializable]
public class FolderFilesNameInfo {
    public List<FileNameInfo> filesInfoList;

    public FolderFilesNameInfo() {
        filesInfoList = new();
    }
}


/// <summary>
/// 文件操作协程及其调用函数
/// </summary>
public class FilesCoroutineOperations : MonoBehaviour {
    public FolderFilesNameInfo saff;

    /// <summary>
    /// 传入以 StreamingAssets 文件夹为根目录的文件相对路径，将文件复制到 persistentDataPath 下
    /// </summary>
    public void StartCoroutine_Copy(string _SApath, string _PDpath, string fileName)
    {
        StartCoroutine(StreamingAssetsToPersistenceData.Copy(_SApath, _PDpath, fileName, this));
        return;
    }

    public void StartCoroutine_GetJson(string _SApath) {
        StartCoroutine(StreamingAssetsToPersistenceData.GetFolderFilesInfoFromJson(_SApath, this));
        return;
    }

    public void StartCoroutine_FilterAndCopy(string _SApath, string _PDpath, string[] _fileType) {
        StartCoroutine(StreamingAssetsToPersistenceData.FilterAndCopyFiles(_SApath, _PDpath, _fileType, this));
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
    /// <remarks>
    /// Android 平台下不可用于 StreamingAssets 目录！
    /// </remarks>
    /// <param name="_folderPath">给定的路径</param>
    /// <returns>给定文件夹下的所有文件的文件信息，不包含路径下的文件夹名及其下文件</returns>
    public static FolderFilesNameInfo GetFilesInfoInFolder(string _folderPath) {
        FolderFilesNameInfo filesInfo = new();
        if (!Directory.Exists(_folderPath)) return new();
        string[] files = Directory.GetFiles(_folderPath);

        foreach (string f in files) {
            FileNameInfo safi = new()
            {
                fileName = Path.GetFileName(f),
                fileType = Path.GetExtension(f)
            };
            filesInfo.filesInfoList.Add(safi); 
        }
        return filesInfo;
    }

    /// <summary>
    /// 获取文件夹下的所有符合类型要求的文件名
    /// </summary>
    /// <remarks>
    /// Android 平台下不可用于 StreamingAssets 目录！
    /// </remarks>
    /// <param name="_folderPath">给定的路径</param>
    /// <param name="_fileType">给定的类型表</param>
    /// <returns>给定文件夹下的所有符合类型要求的文件的文件名，不包含路径下的文件夹名及其下文件</returns>
    public static FolderFilesNameInfo GetFilesNameInFolderByTypes(string _folderPath, string[] _fileType) {
        if (!Directory.Exists(_folderPath)) return new();
        string[] filesName = Directory.GetFiles(_folderPath);

        FolderFilesNameInfo files = new();

        // 取出所有格式受支持的不含路径的文件名
        foreach (string f in filesName) {
            foreach (string t in _fileType) {
                FileNameInfo file = new();
                file.fileName = Path.GetFileName(f);
                file.fileType = Path.GetExtension(f);
                if (file.fileType == t) {
                    files.filesInfoList.Add(file); 
                    break;
                }
            }
        }

        return files;
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
        FolderFilesNameInfo filesInfo;

# if UNITY_EDITOR
        // 若非 Android 平台，直接使用 Directory.GetFiles() 方法获取即可
        filesInfo = GetFilesInfoInFolder(_SApath);
        CreateFileListJson(ref filesInfo, _SApath);
# endif

        // 若在 Android 平台下，不能在 StreamingAssets 下使用 Directory 类
        // 使用 UnityWebRequest 获取 json 文件来获知文件夹下的所有文件信息
        FilesCoroutineOperations fco = _handleObj.AddComponent<FilesCoroutineOperations>();
        fco.StartCoroutine_GetJson(_SApath);
        fco.StartCoroutine_FilterAndCopy(_SApath, _PDpath, _fileType);

        return;
    }
    
    /// <summary>
    /// 从 StreamingAssets 的子目录中的 fileList.json 中读取目录文件列表信息。
    /// </summary>
    /// <remarks>
    /// StreamingAssets 目录中的每个文件夹都对应地建立了 fileList.json 用于储存目录信息。
    /// </remarks>
    public static IEnumerator GetFolderFilesInfoFromJson(string _pth, FilesCoroutineOperations _handleObj) {
        // Android 平台下使用 Directory 访问 StreamingAssets 总是会得到假的结果
        // 因此不能判断路径是否存在，直接读取 Json 文件即可
        // 如果读不到，using 语句会被中断，不会报错崩溃
        // if (!Directory.Exists(_pth)) return new();
        
        string info_js = "";

        using(UnityWebRequest UWR_file = UnityWebRequest.Get(_pth + "/fileList.json"))
        {
            yield return UWR_file.SendWebRequest();
            if (UWR_file.result != UnityWebRequest.Result.Success) yield break;
            info_js = UWR_file.downloadHandler.text;
        }
        
        _handleObj.saff = JsonUtility.FromJson<FolderFilesNameInfo>(info_js);

        yield break;

    }

    // 异步将文件从 _frm 复制到 _to
    public static IEnumerator Copy(string _frm, string _to, string _name, FilesCoroutineOperations _handleObj)
    {
        // persistentDataPath 可读可写，能正常判断文件是否存在
        // 若 persistentDataPath 下已包含该文件，返回
        if (!Directory.Exists(_to)) {
            Directory.CreateDirectory(_to);
        }
        else if (File.Exists(_to + "/" + _name)) {
            Destroy(_handleObj);
            yield break;
        }

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

    /// <summary>
    /// 从已读取到的文件中过滤出受支持格式的文件，开启复制协程
    /// </summary>
    public static IEnumerator FilterAndCopyFiles(string _SApath, string _PDpath, string[]_fileType, FilesCoroutineOperations _handleObj) {
        while (_handleObj.saff == null) yield return null;
        FolderFilesNameInfo filesInfo = _handleObj.saff;

        // 排除不支持的文件，并将受支持的文件复制到 PD 内
        foreach (FileNameInfo fileInfo in filesInfo.filesInfoList) {
            bool legalFile = false;
            foreach (string type in _fileType) {
                if (fileInfo.fileType == type) {
                    legalFile = true;
                    break;
                }
            }
            if (!legalFile) continue;

            _handleObj.StartCoroutine_Copy(_SApath, _PDpath, fileInfo.fileName);
        }
        yield break;
    }

    /// <summary>
    /// 为 StreamingAssets 目录下的每个子文件夹建立 fileList.json 文件
    /// </summary>
    /// <remarks>
    /// 发布到 Android 平台前应先调用一次本方法，仅应在 StreamingAssets 目录内应用本方法
    /// </remarks>
    private static void CreateFileListJson(ref FolderFilesNameInfo _lst, string _pth) {
        if (!Directory.Exists(_pth)) return;
        string info_js = JsonUtility.ToJson(_lst);

        using (StreamWriter _streamWriter = new StreamWriter(_pth + "/fileList.json"))
        {
            _streamWriter.WriteLine(info_js);
            _streamWriter.Flush();
            _streamWriter.Close();
            _streamWriter.Dispose();
        }

        return;
    }
}
