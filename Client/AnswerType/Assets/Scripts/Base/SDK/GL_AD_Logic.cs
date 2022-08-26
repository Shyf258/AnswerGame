//2018.09.13    关林
//广告插播逻辑

using System;
using System.Collections;
using System.Collections.Generic;
using SUIFW;
using SUIFW.Diplomats.Agreement;
using UnityEngine;

public class GL_AD_Logic : Singleton<GL_AD_Logic>
{
    //计数器
    public int _adInterval = 30;

    /// <summary>
    /// 是否展示广告
    /// 插屏 开屏 激励
    /// </summary>
    public bool _isShowAD = false;
    //激励视频播放成功回调   
    private Dictionary<string, SADInfo> _adCallbackDic = new Dictionary<string, SADInfo>();
    //广告初始化
    public void Init()
    {

    }

    #region 游戏内逻辑

    #endregion

    //检测打点信息
    public void CheckAnalyticsLogic(EAnalyticsType type, int index = 0)
    {
    }

    public void CheckRateUs()
    {
#if China_Version 
        return;
#endif
        //检测是否弹出评价
        int value = GL_CoreData._instance._archivedData._isRateUs;
        if (value != -1)
        {
            //var info = GL_SceneManager._instance._archiveInfo;
            //if (value == 0)
            //{
            //    //通过第一章Level3后
            //    if (info._curLevelIndex == 20)
            //    {
            //        SUIFW.UIManager.GetInstance().ShowUIForms(SUIFW.SysDefine.UI_PATH_RATEUS);
            //        GL_CoreData._instance.RefreshRateUsIndex(1);
            //    }
            //}
            //else if (value == 1)
            //{
            //    //通过第二章Level1后
            //    if (info._curLevelIndex == 45)
            //    {
            //        SUIFW.UIManager.GetInstance().ShowUIForms(SUIFW.SysDefine.UI_PATH_RATEUS);
            //        GL_CoreData._instance.RefreshRateUsIndex(2);
            //    }
            //}
            //else if (value == 2)
            //{
            //    //通过第三章Level1后
            //    if (info._curLevelIndex == 70)
            //    {
            //        SUIFW.UIManager.GetInstance().ShowUIForms(SUIFW.SysDefine.UI_PATH_RATEUS);
            //        GL_CoreData._instance.RefreshRateUsIndex(-1);
            //    }
            //}
        }
    }

    /// <summary>
    /// 判断是否有广告
    /// 目前用来广告预加载
    /// </summary>
    //public bool IsAvailableAD(string ad)
    //{
    //     // return false;
    //    return GL_AD_Interface._instance.IsAvailableAD(ad);
    //}

    /// <summary>
    /// 播放广告
    /// </summary>
    public void PlayAD(string ad, Action<bool> adCallback = null, int sceneID = 0)
    {
        if (string.IsNullOrEmpty(ad))
            return;

        //无广告判断
        //if (!IsAvailableAD(ad))
        //    return;


        if (_adCallbackDic.ContainsKey(ad))
        {
            _adCallbackDic[ad]._callback = adCallback;
            _adCallbackDic[ad]._time = Time.time;
        }
        else
        {
            SADInfo info = new SADInfo();
            info._callback = adCallback;
            info._time = Time.time;
            _adCallbackDic.Add(ad, info);
        }

        if (GL_SDK._instance.GetADType(ad) == EADType.Reward)
        {
            GL_SDK._instance.PopUp();
            if (sceneID == 0)
                sceneID = GL_ConstData.SceneID_Normal;
            
            GL_SDK._instance.EntryAdScene(sceneID);
            
        }
        else
        {
            // DDebug.LogError("***** 非激励视频：" );
        }

        //UI_Diplomats._instance.ShowLoadingAD();
        GL_AD_Interface._instance.PlayAD(ad);
        //GL_Analytics_Logic._instance.SendLogEvent(ad, 0);
    }

    public void AdShowFailed(SJson param)
    {
        if (_adCallbackDic.ContainsKey(param.adSite))
        {
            _adCallbackDic[param.adSite]._callback?.Invoke(false);
            _adCallbackDic.Remove(param.adSite);
        }
    }

    //真实广告激励成功
    public void RealAdPlayCompleted(SJson sj)
    {
        if((EADType)sj.adType == EADType.Reward)
        {
          
            Net_ViewAD ad = new Net_ViewAD();
            ad.adid = sj.adSite;
            GL_ServerCommunication._instance.Send(Cmd.WatchAd, JsonUtility.ToJson(ad));
            //刷新财神气泡
            GL_PlayerData._instance.SendWithDrawConfig(EWithDrawType.MoneyPool);

            GL_PlayerData._instance.SystemConfig.viewAds += 1;
        }
        
        if(_adCallbackDic.ContainsKey(sj.adSite))
        {
            if (_adCallbackDic[sj.adSite]!=null)
            {
                _adCallbackDic[sj.adSite]._callback?.Invoke(true);
                _adCallbackDic.Remove(sj.adSite);
            }
        }
    }

    public void Vibrate(long time, int power)
    {
        //if (!GL_CoreData._instance.IsVibrate)
        //{
        //    return;
        //}

        GL_SDK._instance.Vibrate(time, power);
    }

    public void AdClicked(string param)
    {
        SJson sj = JsonUtility.FromJson<SJson>(param);
        if ((EADType)sj.adType == EADType.Reward)
        {
            Net_ViewAD ad = new Net_ViewAD();
            ad.adid = sj.adSite;
            GL_ServerCommunication._instance.Send(Cmd.DownloadAd, JsonUtility.ToJson(ad));
        }
    }

    //广告展示
    public void AdImpressed(string param)
    {
        SJson sj = JsonUtility.FromJson<SJson>(param);
        if (sj == null)
            return;
        EADType type = (EADType)sj.adType;
        if (type == EADType.Reward)
        {
            // Net_ViewAD ad = new Net_ViewAD();
            // ad.adid = sj.adSite;
            // GL_ServerCommunication._instance.Send(Cmd.WatchAd, JsonUtility.ToJson(ad));
            GL_CoreData._instance.SetECPM(sj.adEcpm);
        }
        switch (type)
        {
            case EADType.Reward:
            case EADType.Interstitial:
            case EADType.Splash:
                _isShowAD = true;
                break;
        }
    }
    public void AdClosed(string param)
    {
        SJson sj = JsonUtility.FromJson<SJson>(param);
        if (sj == null)
            return;
        EADType type = (EADType)sj.adType;
        switch (type)
        {
            case EADType.Reward:
            case EADType.Interstitial:
            case EADType.Splash:
                _isShowAD = false;
                break;
        }
    }
    #region 评价
    //跳转游戏评价
    public void Open_Evaluate()
    {
        Application.OpenURL(GL_SDK._instance.GetEvaluate());
    }

    public void Open_TermsOfUse()
    {
        //Application.OpenURL(GL_ConstData.URL_TermsOfUse);
        Func<EAgreement> typeCallBack0 = () => { return EAgreement.User; };
        UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_Agreement, typeCallBack0);
    }

    public void Open_Privacy()
    {
        //Application.OpenURL(GL_ConstData.URL_Privacy);
        Func<EAgreement> typeCallBack1 = () => { return EAgreement.Privacy; };
        UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_Agreement, typeCallBack1);
    }
    #endregion

}


public class SADInfo
{
    public float _time;
    public Action<bool> _callback;
}