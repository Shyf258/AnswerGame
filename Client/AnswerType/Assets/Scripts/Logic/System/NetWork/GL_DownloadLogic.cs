//2019.01.30    关林
//下载逻辑

//using SUIFW;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Video;

public static class DownloadConfig
{
    public static string downLoadPath = "";

    static DownloadConfig()
    {
        downLoadPath = Application.persistentDataPath;
    }
}

public class GL_DownloadLogic : Singleton<GL_DownloadLogic>
{
    /// <summary>
    ///  下载
    /// </summary>
    /// <param name="url"></param>
    /// <param name="downloadFinishedCallback"></param>
    /// <param name="delay"></param>
    public void Download(string url, Action<UnityWebRequest> downloadFinishedCallback, float delay = -1)
    {
        MethodExeTool.StartCoroutine(DownloadCoroutine(url, downloadFinishedCallback, delay));
    }

    public IEnumerator DownloadCoroutine(string url, Action<UnityWebRequest> downloadFinishedCallback, float delay)
    {
        if (delay > 0)
            yield return new WaitForSeconds(delay);

        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();
            if (string.IsNullOrEmpty(webRequest.error) && webRequest.isDone)
            {
                downloadFinishedCallback?.Invoke(webRequest);
            }
            else
            {
                DDebug.LogError("~~~Download failed: " + webRequest.error);
                downloadFinishedCallback?.Invoke(null);
            }
            webRequest.Dispose();
        }

    }
}
