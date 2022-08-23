//2018.09.25    关林
//游戏基础类

using DataModule;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Logic.Fly;
using Logic.System.NetWork;
using SUIFW;
using UnityEngine;
using Object = System.Object;

public class GL_Game : Mono_Singleton_DontDestroyOnLoad<GL_Game>
{
    private EGameState _gameState; //游戏状态
    private EGameState _lastGameState; //  上个游戏状态

    public float _antiTime;
    [HideInInspector] 
    public bool _isInitialize = false;
    [HideInInspector]
    public bool _isStartGame = true;   //是否刚启动游戏

    public GL_SceneSwitch _sceneSwitch; //场景切换逻辑
    public FF_SignIn_Config _signInConfig;
    public FF_Task_Config _taskConfig;       //每日任务配置
    #region 初始化

    public void Awake()
    {
#if !UNITY_IOS
        Application.targetFrameRate = 40;
#else
        Application.targetFrameRate = 60;
#endif
        AppSetting.Init();
        AppData.Init();
        DataModuleManager._instance.LoadAllDataModule();
        GL_LoadAssetMgr._instance.Init();
        GL_SDK._instance.Init();

        GL_Time._instance.Init();
        GL_DateMonitoring._instance.Init();

        GL_CoreData._instance.Init();
        GL_CoreData._instance.Language = ELanguage.CN;
        GL_AD_Logic._instance.Init();
        UI_Diplomats._instance.Init();

        //提前下载资源
        GL_VersionManager._instance.StartCheckVersion();

        _sceneSwitch = new GL_SceneSwitch();
        _sceneSwitch.Init();
        //初始化飞行
        Fly_Manager._instance.Init();
        
        //禁止多指触控
        Input.multiTouchEnabled = false;
    }

    public void Init()
    {
        if (_isInitialize)
            return;

        GL_PlayerData._instance.Init();

        //一些系统逻辑的初始化
        GL_AudioPlayback._instance.Init();
        AudioSettingsInit();
        GL_SceneManager._instance.Init();

//#if GuanGuan_Test
//        UI_Diplomats._instance.ShowUI(SUIFW.SysDefine.UI_Path_GM);
//#endif
        
        //每日任务配置初始化
        _taskConfig = new FF_Task_Config();
        GL_GuideManager._instance.Init();
      
        //每日签到配置初始化
        _signInConfig = new FF_SignIn_Config();
        YS_NetLogic._instance.SearchClockin(()=>
        {
            _signInConfig.Init();
        });

        GL_NewbieSign._instance.Init();
    
        _isInitialize = true;
        DDebug.LogError("~~~Game Initialize success");
    }

    public void InitPure()
    {
        UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_PureGame);
    }

    #endregion

    #region 接口
    //进入游戏
    public void DoLevelEnter()
    {
        //GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.Main_StartGame);
        //int levelIndex = GL_PlayerData._instance.CurLevel;
        //GL_SceneManager._instance.CreateGame(levelIndex);
    }

    public void DoLevelEnter(int level)
    {
        //GL_SceneManager._instance.CreateGame(level);
    }

    //退出游戏
    public void DoLevelExit()
    {
        GameState = EGameState.GameMain;

        // if (!GL_GuideManager._instance._isGuideing)
        //     GL_AD_Logic._instance.PlayAD(GL_AD_Interface.AD_Interstitial_AllDialog);
    }

    //刷新玩法

    public void DoRestartLevel()
    {
    }



    //退出应用
    public void DoExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
    
    #region 游戏设置
    //音效设置初始化
    private void AudioSettingsInit()
    {
        GL_CoreData._instance.BGMAudioOn = GL_CoreData._instance._archivedData._isAudioBGM;
        GL_CoreData._instance.AudioOn = GL_CoreData._instance._archivedData._isAudio;
    }
    #endregion

    #endregion

    public void Update()
    {

        if (GameState == EGameState.Splash || GameState == EGameState.None)
            return;
        float dt = Time.deltaTime;
        if(GameState == EGameState.Loading)
        {
            _sceneSwitch.DoUpdate(dt);
            return;
        }
            
        GL_Input._instance.DoUpdate();
        GL_CoreData._instance.DoUpdate(dt);
        Gl_EffectManager._instance.DoUpdate(dt);
        
        if (GameState == EGameState.GameMain)
        {
            //只有主页时，监测日期
            GL_DateMonitoring._instance.DoUpdate();
        }
        //else if (GameState == EGameState.Playing)
        //{
        //    GL_SceneManager._instance.DoUpdate(dt);
        //}

        // 防沉迷在线计时
        if (GL_CoreData._instance.Anti)
        {
            _antiTime += dt;
            if (_antiTime>=1)
            {
                _antiTime = 0;
                GL_CoreData._instance.AntiTime ++;
            }
        }
    }
    public void ShowTips()
    {
        int time = (int)_antiTime/60;
        Object[] objects = { time };
        UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_TipsPage,objects);
    }

    public void FixedUpdate()
    {
    }

    private void LateUpdate()
    {
    }

    /// <summary>
    /// 每次返回主界面时调用
    /// </summary>
    private void DoGameMain()
    {
        GL_SceneManager._instance.ClearLevel();
        GL_AudioPlayback._instance.SwitchBGM(EGameState.GameMain);

        //主页需要刷新的网络请求
        if (_lastGameState != EGameState.Loading)
        {
            //loading界面已经刷新过了, 不需要重复刷新
            RefreshMainRequest();
            
            GL_PlayerData._instance.SendSystemConfig();
        }
            
    }

    //刷新主页请求
    public void RefreshMainRequest()
    {
        GL_PlayerData._instance.SendWithDrawConfig(EWithDrawType.DailyWithDraw);
        GL_PlayerData._instance.SendWithDrawConfig(EWithDrawType.Normal);
        GL_PlayerData._instance.SendWithDrawConfig(EWithDrawType.CashWithDraw);

        GL_PlayerData._instance.SendVideoRedpackConfig(EVideoRedpackType.VideoRedpack);
        GL_PlayerData._instance.SendVideoRedpackConfig(EVideoRedpackType.DragRedpack);
        GL_PlayerData._instance.SendVideoRedpackConfig(EVideoRedpackType.WithdrawRedpack);
        GL_PlayerData._instance.SendVideoRedpackConfig(EVideoRedpackType.WithdrawCoin);
        GL_PlayerData._instance.SendVideoRedpackConfig(EVideoRedpackType.WithDrawGrow);
    }

    public EGameState GameState
    {
        get { return _gameState; }

        set
        {
            if (_gameState != value)
            {
                //DDebug.LogError("~~~GameState: " + value);
                switch (value)
                {
                    case EGameState.Loading:
                        //loading时 决定当前加载什么场景

                        break;
                    //case EGameState.Settlement:
                        //GL_SceneManager._instance.StopAllCoroutines();
                        //GL_SkillSystem._instance.CloseSkill();
                        //break;
                    //case EGameState.Playing:
                        //DoGame();
                        //GL_CoreData._instance._isADPlayer = true;
                        //break;
                    case EGameState.GameMain:
                        DoGameMain();
                        break;
                    //case EGameState.Revive:
                    //    GL_AudioPlayback._instance.StopBGM();
                    //    GL_AudioPlayback._instance.Play(40);
                    //    //GL_SkillSystem._instance.CloseSkill();
                    //    break;

                    case EGameState.PureVersion:
                        
                        break;
                }

                _lastGameState = _gameState;
                _gameState = value;
                GL_GameEvent._instance.SendEvent(EEventID.GameState, new EventParam<EGameState>(value));
            }
        }
    }

    #region 网络消息通用信息填充

    [HideInInspector]
    public Net_RequesetCommon _netCommonInfo;

    public void RefreshNetCommonInfo()
    {
        UpdateCommonInfo();
        string param = GL_SDK._instance.GetCommonInfo();
        _netCommonInfo = JsonUtility.FromJson<Net_RequesetCommon>(param);
    }

    private void UpdateCommonInfo()
    {
        int appId = (int)AppSetting.BuildApp;
        var appData = DataModuleManager._instance.TableBuildAppData_Dictionary[appId];
        GL_ConstData.WeChatAppId = appData.WeChatAppID;
        GL_ConstData.PackageName = appData.PackageName;
    }

    //等待IP
    public void WaitIpCallback(string param)
    {
        if (string.IsNullOrEmpty(param))
            return;
        Net_RequesetCommon com = JsonUtility.FromJson<Net_RequesetCommon>(param);
        if (com == null)
            return;
        if (!string.IsNullOrEmpty(com.deSecret))
            _netCommonInfo.deSecret = com.deSecret;
        if (!string.IsNullOrEmpty(com.cityId))
            _netCommonInfo.cityId = com.cityId;
        if (!string.IsNullOrEmpty(com.provinceId))
            _netCommonInfo.provinceId = com.provinceId;
        if (!string.IsNullOrEmpty(com.planId))
            _netCommonInfo.planId = com.planId;
        if (!string.IsNullOrEmpty(com.shuSecret))
            _netCommonInfo.shuSecret = com.shuSecret;
        if (!string.IsNullOrEmpty(com.abTest))
            _netCommonInfo.abTest = com.abTest;

        DDebug.LogError("~~~WaitIpCallback1: " + JsonHelper.ToJson(com));
        DDebug.LogError("~~~WaitIpCallback: " + JsonHelper.ToJson(_netCommonInfo));
    }
    #endregion
}