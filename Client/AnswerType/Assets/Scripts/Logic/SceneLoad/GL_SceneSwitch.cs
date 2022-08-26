//2021.7.14 关林
//场景进程

using DataModule;
using System;
using System.Collections;
using System.Collections.Generic;
using SUIFW;
using UnityEngine;
using Object = System.Object;

public class GL_SceneSwitch
{
    public enum EStatus
    {
        None,
        GDPR,
        WaitCommon,     //等待数据填充
        CheckVersion,   //检测是正式版 还是纯净版
        EnterGame,      //进入游戏
        ServerCheck,    //服务器检测(登陆 版本检测 资源下载 关键的服务器消息
        Download,       //如果有现在, 这个是下载进度
        LoginWeChat,    //登陆微信
        Init,           //游戏主逻辑加载
        GameScene,

        EnterPureVersion,   //进入纯净版游戏
    }
    public float _uiProgress;

    public float _lastProgress;  //上一阶段的进度值
    private EStatus _status = EStatus.None;
    public bool _isLoadScene;   //正在加载场景

    public EGameEnterType _enterType; //进入类型

    private bool _isNotice;     //是否确定了隐私协议
    private UI_IF_Loading _uiLoading;
    private UI_IF_Loading UILoading
    {
        get
        {
            if(_uiLoading == null)
                _uiLoading = UIManager.GetInstance().GetUI(SysDefine.UI_Path_Loading) as UI_IF_Loading;

            return _uiLoading;
        }
    }
    public void Init()
    {
        GL_Game._instance.GameState = EGameState.Loading;
        GL_SDK._instance.HideSplash();
        
        _gameScene = new GL_Scene_GameScene();

        int isAgreeGDPR = GL_PlayerPrefs.GetInt(EPrefsKey.IsAgreeGDPR);
        if(AppSetting.BuildTime > 0)
        {
            double time = GL_Time._instance.CalculateSeconds(DateTimeKind.Utc) - AppSetting.BuildTime;
            if (time >= AppSetting.BuildHour * 60 * 60)
            {
                GL_PlayerPrefs.SetInt(EPrefsKey.IsAgreeGDPR, 1);
                isAgreeGDPR = 1;
            }
        }
        //1.隐私协议
        if (isAgreeGDPR == 0)
        {
            Status = EStatus.GDPR;
            
        }
        else
        {
            Status = EStatus.WaitCommon;
        }
    }

    #region GDPR阶段

    public void ShowGDPR()
    {
        //判断是否首次登陆, 需要弹出公告
        //Action sureCallBack = () =>
        //{
        //    Status = EStatus.WaitCommon;

        //};
        UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_Notice);
        UI_Diplomats._instance.LoadingShowSlider(false, false, "");
    }

    public void AgreeGDPR()
    {
        GL_Game._instance.RefreshNetCommonInfo();
        //同意了隐私权限
        GL_PlayerPrefs.SetInt(EPrefsKey.IsAgreeGDPR, 1);
        //申请设备权限
        if(GL_CoreData._instance._isFirstGame || GL_CoreData._instance._archivedData.checker == 2)
            GL_SDK._instance.RequestAdPermissions(true);
        else
            GL_SDK._instance.RequestAdPermissions(false);

        //等待关键数据填充
        MethodExeTool.StartCoroutine(WaitCommonInfo());

        //UI_Diplomats._instance.LoadingShowSlider(true);
    }

    /// <summary>
    /// 隐私协议界面的选择
    /// </summary>
    public void SetNotice(bool set)
    {
        _isNotice = set;
        if (set)
        {
            //同意后,
            //1.判断是否勾选
            if(UILoading.IsAgreeToggle())
            {
                //勾选了,直接进入等待界面
                UI_Diplomats._instance.LoadingShowSlider(false, false, "");
                Status = EStatus.WaitCommon;
            }
            else
            {
                //没勾选,则进入登陆界面
                UI_Diplomats._instance.LoadingShowSlider(false, true, "登陆");
            }
        }
        else
        {
            UI_Diplomats._instance.LoadingShowSlider(false, true, "登陆");
        }
    }
    #endregion


    public IEnumerator WaitCommonInfo()
    {
        bool wait = true;
        float time = 0;
        do
        {
            time += Time.deltaTime;

            if (GL_Game._instance._netCommonInfo == null || string.IsNullOrEmpty(GL_Game._instance._netCommonInfo.deSecret))
            {
                //shuSecret = null, 则等待
                DDebug.LogError("~~~:" + GL_Game._instance._netCommonInfo);
            }
            else
            {
                //有shuSecret,
                //超过2秒直接进入
                if (time > 2)
                {
                    wait = false;
                }
                else
                {
                    //所有数据填充了直接进入
                    if (!string.IsNullOrEmpty(GL_Game._instance._netCommonInfo.cityId)
                        && !string.IsNullOrEmpty(GL_Game._instance._netCommonInfo.provinceId)
                        && !string.IsNullOrEmpty(GL_Game._instance._netCommonInfo.planId)
                        && !string.IsNullOrEmpty(GL_Game._instance._netCommonInfo.shuSecret))
                        wait = false;
                }
            }
            yield return null;
        } while (wait);


        if(AppSetting.EnterType == EGameEnterType.PureVersion)
        {
            CheckFCM(1);
        }
        else
        {
            //判断是否是纯净版
            GL_PlayerData._instance.GetAppConfig(CheckFCM);
        }
    }
    public void UI_OnClickWeChat()
    {
        //1.是否同意GDPR
        if(_isNotice || Status == EStatus.LoginWeChat)
        {
            if(Status == EStatus.GDPR)
            {
                //登陆
                UI_Diplomats._instance.LoadingShowSlider(false, false, "");
                Status = EStatus.WaitCommon;
            }
            else
            {
          
                //微信登陆
                if ((GL_PlayerData._instance.AppConfig != null && GL_PlayerData._instance.AppConfig.isNotice == 1)
                || !GL_PlayerData._instance.IsLoginWeChat())
                {
                    
                    OnClickWeChat();
                }
                else
                {
                    Status = EStatus.Init;
                }
            }
        }
        else
        {
            if (Status == EStatus.GDPR)
                GL_Game._instance._sceneSwitch.ShowGDPR();
        }
    }
    //public void UI_OnClickWeChat()
    //{
    //    //1.是否同意GDPR
    //    int isAgreeGDPR = GL_PlayerPrefs.GetInt(EPrefsKey.IsAgreeGDPR);

    //    if (isAgreeGDPR == 0)
    //    {
    //        if (Status == EStatus.None)
    //            GL_Game._instance._sceneSwitch.ShowGDPR();
    //    }
    //    else
    //    {
    //        if ((GL_PlayerData._instance.AppConfig != null && GL_PlayerData._instance.AppConfig.isNotice == 1)
    //            || !GL_PlayerData._instance.IsLoginWeChat())
    //        {
    //            GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.WeChatLog);
    //            OnClickWeChat();
    //        }
    //        else
    //        {
    //            Status = EStatus.Init;
    //        }
    //    }
    //}

    //检测防沉迷
    private void CheckFCM(int index)
    {
        switch (AppSetting.EnterType)
        {
            case EGameEnterType.WaitServer:
                _enterType = (EGameEnterType)index;
                break;
            default:
                _enterType = AppSetting.EnterType;
                break;
        }
        int isFCM = GL_PlayerPrefs.GetInt(EPrefsKey.IsFCM);
        //判断是否要实名认证
        if (isFCM == 0
            && GL_PlayerData._instance.AppConfig != null 
            && GL_PlayerData._instance.AppConfig.isRealname == 1)
        {
            Action action = () =>
            {
                GL_PlayerPrefs.SetInt(EPrefsKey.IsFCM, 1);
                ContinueLogin();
            };
            CheckMessage message = new CheckMessage();
            message._action = action;
            message.State = CheckState.UnCheck;
            Object[] objects = { message };
            GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.CkeckFCM);
            UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_Check, objects);
            return;
        }
        else
        {
            ContinueLogin();
        }
    }

    //继续登陆
    private void ContinueLogin()
    {
        if (_enterType == EGameEnterType.PureVersion)
        {
            GL_VersionManager._instance.StartCheckVersion();
            int isFCM = GL_PlayerPrefs.GetInt(EPrefsKey.IsFCM);
            if (AppSetting.Check && isFCM == 0)
            {
                Action action = () =>
                {
                    GL_PlayerPrefs.SetInt(EPrefsKey.IsFCM, 1);
                    MethodExeTool.StartCoroutine(WaitSplash());
                    
                };
                CheckMessage message = new CheckMessage();
                message._action = action;
                message.State = CheckState.UnCheck;
                Object[] objects = { message };
                GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.CkeckFCM);
                UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_Check, objects);
            }
            else
            {
                //纯净版
                MethodExeTool.StartCoroutine(WaitSplash());
                
            }
           
        }
        else if (_enterType == EGameEnterType.OfficialVersion)
        {
            Status = EStatus.EnterGame;
        }
    }

    public void DoUpdate(float dt)
    {
        if (_isLoadScene)
            _curScene.DoUpdate(dt);
        if (_status == EStatus.ServerCheck)
        {
            int value = UnityEngine.Random.Range(-2, 3);
            if (value <= 0 || _uiProgress > 80)
                return;

            _uiProgress += value;
        }
        else if (_status == EStatus.GameScene)
        {
            _uiProgress = _lastProgress + _curScene._loadProgress * (100 - _lastProgress) / 100f;
        }
        else if (_status == EStatus.EnterPureVersion)
        {
            int value = UnityEngine.Random.Range(-2, 8);
            if (value <= 0)
                return;

            _uiProgress += value;
            _uiProgress = Mathf.Clamp(_uiProgress, 0, 100);
            if (_uiProgress >= 100)
            {
                //假加载完成
                Status = EStatus.None;

                //GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.Login_PureVersion);
                GL_Game._instance.GameState = EGameState.PureVersion;
            }
        }
    }

    //游戏数据初始化完成
    private void GameInitOver()
    {
        //判断加载什么场景.
        Status = EStatus.GameScene;
        
    }



    //登陆成功
    private void LoginOver()
    {
        GL_VersionManager._instance.StartCheckVersion();
        //未登录微信 或  当前是审核员,则需要登陆微信
        if (GL_PlayerData._instance.AppConfig.isNotice == 1)
        {
            if (!GL_PlayerData._instance.IsLoginWeChat() || GL_CoreData._instance._isFirstGame)
            {
                //b包,  没有登陆时, 等待微信登陆
                UI_Diplomats._instance.LoadingShowSlider(false, true, "微信登录");
                Status = EStatus.LoginWeChat;
                return;
            }
        }
        else
        {
            if (!GL_PlayerData._instance.IsLoginWeChat())
            {
                //b包,  没有登陆时, 等待微信登陆
                UI_Diplomats._instance.LoadingShowSlider(false, true, "微信登录");
                Status = EStatus.LoginWeChat;
                return;
            }
        }

        //有微信信息的, 直接进入游戏
        UI_Diplomats._instance.LoadingShowSlider(true, false, "");
        Status = EStatus.Init;
    }

    #region 微信登陆

   
    public void OnClickWeChat()
    {
        if (Status == EStatus.LoginWeChat)
        {
            if (AppSetting.IsSikpWechatLogin)
            {
                SUIFW.Diplomats.Common.UI_HintMessage._.ShowMessage("跳过登录");
                var uiLoading = SUIFW.UIManager.GetInstance().GetUI(SUIFW.SysDefine.UI_Path_Loading) as UI_IF_Loading;
                uiLoading.ShowSlider(true);
                Status = EStatus.Init;
                return;
            }

            GL_SDK._instance.Login("wechat", CB_Login);
        }
    }
    
    private void CB_Login(string param)
    {
        
        var uiLoading = SUIFW.UIManager.GetInstance().GetUI(SUIFW.SysDefine.UI_Path_Loading) as UI_IF_Loading;
        DDebug.LogError("~~~SDK登陆回调: " + param);
        Net_WeChatLogin com = new Net_WeChatLogin();
        com.code = param;

        if (AppSetting.IsTestUnity)
        {
            SUIFW.Diplomats.Common.UI_HintMessage._.ShowMessage("登陆成功");
            GL_PlayerData._instance.CB_WeChatLoginSuccess(param);
            uiLoading.ShowSlider(true);
            Status = EStatus.Init;
        }
        else
        {
            GL_ServerCommunication._instance.Send(Cmd.LoginWeChat, JsonUtility.ToJson(com), (string msg) =>
            {
                if (!string.IsNullOrEmpty(msg))
                {
                    SUIFW.Diplomats.Common.UI_HintMessage._.ShowMessage("登陆成功");
                    GL_PlayerData._instance.CB_WeChatLoginSuccess(msg);
                    uiLoading.ShowSlider(true);
                    Status = EStatus.Init;
                }
                else
                {
                    GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.WeChatFails);
                    SUIFW.Diplomats.Common.UI_HintMessage._.ShowMessage("登陆失败");
                }
            });
        }
    }
    #endregion


    #region 加载场景

    public GL_SceneStateBase _curScene; //当前场景
    public GL_Scene_GameScene _gameScene;
    //加载主界面
    public void LoadGameScene(Action action = null)
    {
        _isLoadScene = true;
        _curScene?.ExitScene();
        _curScene = _gameScene;
        _gameScene.EnterScene(action);
    }
    public void LoadGameSceneComplete()
    {
        GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.LoadGameSceneComplete);   
        GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.ActiveGame);     
        GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.WecChatLogin);
        _isLoadScene = false;
        MethodExeTool.StartCoroutine(WaitSplash());
        //if (GL_CoreData._instance._isFirstGame)
        //{
        //    GL_Game._instance.GameState = EGameState.GameMain;
        //}
        //else
        //{
        //    MethodExeTool.StartCoroutine(WaitSplash());
        //}
    }

    private IEnumerator WaitSplash()
    {
        //先等待资源加载
        do
        {
            yield return null;
        } while (!GL_VersionManager._instance.IsDone);


        //float time = 0;
        //bool isAD = false;
        //do
        //{
        //    //检测是否有插屏
        //    isAD = GL_AD_Interface._instance.IsAvailableAD(EADType.Splash);
        //    if (isAD)
        //        break;
        //    yield return new WaitForSeconds(0.5f);
        //    time += 0.5f;
        //} while (time <= 2.5f);
        if(_enterType == EGameEnterType.PureVersion)
        {
            //纯净版
            Status = EStatus.EnterPureVersion;
        }
        else
        {
            GL_Game._instance.GameState = EGameState.GameMain;
        }

        //DDebug.LogError("~~~启动插屏 是否能播放成功: " + isAD);
        //if(isAD)
        //{
        //    //进入游戏后 播放一个开屏
        //    GL_AD_Logic._instance.PlayAD(GL_AD_Interface.AD_Splash_Splash);
        //}
    }

    #endregion


    public EStatus Status
    {
        get
        {
            return _status;
        }
        set
        {
            if (value != _status)
            {
                //DDebug.LogError("~~~value:" +value);
                _status = value;

                switch (value)
                {
                    case EStatus.None:
                        break;
                    case EStatus.GDPR:
                        //首次登陆游戏 打开公告板
                        ShowGDPR();
                        break;
                    case EStatus.WaitCommon:
                        AgreeGDPR();
                        break;
                    case EStatus.EnterGame:
                        //1.显示splash屏
                        //2.显示Loading
                        
                        Status = EStatus.ServerCheck;
                        //GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.StartLoadGame, "_2");
                        break;
                    case EStatus.ServerCheck:
                        //不需要加载
                        
                        //需要登陆
                        GL_PlayerData._instance.LoginWaitData(LoginOver);
                        break;
                    case EStatus.Download:
                        break;
                    case EStatus.Init:
                        GL_Game._instance.Init();
                        GameInitOver();
                        break;
                    case EStatus.GameScene:
                        _lastProgress = _uiProgress;
                        LoadGameScene(LoadGameSceneComplete);
                        break;

                    case EStatus.EnterPureVersion:
                        GL_Game._instance.InitPure();
                        //GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.StartLoadGame, "_1");
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
