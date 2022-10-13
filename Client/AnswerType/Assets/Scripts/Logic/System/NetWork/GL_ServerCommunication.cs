//2018.12.03    关林
//服务器通讯

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using DataModule;
using System.Security.Cryptography;
using Encryption;
using SUIFW;

public class GL_ServerCommunication : Singleton<GL_ServerCommunication>
{
    private float _uploadPrograss;
    public float UploadPrograss { get => _uploadPrograss; }

    // 回调
    private Dictionary<int, Action<string>> _callbackDic = new Dictionary<int, Action<string>>();
    public void Init()
    {
    }


    #region 接口

    public void Send(Cmd type, string postDataJson = "", Action<string> action = null, List<UrlParams> urlParams = null)
    {
        TableNetworkRequestData requestData = GameDataTable.GetTableNetworkRequestData((int)type);

        //添加参数
        var joinUrl = JoinURL(requestData.JointURL, urlParams);
        string url = GL_ConstData.HTTP + CalculateURL(requestData.URL) + joinUrl;

        string method = requestData.Method;

        if (action != null && !_callbackDic.ContainsKey((int)type))
            _callbackDic.Add((int)type, action);

        DDebug.Log("~~~[Send]" + type + " [Json] " + postDataJson + " [URL]" + url);

        MethodExeTool.StartCoroutine(SendCommunication(type, url, method, postDataJson, action));
    }
    
    public void RealSend(TableNetworkRequestData requestData,Cmd type,  string postDataJson = "", Action<string> action = null, List<UrlParams> urlParams = null)
    {
        //添加参数
        var joinUrl = JoinURL(requestData.JointURL, urlParams);
        string url = GL_ConstData.HTTP + CalculateURL(requestData.URL) + joinUrl;

        string method = requestData.Method;

        if (action != null && !_callbackDic.ContainsKey((int)type))
            _callbackDic.Add((int)type, action);

        DDebug.Log("~~~[Send]" + type + " [Json] " + postDataJson + " [URL]" + url);

        MethodExeTool.StartCoroutine(SendCommunication(type, url, method, postDataJson, action));
    }
    
    private string description = "您需要观看{0}次视频";
    private IEnumerator SendCommunication(Cmd type, string url, string method, string postDataJson, Action<string> action)
    {
        UnityWebRequest request = new UnityWebRequest(url, method);

        #region 添加参数
        if (!string.IsNullOrEmpty(postDataJson))
        {
            //加密 
            if (!AppSetting.IsTestUnity)
            {
                postDataJson = GL_SDK._instance.GetHttpBody(postDataJson);
            }


            byte[] bytes = Encoding.UTF8.GetBytes(postDataJson);
            request.uploadHandler = new UploadHandlerRaw(bytes);
        }
        #endregion

        #region 添加头信息

        if (AppSetting.IsTestUnity)
        {
            //加密测试
            request.SetRequestHeader("isTest", "ok");
        }
        else
        {
            request.SetRequestHeader("Authorization", GL_SDK._instance.GetAuthorization());
        }

        request.SetRequestHeader("Content-Type", "application/json");
        #endregion

        request.certificateHandler = new WebRequestCert();
        request.timeout = 5;

        request.downloadHandler = new DownloadHandlerBuffer();

        request.SendWebRequest();
        while (true)
        {
            if (request.isDone)
            {
                string json = request.downloadHandler.text;
                if (!AppSetting.IsTestUnity)
                {
                    json = GL_SDK._instance.GetResponseBody(json);
                }

                if (request.downloadHandler != null)
                {
                    DDebug.Log("~~~[Recive]" + type + "[Json]" + json + "code  " + request.responseCode);
                }

                if (request.isNetworkError)
                {
                    Debug.LogError("~~~http: " + type + " 请求错误:" + request.error);
                }
                else
                {

                    if (request.responseCode == 200)
                    {
                        Net_Normal msg = JsonHelper.FromJson<Net_Normal>(json);
                        switch ((EResponseCode)msg.code)
                        {
                            case EResponseCode.Succeed:
                                {
                                    //处理回调
                                    if (IsSplitNormalJson(type))
                                    {
                                        json = JsonHelper.GetJsonValue(json, "data");
                                    }

                                    if (action != null)
                                        action.Invoke(json);
                                }

                                break;
                            case EResponseCode.TokenInvalid:
                                //刷新token
                                GL_PlayerData._instance.SendLoginGuest();
                                break;
                            case EResponseCode.BlackUser:
                                //黑名单
                                // GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.Login_Blacklist);
                                Application.Quit();
                                break;
                            case EResponseCode.WithdrawFailure:
                                SUIFW.Diplomats.Common.UI_HintMessage._.ShowMessage(msg.message);
                                break;
                            case EResponseCode.TipsADS:
                                break;
                            default:
                                break;
                        }
                    }
                }
                break;
            }

            yield return null;

        }
        request.Dispose();
    }


    private bool IsSecret(Cmd cmd)
    {
        //switch (cmd)
        //{
        //    case Cmd.PostGoogleOrders:
        //        return true;
        //}
        return false;
    }
    //是否参数加密
    private bool IsParamEncrypt(Cmd cmd)
    {
        //switch (cmd)
        //{
        //    case Cmd.PostGoogleOrders:
        //        return true;
        //}
        return false;
    }

    /// <summary>
    /// 是否分离t通用json
    /// </summary>
    /// <returns></returns>
    private bool IsSplitNormalJson(Cmd cmd)
    {
        //switch (cmd)
        //{
        //    case Cmd.LoginVisitor:
        //    case Cmd.BindingApple:
        //    case Cmd.BindingFaceBook:
        //    case Cmd.UnbindFaceBook:
        //    case Cmd.UnbindApple:
        //    case Cmd.PostGoogleOrders:
        //        return false ;
        //}
        return true;
    }

    private void CheckIllegalCharacter(Net_CB_ErrorInfo error)
    {
        switch (error.message)
        {
            case 1001:
                //UI_Diplomats._instance.ShowTipWind("通知", LanguageMgr.GetInstance().ShowText(13016), LanguageMgr.GetInstance().ShowText(10010));
                break;
        }
    }

    //是否有参数
    private bool IsParam(Cmd type)
    {
        switch (type)
        {
            //case ERequestType.GuestLogin:
            //case ERequestType.SetUserRankScore:
            //case ERequestType.SetUserListScore:
            //case ERequestType.SetPlayInfo:
            //case ERequestType.GetPlayInfo:
            //    return true;
            default:
                break;
        }
        return true;
    }

    private string JoinURL(string url, List<UrlParams> list)
    {
        if (list == null || list.Count == 0)
            return url;
        for (int i = 0; i < list.Count; ++i)
        {
            if (url.Contains(list[i].key))
            {
                url = url.Replace(list[i].key, list[i].value);
            }
        }
        return url;
    }



    #endregion

    private string CalculateURL(int id)
    {
        string result;
        var data = GameDataTable.GetTableNetworkURLData(id);

        switch (AppSetting.ServerType)
        {
            case EServerType.Test:
                result = data.TestURL;
                break;
            case EServerType.Noraml:
#if UNITY_EDITOR
                result = data.TestURL;
#else
                result = data.URL_CN;
#endif
                break;
            case EServerType.Local:
                result = data.IntranetTest;
                break;
            default:
                result = data.TestURL;
                break;
        }

        //result += ":8080";
        return result;
    }

    //计算api签名
    public string CalculateSecret()
    {
        string result;
#if GuanGuan_Test || UNITY_EDITOR
        result = "S4yrbflbANL517";
#else
        result = "fhezEttx5yJuVN";
#endif
        return result;
    }
    //计算参数密钥
    public string CalculateParamKey()
    {
        string result;
#if GuanGuan_Test || UNITY_EDITOR
        result = "B5HMExxaeZnRPOc5yckjscnx/3AAWhuqHY3MKNVE+Dc=";
#else
         result = "tlKf/tkSUjzzXr3qUMXZ76Z0rlJ2MHUQz+AmXuWnS3A=";
#endif
        return result;
    }
 
    /// 获取图片
    /// </summary>
    /// <param name="url"></param>
    /// <param name="callBack"></param>
    public void GetTexturePic(string url, System.Action<Texture2D> callBack)
    {
        DDebug.Log("GetTexture " + url);
        string md5 = GL_Tools.GetMd5String(url);
        if (string.IsNullOrEmpty(md5))
            return;

        Texture2D tex = GL_Tools.LoadPNGTORenderTexture(GL_ConstData.ServerTexturePath, md5);
        
        
        if (tex != null)
        {
            callBack(tex);
            return;
        }
        Debug.Log("Texture " + url);
        GL_Game._instance.StartCoroutine(GetTextureHttp(url, callBack));
    }

    private IEnumerator GetTextureHttp(string url, System.Action<Texture2D> callBack)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            DownloadHandlerTexture texDl = new DownloadHandlerTexture(true);
            webRequest.downloadHandler = texDl;

            // 等待下载
            yield return webRequest.SendWebRequest();


            if (string.IsNullOrEmpty(webRequest.error) && !webRequest.isNetworkError && webRequest.isDone)
            {
                if (texDl.texture != null)
                {
                    if (callBack != null)
                    {
                        GL_Tools.SaveTexture2DToPNG(texDl.texture, GL_ConstData.ServerTexturePath, GL_Tools.GetMd5String(url));

                        callBack(texDl.texture);

                        GL_CoreData._instance.SaveData();
                    }
                }
            }
            else
            {
                DDebug.LogError(webRequest.error);
            }

        }
    }
}

/// <summary>
/// 链接拼接
/// </summary>
public class UrlParams
{
    public string key;
    public string value;
    public UrlParams(string key, string value)
    {
        this.key = key;
        this.value = value;
    }
}



