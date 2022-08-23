//2022.1.27 关林
//下载文件 

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GL_DownloadFile
{
    private enum EDownState
    {
        None,
        Downloading,        //下载中
        DownloadComplete,   //下载完成
        DownloadFail,       //下载失败
    }
    private string _url;
    private string _fileName;
    private Action<string> _action;
    private EDownState _downState;  //下载状态

    public GL_DownloadFile(string url, string fileName, Action<string> action)
    {
        _url = url;
        _fileName = fileName;
        _action = action;
        _downState = EDownState.None;
        DownUrl();
    }
    private void DownUrl(float delay = 0)
    {
        if (_downState != EDownState.None && _downState != EDownState.DownloadFail)
            return;

        _downState = EDownState.Downloading;
        GL_DownloadLogic._instance.Download(_url, OnUpdateFileDownloadFinised, delay);
        

    }
    private void OnUpdateFileDownloadFinised(UnityWebRequest webRequest)
    {
        if (webRequest == null)
        {
            _downState = EDownState.DownloadFail;
            //如果下载失败，等待1s后重新下载，可以是个逐渐增长的等待时间
            DownUrl(1.0f);
            return;
        }
        DDebug.Log(webRequest.url);

        byte[] content = webRequest.downloadHandler.data;
        string writePath = string.Format("{0}/{1}", Application.persistentDataPath, _fileName);
        DDebug.Log(writePath);
        if (GL_Tools.WriteUpdateFileToDisk(writePath, content))
        {
            _action?.Invoke(writePath);
        }
    }
}
