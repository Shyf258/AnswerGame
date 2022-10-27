using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using UnityEngine;

public static class AppSetting
{
    public static EAppResourceLoadMode LoadMode;
    public static EServerType ServerType;
    public static bool IsCloseGuide;
    private static bool isLog;
    public static bool IsTestUnity;
    public static EGameEnterType EnterType;
    public static string AbTest;
    public static bool IsSkipAD;
    public static EBuildApp BuildApp;
    public static bool Check;
    public static bool IsSikpWechatLogin;
    public static double BuildTime;
    public static int BuildHour;
    public static bool IsOpenSessId;
    public static void Init()
    {
        AppSettingConfig config = Resources.Load<AppSettingConfig>("AppSettingConfig");
        if (config == null)
        {
            LoadMode = EAppResourceLoadMode.Resource;
            ServerType =  EServerType.Noraml;
            IsCloseGuide = false;
            isLog = config._isLog;
            IsTestUnity = false;
            EnterType =  EGameEnterType.WaitServer;
            AbTest = "a";
            IsSkipAD = false;
            BuildApp = EBuildApp.RSDYJ;
            Check = false;
            IsSikpWechatLogin = false;
            BuildHour = 0;
        }
        else
        {
            LoadMode = config._appLoadMode;
            ServerType = config._serverType;
            IsCloseGuide = config._isCloseGuide;
            isLog = config._isLog ? config._isLog : GL_PlayerPrefs.GetBool(EPrefsKey.IsOpenLogToTool);
            IsTestUnity = config._isTestUnity;
            EnterType = config._enterType;
            AbTest = config._abTest.ToLower();
            IsSkipAD = config._isSkipAD;
            BuildApp = config._buildApp;
            Check = config._check;
            IsSikpWechatLogin = config._skipWeChatLogin;
            BuildTime = config._buildTime;
            BuildHour = config._buildHour;
            IsOpenSessId = config._isOpenSessId;
        }
        
    }


    public static bool IsLog
    {
        get
        {
#if UNITY_EDITOR
            return true;
#else
            return isLog;
#endif

        }
    }
}
