//2019.12.03    关林
//SDK接入 和 接口
//脚本需要挂在主场景中 名字为 "GSdk" 的GameObject上!!!!

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using SUIFW;
using Random = UnityEngine.Random;

public class GL_SDK : Mono_Singleton_DontDestroyOnLoad<GL_SDK>
{
    [Header("1.激励视频")]
    public List<string> _rewardADList = new List<string>();
    [Header("2.插屏")]
    public List<string> _interstitialList = new List<string>();
    [Header("3.原生广告")]
    public List<string> _nativeList = new List<string>();
    [Header("4.banner广告")]
    public List<string> _bannerList = new List<string>();
    [Header("5.开屏广告")]
    public List<string> _splashList = new List<string>();
    #region Android 初始化
    private AndroidJavaObject _javaObject = null;
    private AndroidJavaObject _javaObject_Unity = null;

    public void Init()
    {
        try
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            Debug.Log("~~~Andraid SDK Initializing");

            AndroidJavaClass _javaClass = new AndroidJavaClass("com.cocoa.ad.game.GGame");
            if (_javaClass == null)
                Debug.Log("~~~SDK Initialize Failure: " + "_javaClass = null");

            _javaObject = _javaClass.CallStatic<AndroidJavaObject>("getInstance");
            if (_javaObject == null)
                Debug.Log("~~~SDK Initialize Failure: " + "_javaObject = null");

            AndroidJavaClass unityActivity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            _javaObject_Unity = unityActivity.GetStatic<AndroidJavaObject>("currentActivity");

            _javaObject.Call("setContext", _javaObject_Unity);
#endif
        }
        catch (System.Exception e)
        {
            Debug.LogError("~~~SDK Initialize Failure" + e);
        }
    }


    #endregion

    #region 广告接口
    /// <summary>
    /// 判断广告位是否有广告
    /// </summary>
    public bool IsAvailableAD(int adType)
    {
        bool result = true;
#if UNITY_ANDROID && !UNITY_EDITOR
        if (_javaObject != null)
             result = _javaObject.Call<bool>("adAvailable", adType);
#elif UNITY_IOS && !UNITY_EDITOR
        //result = available_at_position(adSite);
#endif
        return result;
    }

    /// <summary>
    /// 显示广告
    /// </summary>
    public void DisplayAd(string adSite)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
		if (_javaObject != null)
            _javaObject.Call("displayAd", adSite);

#elif UNITY_IOS && !UNITY_EDITOR
			display_at_position(adSite);
#endif
    }

    /// <summary> 关闭原生广告 </summary>
    public void CloseNativeAd()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
		if (_javaObject != null)
            _javaObject.Call("closeNativeAd");

#elif UNITY_IOS && !UNITY_EDITOR
		
#endif
    }


    /// <summary> 关闭横幅广告 </summary>
    public void CloseBannerAd()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
		if (_javaObject != null)
            _javaObject.Call("closeBannerAd");

#elif UNITY_IOS && !UNITY_EDITOR
	
#endif
    }

    /// <summary> 进入广告场景 </summary>
    public void EntryAdScene(int id)
    {
        // DDebug.LogError("***** 进入广告场景id："+ id );
#if UNITY_ANDROID && !UNITY_EDITOR
		 if (_javaObject != null)
        {
            // DDebug.LogError("***** 对象不为空");
            _javaObject.Call("enterScene", id);
        }

#elif UNITY_IOS && !UNITY_EDITOR
	
#endif
    }
    /// <summary> 隐藏安卓的闪屏 </summary>
    public void HideSplash()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
		if (_javaObject != null)
            _javaObject.Call("onHideSplash");

#elif UNITY_IOS && !UNITY_EDITOR
	
#endif
    }

    #endregion

    #region 功能接口


    /// <summary>
    /// 请求权限
    /// </summary>
    public void RequestAdPermissions(bool set)
    {
        DDebug.LogError("~~~sdk: 申请设备权限!");
        try
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            _javaObject.Call("requestAdPermissions", set);
#elif UNITY_IOS && !UNITY_EDITOR
#endif
        }
        catch (Exception)
        {
        }
    }

    /// <summary>
    /// 通知SDK UserSecret
    /// </summary>
    public void SetUserSecret(string secret)
    {
        try
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            _javaObject.Call("setUserSecret", secret);
#elif UNITY_IOS && !UNITY_EDITOR
#endif
        }
        catch (Exception)
        {
        }
    }
    /// <summary>
    /// 安装应用
    /// </summary>
    public void InstallApk(string apkPath)
    {
        DDebug.LogError("~~~sdk: 通知安装应用: " + apkPath);
        try
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            _javaObject.Call("installApk", apkPath);
#elif UNITY_IOS && !UNITY_EDITOR
#endif
        }
        catch (Exception)
        {
        }
    }
   // private string str = "观看完整视频可获得高额奖励";
   //
   // private string point = "点击广告可以获得更高额度奖励哟！";
    //提示弹窗
    public void PopUp()
    {
        string str = "观看完整视频可获得高额奖励";
        //try
        //{
        //    if (GL_PlayerData._instance.CbPlayAd != null)
        //    {
        //        if (GL_PlayerData._instance.CbPlayAd.induceRate>=Random.Range(1,10000) )
        //        {
        //            str =  "点击广告可以获得更高额度奖励哟！";
        //        }
        //    }
        //    else
        //    {
        //        str =   "观看完整视频可获得高额奖励";
        //    }
        //}
        //catch
        //{
        //    str = "观看完整视频可获得高额奖励";
        //    GL_PlayerData._instance.GetPlayAdConfig();
        //}
        
        
        
        try
        {
#if UNITY_ANDROID && !UNITY_EDITOR
        _javaObject.Call("showToastDialog", str);
        _javaObject.Call("showFloatingRewardMsg", "观看完成视频可获取奖励",AppSetting.BuildApp == EBuildApp.BXDYJ);
#elif UNITY_IOS && !UNITY_EDITOR

#endif
        }
        catch (Exception)
        {
        }
    }
    /// <summary>
    /// 获得广告类型
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public EADType GetADType(string key)
    {
        
        EADType type = EADType.None;
        
        if (_rewardADList.Contains(key))
        {
            type = EADType.Reward;
        }
        else if (_interstitialList.Contains(key))
        {
            type = EADType.Interstitial;
        }
        else if (_nativeList.Contains(key))
        {
            type = EADType.Native;
        }
        else if (_bannerList.Contains(key))
        {
            type = EADType.Banner;
        }
        else if (_splashList.Contains(key))
        {
            type = EADType.Splash;
        }
        return type;
    }
    
    //手机振动
    public void Vibrate(long time, int power)
    {
        try
        {
#if UNITY_ANDROID && !UNITY_EDITOR
        _javaObject.CallStatic("runShake", _javaObject_Unity, time, power);
#elif UNITY_IOS && !UNITY_EDITOR
        call_system_vibrate();
#endif
        }
        catch (Exception)
        {
        }
    }

    
    
    //事件打点
    public void LogEvent(string eventType, string eventName)
    {
        //DDebug.LogError("~~~发送打点类型:" + eventType + " 打点信息:" + eventName);

#if UNITY_ANDROID && !UNITY_EDITOR
        if (_javaObject != null)
             _javaObject.Call("logEvent", eventName, eventType);
#elif UNITY_IOS && !UNITY_EDITOR
        log_event(eventName, eventType);
#endif
        
    }

    /// <summary>
    /// 游戏分享
    /// </summary>
    public void GameShare()
    {
        //GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.GameShare_Click);
        string icon = (Application.persistentDataPath + "/" +GL_ConstData.ServerTexturePath + GL_PlayerData._instance.WeChatIcon);
        // DDebug.LogError("*****游戏分享链接地址"+icon);
        // DDebug.LogError("*****游戏分享微信名"+GL_PlayerData._instance.WeChatName);
        // DDebug.LogError("*****游戏分享邀请码"+GL_PlayerData._instance.InvitationCode);
        // DDebug.LogError("*****游戏分享邀请链接"+GL_PlayerData._instance.InvitationUrl);
#if GuanGuan_Test
      
#endif
     
#if UNITY_ANDROID && !UNITY_EDITOR
         if (_javaObject != null)
            _javaObject.Call("weChatShare",icon,GL_PlayerData._instance.WeChatName,GL_PlayerData._instance.InvitationCode,GL_PlayerData._instance.InvitationUrl);
#elif UNITY_IOS && !UNITY_EDITOR
        // log_event(eventName, eventType);
#endif
    }
    
    
    /// <summary>
    /// 游戏分享
    /// </summary>
    public void GameShareBGSK()
    {
       
#if GuanGuan_Test

#endif

#if UNITY_ANDROID && !UNITY_EDITOR
         if (_javaObject != null)
            _javaObject.Call("weChatShareBG",GL_PlayerData._instance.InvitationUrl, GL_PlayerData._instance.InvitationCode);
#elif UNITY_IOS && !UNITY_EDITOR
        // log_event(eventName, eventType);
#endif
    }
    
    /// <summary>
    /// 注销
    /// </summary>
    public void GameClear()
    {
       
#if UNITY_ANDROID && !UNITY_EDITOR
         if (_javaObject != null)
            _javaObject.Call("GameClear");
#elif UNITY_IOS && !UNITY_EDITOR
        // log_event(eventName, eventType);
#endif
    }

    
    public string GetProductName()
    {
        string result = Application.productName;
#if UNITY_ANDROID && !UNITY_EDITOR
        if (_javaObject != null)
             result = _javaObject.Call<string>("getProductName");
#elif UNITY_IOS && !UNITY_EDITOR
        //result = available_at_position(adSite);
#endif

        return result;
    }

    //获取系统版本
    public string GetSystemVersion()
    {
        string result = "android 4.3.0";
#if UNITY_ANDROID && !UNITY_EDITOR
        if (_javaObject != null)
             result = _javaObject.Call<string>("getSystemVersion");
#elif UNITY_IOS && !UNITY_EDITOR
        //result = available_at_position(adSite);
#endif
        return result;
    }

    //获取应用版本
    public string GetAppVersion()
    {
        string result = "1.0.1";
#if UNITY_ANDROID && !UNITY_EDITOR
        if (_javaObject != null)
             result = _javaObject.Call<string>("appVersion");
#elif UNITY_IOS && !UNITY_EDITOR
        result = appVersion();
#endif
        return result;
    }

    //获取GAID
    public string GetDeviceGAID()
    {
        string result = SystemInfo.deviceUniqueIdentifier + GL_Tools.GetRandomStr(10,true, true, true, true);

        //DDebug.Log("GAID : " + result);
        //result = "10dd8d60cd40c26a9abd18b177a410e3a47c633ajrz6008";
#if UNITY_ANDROID && !UNITY_EDITOR
        if (_javaObject != null)
             result = _javaObject.Call<string>("getDeviceGAID");
#elif UNITY_IOS && !UNITY_EDITOR
        result = open_uuid();
#endif
        return result;
    }


    //获取评价链接
    public string GetEvaluate()
    {
        string result = "https://itunes.apple.com/us/app/wobble-man/id1514732634?mt=8";
#if UNITY_ANDROID && !UNITY_EDITOR
        if (_javaObject != null)
             result = _javaObject.Call<string>("sdk_APPEvaluate");
#elif UNITY_IOS && !UNITY_EDITOR
        result = sdk_APPEvaluate();
#endif
        return result;
    }

    /// <summary>
    /// 获得应用常见信息
    /// deSecret, channel pkgName vn vc installTime
    /// </summary>
    public string GetCommonInfo()
    {
        DDebug.LogError("~~~GetCommonInfo1");
        string result = string.Empty;

        if (AppSetting.IsTestUnity)
        {
            Net_RequesetCommon com = new Net_RequesetCommon();
            com.deSecret = GetDeviceGAID();
            com.userSecret = GL_PlayerData._instance.UserSecret;
            com.pkgName = GL_ConstData.PackageName;
            com.vn = GetAppVersion();
            com.vc = 7;
            com.shuSecret = "testtest";
            com.cityId = "111";
            com.provinceId = "222";
            com.planId = "";
            //com.abTest = "b";
            com.abTest = AppSetting.AbTest;
            result = JsonUtility.ToJson(com);
        }
        try
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (_javaObject != null)
            {
                result = _javaObject.Call<string>("getCommonInfo");
                DDebug.LogError("~~~GetCommonInfo2 : " + result);
            }
            else
            {
                DDebug.LogError("~~~GetCommonInfo : _javaObject == null");
            }
#elif UNITY_IOS && !UNITY_EDITOR
        //result = open_uuid();
#endif
        }
        catch (Exception e)
        {
            DDebug.LogError("~~~GetCommonInfo error : " + e);
        }


        return result;
    }


 
    

    #endregion

#region 广告回调
    //一次广告请求，加载到广告时调⽤
    public void onAdAvailable(string param)
    {
        GL_AD_Interface._instance.CB_AdAvailable(param);
    }

    //广告点击时调⽤
    public void onAdClicked(string param)
    {
        GL_AD_Interface._instance.CB_AdClicked(param);
    }


    //广告关闭时调用
    public void onAdClosed(string param)
    {
        GL_AD_Interface._instance.CB_AdClosed(param);
    }

    //广告展示时调用
    public void onAdImpressed(string param)
    {
        GL_AD_Interface._instance.CB_AdImpressed(param);
    }

    //视频播放完成后调用
    public void onAdPlayCompleted(string param)
    {

        GL_AD_Interface._instance.CB_AdPlayCompleted(param);

    }
    //视频播放失败
    public void onAdShowFailed(string param)
    {
        GL_AD_Interface._instance.CB_AdShowFailed(param);
    }
#endregion

#region sdk逻辑回调
    //通用城市 数据
    public void onIpCallback(string param)
    {
        DDebug.LogError("~~~onIpCallback: " + param);
        GL_Game._instance.WaitIpCallback(param);
    }

#endregion

#region 第三方登陆

    private Action<string> _loginCallback;
    /// <summary>
    /// 三方登陆
    /// </summary>
    /// <param name="type">wx:微信登陆  qq:qq登陆</param>
    public void Login(string loginType, Action<string> action)
    {
        GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.WeChatClick);
        _loginCallback = action;
#if UNITY_EDITOR
        //模拟
        switch (loginType)
        {
            case "wechat":
                {
                    Net_CB_WeChatLogin we = new Net_CB_WeChatLogin();
                    we.nickname = "测试账号";
                    we.icon = "https://thirdwx.qlogo.cn/mmopen/vi_32/DYAIOgq83erdJpia0RSjlbQgTwtGibpVZkPxNtNFWMQWLhHZBlu5E9QL3DLd1Pkj9JyoDn746Mfuiaymz4jrYnSicg/132";
                    we.invitationCode = "10110101";
                    onWeChatLoginSuccess(JsonUtility.ToJson(we));
                }
                break;
            case "qq":
                {
                }
                break;
        }
#endif

#if UNITY_ANDROID && !UNITY_EDITOR
                if (_javaObject != null)
                    _javaObject.Call("login",loginType);
#elif UNITY_IOS && !UNITY_EDITOR
                //login(loginType);
#endif
    }

    /// <summary>
    /// 微信登陆成功回调
    /// </summary>
    /// <param name="param"></param>
    public void onWeChatLoginSuccess(string param)
    {
        GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.WeChatLoginResult);
        //DDebug.LogError("~~~onWeChatLoginSuccess");
        _loginCallback?.Invoke(param);
        _loginCallback = null;


    }

    /// <summary>
    /// 微信登陆失败回调
    /// </summary>
    /// <param name="param"></param>
    public void onWeChatLoginFail(string param)
    {
        DDebug.LogError("~~~onWeChatLoginFail");
        _loginCallback?.Invoke(null);
        _loginCallback = null;
    }

    #endregion

    #region 推送 和日历
    public void SendNotification(int id, string title, string message, int delayMs, int intervalMs, int sound, string soundName, int vibrate, int lights)
    {
        DDebug.LogError("SendNotification: " + id  + "  title: " + title + "  message:" + "  delayMs: " + delayMs + "  intervalMs: " + intervalMs);
#if UNITY_ANDROID && !UNITY_EDITOR
        if (_javaObject != null)
            _javaObject.Call("setNotification", id, title, message, delayMs, intervalMs, sound, soundName, vibrate, lights);
#elif UNITY_IOS && !UNITY_EDITOR
#endif
    }
    public void CanceNotification(int id)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (_javaObject != null)
            _javaObject.Call("cancelNotification", id);
#elif UNITY_IOS && !UNITY_EDITOR
#endif
    }


    /// <summary>
    /// 检测是否有日历权限
    /// </summary>
    public bool CheckCalendarPermission()
    {
        
        bool result = false;
#if UNITY_ANDROID && !UNITY_EDITOR
        if (_javaObject != null)
            result = _javaObject.Call<bool>("checkCalendarPermission");
#elif UNITY_IOS && !UNITY_EDITOR
        result = false;
#endif
        // DDebug.LogError("***** 日历权限："+ result);
        return result;
    }

    //请求日历权限 ,  发送日历内容
    public void RequestCalendarPermission(string title, string content)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (_javaObject != null)
            _javaObject.Call("requestCalendarPermission", title, content);
#elif UNITY_IOS && !UNITY_EDITOR

#endif
        // DDebug.LogError("***** 日程标题"+ title);
        // DDebug.LogError("***** 日程内容"+ content);
    }
    
    
    
    #endregion

    #region sdk网络通信

    //获得头信息
    public string GetAuthorization()
    {
        string result = string.Empty;
#if UNITY_ANDROID && !UNITY_EDITOR
                if (_javaObject != null)
                     result = _javaObject.Call<string>("getAuthorization");
#elif UNITY_IOS && !UNITY_EDITOR
                //result = open_uuid();
#endif
        return result;
    }

    //加密
    public string GetHttpBody(string json)
    {
        string result = string.Empty;
#if UNITY_ANDROID && !UNITY_EDITOR
                if (_javaObject != null)
                     result = _javaObject.Call<string>("getHttpBody", json);
#elif UNITY_IOS && !UNITY_EDITOR
                //result = open_uuid();
#endif
        return result; 
    }
    //解密
    public string GetResponseBody(string json)
    {
        string result = string.Empty;
#if UNITY_ANDROID && !UNITY_EDITOR
                if (_javaObject != null)
                     result = _javaObject.Call<string>("getResponseBody", json);
#elif UNITY_IOS && !UNITY_EDITOR
                //result = open_uuid();
#endif
        return result;
    }
    //    /// <summary>
    //    /// 网络请求
    //    /// </summary>
    //    public void SendCommunication(Cmd type, string url, string method, string postDataJson)
    //    {
    //#if UNITY_EDITOR
    //#elif UNITY_ANDROID
    //            SRequesetJsonData requesetJsonData = new SRequesetJsonData();
    //            requesetJsonData.url = url;
    //            requesetJsonData.requestData = postDataJson;
    //            requesetJsonData.idx = ((int)type).ToString();

    //            string jsonData = JsonUtility.ToJson(requesetJsonData);
    //            _javaObject.Call("commonRequest", jsonData);
    //#endif
    //    }


    //    /// <summary>
    //    /// 网络请求回调成功接口
    //    /// </summary>
    //    /// <param name="response"></param>
    //    public void commonReuqestSuccess(string response)
    //    {
    //        Debug.Log("安卓加密返回数据：" + response);

    //        Net_SDK_CB_JsonData resqJsonData = JsonConvert.DeserializeObject<Net_SDK_CB_JsonData>(response);

    //        Debug.Log("~~~远端数据：" + resqJsonData);
    //        //JsonData jsonDataRes = JsonMapper.ToObject(resqJsonData.response);

    //        //Debug.Log("远端数据：" + resqJsonData);

    //        //if (!jsonDataRes["code"].Equals(200))
    //        //{
    //        //    Tuner.Log(jsonDataRes["message"].ToString());
    //        //    FGui.ToastBoard().SetToastInfo(jsonDataRes["message"].ToString());
    //        //    FGui.ToastBoard().Show();
    //        //    //return;
    //        //}

    //        //if (((IDictionary)jsonDataRes).Contains("data"))
    //        //{
    //        //    _callbacks?.Invoke(resqJsonData.idx, jsonDataRes["data"].ToJson());

    //        //    Debug.Log($"!!!!!!!!!!!请求回调成功接口:index:{resqJsonData.idx}----data:{jsonDataRes["data"].ToJson()}");
    //        //}

    //        //FGui.TopBoard().Hide();
    //    }

    #endregion

    #region 广告提现页面打开
    
    //广告点击时调⽤
    public void onPlayAdPage(string param)
    {
        //if (!GL_CoreData._instance.AbTest)
        {
            GL_PlayerData._instance.SendWithDrawConfig(EWithDrawType.TipsPage, (() =>
            {
                if (GL_PlayerData._instance._withDrawTarget == null) 
                    return;
                    
                if (GL_PlayerData._instance._withDrawTarget[EWithDrawType.TipsPage].withDrawLimit>0)
                {
                    GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.WithDrawTipPopup);
                    //打开页面
                    UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_PlayAdWithDraw/*,sj
                    */);
                }
            }));
           
        }
    }
    /// <summary>
    /// 开始计时
    /// </summary>
    /// <param name="time">提现次数</param>
    public void CoolDown(int time)
    {
        //DDebug.LogError("广告提现页配置剩余次数："+time);
        try
        {
#if UNITY_ANDROID && !UNITY_EDITOR
        _javaObject.Call("closeWithDrawDialog",time);
#elif UNITY_IOS && !UNITY_EDITOR
        call_system_vibrate();
#endif
        }
        catch (Exception)
        {
        }
    }

    #endregion
    
    #region sdk SessId验证
    private Dictionary<string, SDK_Packet> _packetDic = new Dictionary<string, SDK_Packet>();
    public void GetSessId(SDK_Packet packet)
    {
        if (!packet.requestData.IsCheckSM)
            return;
        if (_packetDic.ContainsKey(packet.requestData.SMeventCode))
            return;

        _packetDic.Add(packet.requestData.SMeventCode, packet);

        if(AppSetting.IsTestUnity)
        {
            SSessId ss = new SSessId();
            ss.sessId = "test";
            ss.eventCode = packet.requestData.SMeventCode;
            onSessIdCallback(JsonUtility.ToJson(ss));
        }

#if UNITY_ANDROID && !UNITY_EDITOR
        if (_javaObject != null)
             _javaObject.Call("getSessId", packet.requestData.SMeventCode, packet.requestData.SMoptMsg);
#elif UNITY_IOS && !UNITY_EDITOR
        //result = available_at_position(adSite);
#endif
    }

    public void onSessIdCallback(string param)
    {
        GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.SessIdCallback);
        Debug.LogError("~~~onSessIdCallback: " + param);
        SSessId ss = JsonUtility.FromJson<SSessId>(param);
        if (_packetDic.ContainsKey(ss.eventCode))
        {
            var data = _packetDic[ss.eventCode];

            //替换sessId
            int index = data.postDataJson.IndexOf("sessId");
            index = data.postDataJson.IndexOf(":", index);
            data.postDataJson = data.postDataJson.Insert(index + 2, ss.sessId);

            GL_ServerCommunication._instance.RealSend(data.requestData, data.type, data.postDataJson, data.action, data.urlParams);

            _packetDic.Remove(ss.eventCode);
        }
    }
    #endregion

    #region iOS 定义接口
#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
	public static extern void init_sdk();

	[DllImport("__Internal")]
	public static extern bool available_at_position(string position);

	[DllImport("__Internal")]
	public static extern void display_at_position(string position);
    [DllImport("__Internal")]
    public static extern void display_banner();
    [DllImport("__Internal")]
    public static extern void log_event(string name, string type);
     [DllImport("__Internal")]
    public static extern void log_eventaf(string name, string type);

	[DllImport("__Internal")]
	public static extern void change_visibility_of_position(string position,int visibility);
    [DllImport("__Internal")]
	public static extern void call_system_vibrate();
    [DllImport("__Internal")]
    public static extern void log_purchase(string pid,string type,float price,string currency);
    [DllImport("__Internal")]
    public static extern string open_uuid();

    [DllImport("__Internal")]
    public static extern string abtest_config(string key);
    [DllImport("__Internal")]
    public static extern void set_level(int level);
    [DllImport("__Internal")]
    public static extern void log_douyin_ad_event(string site, string type);
    [DllImport("__Internal")]
    public static extern bool isShowAntiAddictionAgePopup();
    [DllImport("__Internal")]
    public static extern int ageLimit();
    [DllImport("__Internal")]
    public static extern string appVersion();
    [DllImport("__Internal")]
    public static extern void login(string type);
    [DllImport("__Internal")]
    public static extern bool sdk_isShowAppleLoginBtn();
    [DllImport("__Internal")]
    public static extern string sdk_APPEvaluate();
    [DllImport("__Internal")]
    public static extern void FacebookLogout();// FB退出登录

#endif
    #endregion
}

//SDK返回值 格式
public class SJson
{
    public string adSite;
    public int adType;  //1激励 2插屏 3原生 4banner 5开屏
    public string adEcpm;
    public string reqAdId;
}



public class SSessId
{
    public string eventCode;
    public string sessId;
}