//2022.1.18 关林
//版本管理器

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DataModule;
using UnityEngine;
using UnityEngine.Networking;

public class GL_VersionManager : Singleton<GL_VersionManager>
{
    #region 资源配置

    public static string TotalUrl = "https://static.ciyunjinchan.com/Unity/"+ QNDownloadAppEnName;
    public static string QNDownloadAppEnName => GameDataTable.GetTableBuildAppData((int) AppSetting.BuildApp).ProductEnName;

    private List<string> _resList = new List<string>()
    {
        //"/v1/AssetPack",
        //"/v1/art/assetbundle/levelimage",
        //"/v1/art/assetbundle/test/prefabtest1",
        //"/v1/art/assetbundle/test/prefabtest2",
        //"/v1/art/materia5",
        //"/v1/art/materia6",
    };


    #endregion

    #region 图片下载
    //音频下载地址
    public const string PictureUrl = "/Picture/{0}.jpg";
    private int _curDownloadPictureIndex = 11;


    private string CalculatePictureUrl(int level)
    {
        string result = string.Empty;
        var data = GameDataTable.GetTableAnswerInfoData(level);
        if(data != null && !string.IsNullOrEmpty(data.Picture))
            result = data.Picture;

        return result;
    }
    #endregion

    #region 音效下载
    //音频下载地址
    public const string AudioUrl = "/Audio/LevelAudio{0}.mp3";

    private int _curDownloadAudioIndex = 11;
    public void RefreshLevel(int level)
    {
        if((_curDownloadAudioIndex - level) < 10)
        {
            if(_curDownloadAudioIndex < level)
            {
                _curDownloadAudioIndex = level;
            }
            _needDownloadList.Clear();
            _downloadIndex = 0;
            //_resList.Clear();
            //还是十关, 可以提前下载了
            for (int i = _curDownloadAudioIndex; i < _curDownloadAudioIndex + 10; i++)
            {
                int index = GL_SceneManager._instance.CalculateReallevelIndex(i);
                string audio = index.ToString();
                if (DataModuleManager._instance.TableAnswerInfoData_Dictionary.ContainsKey(index))
                {
                    if (DataModuleManager._instance.TableAnswerInfoData_Dictionary[index].Audio != null)
                    {
                        audio = DataModuleManager._instance.TableAnswerInfoData_Dictionary[index].Audio + audio;
                    }
                    if (!audio.Equals(index.ToString()))
                    {
                        _resList.Add(string.Format(AudioUrl, audio));
                    }
                }

                string pPath = CalculatePictureUrl(index);
                if (!string.IsNullOrEmpty(pPath))
                {
                    _resList.Add(string.Format(PictureUrl, pPath));
                }
            }
            _curDownloadAudioIndex += 10;
            CB_ResVersionCallBack(null);
        }
    }
    #endregion

    //需要下载资源的 列表
    private List<string> _needDownloadList = new List<string>();
    // 已下载数量
    public int _downloadIndex = 0;
    //资源查找列表
    //public Dictionary<string, string> _resDic = new Dictionary<string, string>();

    /// <summary>
    ///  是否结束加载
    /// </summary>
    public bool IsDone { get; set; }

    /// <summary>
    /// 检查版本
    /// </summary>
    public void StartCheckVersion()
    {
        // 初始化状态
        IsDone = false;

        //初始化前十关的音效
        //图片
        int level = GL_PlayerData._instance.CurLevel;
        for (int i = level; i <= level+ 10; i++)
        {
            int index = GL_SceneManager._instance.CalculateReallevelIndex(i);
            string audio = index.ToString();
            if(DataModuleManager._instance.TableAnswerInfoData_Dictionary.ContainsKey(index))
            {
                if (DataModuleManager._instance.TableAnswerInfoData_Dictionary[index].Audio != null)
                {
                    audio = DataModuleManager._instance.TableAnswerInfoData_Dictionary[index].Audio + audio;
                }
                if (!audio.Equals(index.ToString()))
                {
                    _resList.Add(string.Format(AudioUrl, audio));
                }
            }

            //计算图片
            string pPath = CalculatePictureUrl(index);
            if(!string.IsNullOrEmpty(pPath))
            {
                _resList.Add(string.Format(PictureUrl, pPath));
            }
        }


        // 检查App版本
        CheckAppVersion();
    }
    /// <summary>
    ///  检查app版本
    /// </summary>
    private void CheckAppVersion()
    {
        //TODO:GL 后续改成 服务器返回版本信息,来判断是否要更新应用
        //GL_ServerCommunication._instance.Send(Cmd.AppVersion, CB_AppVersion);

        //当前 简单处理
        CB_AppVersion(null);
    }
    /// <summary>
    /// app 版本信息回调
    /// </summary>
    /// <param name="msg"></param>
    public void CB_AppVersion(string msg)
    {
        //1.判断是否更新应用

        //2.如果不需要更新应用, 则检测版本更新
        CheckResVersion();
    }
    /// <summary>
    /// 检查资源版本
    /// </summary>
    private void CheckResVersion()
    {
        //TODO:GL 后续改成 服务器返回资源列表 动态检测
        //GL_ServerCommunication._instance.Send(Cmd.ResVersion, CB_AppVersion);
        //当前简单处理
        CB_ResVersionCallBack(null);
    }

    /// <summary>
    /// 资源信息回调
    /// </summary>
    /// <param name="msg"></param>
    private void CB_ResVersionCallBack(string msg)
    {
        //1.检测需下载的文件,本地是否存在
        for (int i = 0; i < _resList.Count; i++)
        {
            //本地
            string path = DownloadConfig.downLoadPath + _resList[i];
            if(!GL_Tools.IsFileExist(path))
            {
                _needDownloadList.Add(_resList[i]);
            }
            else
            {
                AddDictionary(_resList[i]);
            }
        }

        if(_needDownloadList.Count <= 0)
        {
            //不需要下载
            IsDone = true;
        }
        else
        {
            DownloadUpdateFiles(_downloadIndex);
        }
    }


    private void DownloadUpdateFiles(int index, float delay = -1)
    {
        if (index >= _needDownloadList.Count)
        {
            IsDone = true;
            return;
        }
        DDebug.Log("~~~Begin download update files " + _needDownloadList[index]);

        string downloadUrl = TotalUrl + _needDownloadList[index];
        GL_DownloadLogic._instance.Download(downloadUrl, OnUpdateFileDownloadFinised);
    }

    //添加dic
    private void AddDictionary(string path)
    {
        //int IndexofA = path.LastIndexOf("/");
        //string name = path.Substring(IndexofA + 1, path.Length - IndexofA - 1);
        //_resDic.Add(name, path);
    }
    void OnUpdateFileDownloadFinised(UnityWebRequest webRequest)
    {
        if (webRequest == null)
        {
            //如果下载失败，等待1s后重新下载，可以是个逐渐增长的等待时间
            DownloadUpdateFiles(_downloadIndex, 1.0f);
            return;
        }
        DDebug.Log(webRequest.url);

        byte[] content = webRequest.downloadHandler.data;
        string path = _needDownloadList[_downloadIndex];
        string writePath = string.Format("{0}/{1}", DownloadConfig.downLoadPath, path);
        DDebug.Log(writePath);
        if (GL_Tools.WriteUpdateFileToDisk(writePath, content))
        {
            AddDictionary(path);
            _downloadIndex += 1;
            DownloadUpdateFiles(_downloadIndex);
        }
    }

    bool WriteUpdateFileToDisk(string path, byte[] content)
    {
        try
        {
            string dir = path.Substring(0, path.LastIndexOf("/"));
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            FileStream fs = new FileStream(path, FileMode.OpenOrCreate);
            fs.Write(content, 0, content.Length);
            fs.Flush();
            fs.Close();
            fs.Dispose();
            return true;
        }
        catch (Exception e)
        {
            DDebug.Log("write download file to disk exception " + e.ToString());
        }
        return false;
    }


    public string GetPath(string value)
    {
        string result = string.Empty;
        foreach (var item in _resList)
        {
            if (item.Contains(value))
            {
                result = item;
                break;
            }

        }

        return result;
    }

}
