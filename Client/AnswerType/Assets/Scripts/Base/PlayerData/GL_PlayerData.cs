using DataModule;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Logic.System.NetWork;
using SUIFW;
using SUIFW.Diplomats.Common;
using UnityEngine;

public class GL_PlayerData : Singleton<GL_PlayerData>
{
    private SWeChatArchiveInfo _weChatArchiveInfo
    {
        set => GL_CoreData._instance._archivedData._weChatInfo = value;
        get => GL_CoreData._instance._archivedData._weChatInfo;
    }

    //系统信息
    private Net_CB_SystemConfig _systemConfig;

    private Action _loginWaitCallback; //登陆等待回调
    private Action _loginCallback; //登陆回调

    public DateTime _strengthRefreshTime; //系统信息刷新时间

    public void Init()
    {
        InitVideoRedpack();
    }


    //首次启动时刷新的消息
    private void FirstRefresh()
    {
        SendSystemConfig();
        GL_Game._instance.RefreshMainRequest();
        SendWithDrawConfig(EWithDrawType.Clockin);
        SendWithDrawConfig(EWithDrawType.MoneyPool);

        GetMilestoneTaskConfig();
        GL_GuideManager._instance.CheckGuideConfig();
        GetMilestoneConfig();
        GetPlayAdConfig();
        //if (!GL_CoreData._instance.AbTest)
        {
            SendWithDrawConfig(EWithDrawType.TipsPage, (() =>
            {
                if (_withDrawTarget == null || _withDrawTarget.Count == 0) 
                    return;
                if (!_withDrawTarget.ContainsKey(EWithDrawType.TipsPage))
                    return;
                    
                if (_withDrawTarget[EWithDrawType.TipsPage].withDrawLimit>0)
                {
                    GL_SDK._instance.CoolDown(_withDrawTarget[EWithDrawType.TipsPage].withDrawLimit);
                }
              
            }));
        }
    }

    public Net_CB_SystemConfig SystemConfig
    {
        get { return _systemConfig; }


        set
        {
            if (value != _systemConfig)
            {
                _systemConfig = value;
                RefreshStrengthDateTime();
            }
        }
    }
    
    #region 广告CPM播放

    /// <summary>
    /// 播放激励视频
    /// </summary>
    public int _idiomConjCoinReward
    {
        set
        {
            GL_CoreData._instance._archivedData._settlementRewardIndex = value;
            GL_CoreData._instance.SaveData();
        }
        get
        {
            return GL_CoreData._instance._archivedData._settlementRewardIndex;
        }
    }
    /// <summary>
    /// 播放插屏
    /// </summary>
    public int _dialogCoinReward
    {
        set
        {
            GL_CoreData._instance._archivedData._dialogTimes = value;
            GL_CoreData._instance.SaveData();
        }
        get
        {
            return GL_CoreData._instance._archivedData._dialogTimes;
        }
    }
    
    public bool IsPlayIdiomConjCoinRewardAD(bool isReset,bool isAddNumber = true)
    {
        if (AppConfig != null)
        {
            if (AppConfig.isPassive == 1) //隐藏，直接不播放视频
            {
                return false;
            }
        }
        
        if (isReset)
        {
            _idiomConjCoinReward = 0;
            return true;
        }
        
        int divisor;

        //if(GL_CoreData._instance.AbTest || CurLevel <= 21)
        if (CurLevel <= 21)
        {
            //A版本. 或者21关以下 分层
            if (GL_CoreData._instance._adECPM <= 0)
            {
                divisor = 5;
            }
            else if (GL_CoreData._instance._adECPM < 25)
            {
                divisor = 3;
            }
            else if (GL_CoreData._instance._adECPM >= 25 && GL_CoreData._instance._adECPM < 50)
            {
                divisor = 4;
            }
            else
            {
                divisor = 5;
            }
        }
        else
        {
            //B版21关以上固定4次
            divisor = 4;
        }

        
        
        if(isAddNumber)
            _idiomConjCoinReward++;
        //DDebug.Log("@@@@@@@@@@" + _normalRewardCount);
        // DDebug.LogError("***** 激励视频广告当前累计次数："+ _idiomConjCoinReward + " +  ECPM:" + GL_CoreData._instance._adECPM
        // +"  +  播放需要累计次数："+divisor);
        //bool isPlay = _idiomConjCoinReward % divisor == 0;
        bool isPlay = _idiomConjCoinReward >= divisor;
        // _idiomConjCoinReward = isPlay ? 0 : _idiomConjCoinReward;
        return isPlay;
    }

  

    public bool IsPlayDialog(bool isReset, bool isAddNumber = true)
    {
        if (isReset)
        {
            _dialogCoinReward = 0;
            return true;
        }
        
        int divisor;
        // if (GL_CoreData._instance.AbTest)
        //     divisor = 4;
        // else
           

      
        if (GL_CoreData._instance._adECPM<=0)
        {
            divisor = 5;
        }
        else if (GL_CoreData._instance._adECPM<25)
        {
            divisor = 3;
        }
        else if (GL_CoreData._instance._adECPM>=25 && GL_CoreData._instance._adECPM<50)
        {
            divisor = 4;
        }
        else
        {
            divisor = 5;
        }
        if(isAddNumber)
            _dialogCoinReward++;
        //DDebug.Log("@@@@@@@@@@" + _normalRewardCount);
        //bool isPlay = _idiomConjCoinReward % divisor == 0;
        bool isPlay = _dialogCoinReward >= divisor;
        _dialogCoinReward = isPlay ? 0 : _dialogCoinReward;
        return isPlay;
    }


    #endregion

    #region 登陆时 刷新必要数据

    //登陆时需要等待的必须数据
    public void LoginWaitData(Action action)
    {
        _loginCallback = action;
        LoginGuest(() =>
        {
            //登陆完成
            //刷新所有必要数据 
            FirstRefresh();
            MethodExeTool.StartCoroutine(GetAllConfig());
        });
    }

    private IEnumerator GetAllConfig()
    {
        float time = 0;
        bool set = true;
        do
        {
            time += Time.deltaTime;
            if (time > 5 ||
                (SystemConfig != null
                && GetWithDrawConfig(EWithDrawType.Normal) != null
                && GetWithDrawConfig(EWithDrawType.DailyWithDraw) != null
                && _milestoneTaskConfig != null
                && _milestoneConfig != null)
                )
            {
                set = false;
                break;
            }

            yield return null;
        } while (set);

        _loginCallback?.Invoke();
    }


    #region 登陆

    private void LoginGuest(Action action)
    {
        _loginWaitCallback = action;
        //检测是否需要登陆
        if (string.IsNullOrEmpty(UserSecret))
        {
            GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.FirstSendLogin);
            //本地没有UserSecret需要登陆
            MethodExeTool.Loop(SendLoginGuest, 5f, -1);
        }
        else
        {
            //直接刷新数据
            MethodExeTool.Loop(SendSystemConfig, 5f, -1);
        }
    }

    public void SendLoginGuest()
    {
        Net_RequesetCommon com = new Net_RequesetCommon();
        GL_ServerCommunication._instance.Send(Cmd.LoginGuest, JsonHelper.ToJson(com), CB_LoginGuest);
    }

    private void CB_LoginGuest(string json)
    {
        Net_CB_LoginGuest msg = JsonUtility.FromJson<Net_CB_LoginGuest>(json);
        if (msg == null)
            return;
        GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.LoginResult);
        MethodExeTool.CancelInvoke(SendLoginGuest);
        UserSecret = msg.usersecret;
        GL_SDK._instance.SetUserSecret(msg.usersecret);
        //检测是否微信登陆
        if (!string.IsNullOrEmpty(msg.nickname))
        {
            //刷新微信数据
            _weChatArchiveInfo = new SWeChatArchiveInfo(msg);
        }
        MethodExeTool.Loop(SendSystemConfig, 5f, -1);
    }

    private Action<int> _appConfigCallback;

    public Net_CB_AppConfig AppConfig;
    
    public void GetAppConfig(Action<int> action)
    {
        GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.GetAppConfig);
        _appConfigCallback = action;
        MethodExeTool.Loop(SendAppConfig, 5f, -1);
    }

    private void SendAppConfig()
    {
        Net_RequesetCommon com = new Net_RequesetCommon();
        GL_ServerCommunication._instance.Send(Cmd.AppConfig, JsonHelper.ToJson(com), CB_AppConfig);
    }

    private void CB_AppConfig(string json) 
    {
        Net_CB_AppConfig msg = JsonUtility.FromJson<Net_CB_AppConfig>(json);
        if (msg == null)
            return;
        GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.AppConfigResult);
        MethodExeTool.CancelInvoke(SendAppConfig);
        AppConfig = msg;
        GL_CoreData._instance._archivedData.checker = msg.isNotice;
        GL_CoreData._instance.SaveData();
        _appConfigCallback?.Invoke(msg.isClean);
    }

    #endregion

    #region 刷新系统配置

    private Action _actionSendSystemConfig;
    
    /// <summary>
    /// 刷新系统配置
    /// </summary>
    public void SendSystemConfig()
    {
        Net_RequesetCommon com = new Net_RequesetCommon();
        GL_ServerCommunication._instance.Send(Cmd.SystemConfig, JsonHelper.ToJson(com), CB_SystemConfig);
    }

    private void CB_SystemConfig(string json)
    {
        Net_CB_SystemConfig msg = JsonUtility.FromJson<Net_CB_SystemConfig>(json);
        if (msg == null)
            return;

        //数据刷新后面在做
        SystemConfig = msg;
   
        if (_loginWaitCallback != null)
        {
            MethodExeTool.CancelInvoke(SendSystemConfig);
            _loginWaitCallback.Invoke();
            _loginWaitCallback = null;
        }
    }

    public void GetSystemConfig(Action action = null)
    {
        _actionSendSystemConfig = action;
        Net_RequesetCommon com = new Net_RequesetCommon();
        GL_ServerCommunication._instance.Send(Cmd.SystemConfig, JsonHelper.ToJson(com), CB_GetSystem);
    }

    private void CB_GetSystem(string json)
    {
        Net_CB_SystemConfig msg = JsonUtility.FromJson<Net_CB_SystemConfig>(json);
        if (msg == null)
            return;

        //数据刷新后面在做
        SystemConfig = msg;
     
        _actionSendSystemConfig?.Invoke();
        _actionSendSystemConfig = null;
    }

    #endregion

    #endregion

    #region 云控配置

    private AppControl _appControlConfig = new AppControl();

    public AppControl AppControlConfig
    {
        get => _appControlConfig;
        set => _appControlConfig = value;
    }

    private Action _actionAppControl;

    public void GetAppControl(Action action = null)
    {
        _actionAppControl = action;
        MethodExeTool.Loop(GetAppControlConfig,5f,-1);
    }

    public void GetAppControlConfig()
    {
        Net_RequesetCommon com = new Net_RequesetCommon();
        GL_ServerCommunication._instance.Send(Cmd.AppControl, JsonUtility.ToJson(com), CB_AppControl);
    }

    private void CB_AppControl(string json)
    {
        AppControl msg  = JsonUtility.FromJson<AppControl>(json);
        if (msg == null)
            return;
        MethodExeTool.CancelInvoke(GetAppControlConfig);
        AppControlConfig = msg;
        _actionAppControl?.Invoke();
        _actionAppControl = null;
    }
    
    #endregion

    #region 提现储存池配置

    
    /// <summary>
    /// 需要登录天数
    /// </summary>
    private List<int> bankDay = new List<int>()
    {
        4,7,15,30
    };
        
    /// <summary>
    /// 倍数
    /// </summary>
    private List<float> multiple = new List<float>()
    {
        1,3,10,30
    };

    public void NewBankConfig()
    {
        BankConfig = new BankConfig();
        BankConfig.nowDay = 1;
        BankConfig.nowMoney = 0;
        BankConfig.bankConfig = new List<WithDrawWaitConfig>();
        for (int i = 0; i < multiple.Count; i++)
        {
            WithDrawWaitConfig config = new WithDrawWaitConfig();
            config.multiple = multiple[i];
            config.targetDayCount = bankDay[i];
            config.canWithDraw = true;
            BankConfig.bankConfig.Add(config);
        }
        GL_CoreData._instance.SaveData();
    }


    public BankConfig BankConfig
    {
        get { return GL_CoreData._instance._archivedData._bankConfig; }
        set
        {
            GL_CoreData._instance._archivedData._bankConfig = value;
        }
    }

  
    /// <summary>
    /// 当前选择的档位
    /// </summary>
    private float _waitWithDraw;

    public float WaitWithDraw
    {
        get => _waitWithDraw;
        set => _waitWithDraw = value;
    }
    
    #endregion
    
    #region 微信

    /// <summary>
    /// 是否登陆微信
    /// </summary>
    public bool IsLoginWeChat()
    {
        bool result = false;
        if (_weChatArchiveInfo != null)
            if (!string.IsNullOrEmpty(_weChatArchiveInfo._name))
                result = true;

        return result;
    }

    /// <summary>
    /// 退出微信
    /// </summary>
    public void LogOut()
    {
        _weChatArchiveInfo = null;
    }

    /// <summary>
    /// 微信昵称
    /// </summary>
    public string WeChatName
    {
        get
        {
            if (_weChatArchiveInfo == null)
                return string.Empty;
            return _weChatArchiveInfo._name;
        }
    }


    public string WeChatIcon
    {
        get
        {
            if (_weChatArchiveInfo == null)
                return string.Empty;
            return GL_Tools.GetMd5String(_weChatArchiveInfo._icon);
        }
    }

    /// <summary>
    /// 邀请码
    /// </summary>
    public string InvitationCode
    {
        get
        {
            if (_weChatArchiveInfo == null)
                return string.Empty;
            return _weChatArchiveInfo._invitationCode;
        }
    }

    //邀请链接
    public string InvitationUrl
    {
        get
        {
            if (_weChatArchiveInfo == null)
                return string.Empty;
            return _weChatArchiveInfo._invitationUrl;
        }
    }

    //头像要做 下载后本地缓存
    //获取微信头像
    public void GetWeChatIcon(Action<Sprite> action)
    {
        if (_weChatArchiveInfo == null)
        {
            action.Invoke(null);
            return;
        }

        GL_ServerCommunication._instance.GetTexturePic(_weChatArchiveInfo._icon, (t) =>
        {
            Sprite s = Sprite.Create(t, new Rect(0, 0, t.width, t.height), Vector2.one / 2f);
            action.Invoke(s);
        });
    }

    //微信登陆成功回调
    public void CB_WeChatLoginSuccess(string param)
    {
        Net_CB_WeChatLogin msg = JsonHelper.FromJson<Net_CB_WeChatLogin>(param);
        if (msg == null)
            return;

        if (_weChatArchiveInfo == null)
        {
            // GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.WechatLogin);
        }

        _weChatArchiveInfo = new SWeChatArchiveInfo(msg);
    }

    #endregion

    #region 广告播放配置

    public Net_CB_PlayAD CbPlayAd;
    private Action _actionPlayAd;
    public void GetPlayAdConfig(Action action=null)
    {
        _actionPlayAd = action;
        Net_RequesetCommon com = new Net_RequesetCommon();
        GL_ServerCommunication._instance.Send(Cmd.PlayAd, JsonUtility.ToJson(com), CB_PlayAdConfig);
    }

    private void CB_PlayAdConfig(string json)
    {
        Net_CB_PlayAD msg = JsonUtility.FromJson<Net_CB_PlayAD>(json);
        if (msg == null && JsonUtility.ToJson(CbPlayAd) != JsonUtility.ToJson(msg))
            return;
        CbPlayAd = msg;
        if (_actionPlayAd!=null)
        {
            _actionPlayAd?.Invoke();
            _actionPlayAd = null;
        }
    }

    #endregion
    
    #region 关卡数据

    public int MaxLevel = 1000;

    public int CurLevel
    {
        get
        {
            if (SystemConfig != null)
                return SystemConfig.userLevel;
            else
                return GL_CoreData._instance._archivedData._levelIndex;
        }
        set
        {
            if ( SystemConfig!=null)
            {
                SystemConfig.userLevel = value;
            }
            GL_CoreData._instance._archivedData._levelIndex = value;
          
        }
    }

    public int AnswerCount
    {
        get
        {
            return GL_CoreData._instance._archivedData._answerCount;
        }
        set
        {
            GL_CoreData._instance._archivedData._answerCount = value;
            GL_CoreData._instance.SaveData();
        }
    }
    #endregion

    #region 货币数据

    /// <summary>
    /// 金币
    /// </summary>    
    public int Coin
    {
        get { return SystemConfig.coupon; }
        set 
        {
            SystemConfig.coupon = value;
        }
    }

    /// <summary>
    /// 根据金币计算的人民币数量
    /// </summary>
    public float Coin_RMB
    {
        get { return (Coin / 1000) / 100.0f; }
    }


    public int UserDayLevel
    {
        get
        {
            return _systemConfig.userDayLevel;
        }
        set
        {
            _systemConfig.userDayLevel = value;
            if (_systemConfig.userDayLevel < 0)
                _systemConfig.userDayLevel = 0;
        }
    }
    
    /// <summary>
    /// 假现金
    /// </summary>
    public float Bogus
    {
        get { return SystemConfig.bogus; }
        set { SystemConfig.bogus = (int) value; }
    }

    //假现金换算后数值
    public float Bogus_Convert
    {
        get { return Bogus / 100f; }
    }

    #endregion

    #region 体力

    public Net_CB_SearchEnergy EnergyConfig;

    /// <summary>自动添加体力上限</summary>
    public int MaxEnergy => EnergyConfig == null ? 0 : EnergyConfig.upperLimit;

    /// <summary>体力间隔总时长（S）</summary>
    public int EnergyInterval => EnergyConfig == null ? 0 : EnergyConfig.strengthInterval;

    /// <summary>大量体力领取次数</summary>
    public int BigEnergyLimit => EnergyConfig == null ? 0 : EnergyConfig.largeStrengthLimit;

    /// <summary>小体力领取次数</summary>
    public int SmallEnergyLimit => EnergyConfig == null ? 0 : EnergyConfig.fewStrengthLimit;


    private int _maxStrength = 25;

    /// <summary>
    /// 体力
    /// </summary>
    public int Strength
    {
        get
        {
            if (SystemConfig.strength > _maxStrength)
                SystemConfig.strength = _maxStrength;

            return SystemConfig.strength;
        }
        set
        {
            if (SystemConfig != null && value != SystemConfig.strength)
            {
                //体力累积上限25
                if (value > _maxStrength)
                    value = _maxStrength;

                SystemConfig.strength = value;
                RefreshStrengthDateTime();
            }

        }
    }

    
    
    //倒计时结束,增加体力
    public void AddStrength()
    {
        DDebug.LogError("~~~倒计时加体力");
        Strength += 1;
        _strengthRefreshTime = DateTime.MinValue;
        RefreshStrengthDateTime();
    }


    public void RefreshStrengthDateTime()
    {
        if (EnergyConfig == null)
            _strengthRefreshTime = DateTime.MinValue;

        if (Strength >= MaxEnergy)
        {
            _strengthRefreshTime = DateTime.MinValue;
        }
        else if (_strengthRefreshTime == DateTime.MinValue)
        {
            _strengthRefreshTime = DateTime.Now;
        }
    }

    #endregion

    #region 每日签到

    // public Net_CB_SignInConfig SignInConfig;

    #endregion

    #region 每日打卡
    /// <summary>
    /// 每日打卡配置
    /// </summary>
    public Net_CB_ClockinConfig SigNetCbClockinConfig;
    

    /// <summary>
    /// 打卡签到
    /// </summary>
    /// <param name="action"></param>
    public void ClockInReport(Action action=null)
    {
        Net_RequesetCommon req = new Net_RequesetCommon();
        GL_ServerCommunication._instance.Send(Cmd.Clockin, JsonHelper.ToJson(req) ,(delegate(string json)
        {
            action?.Invoke();
        }));
    }
    
    #endregion

    #region 每日任务

    public Net_CB_TaskConfig TaskConfig;

    #endregion

    #region 里程碑
    
    private Action _milestoneConfigCallback;

    public Net_CB_MilestoneConfigList _milestoneConfig;
    //获取里程碑信息
    public void GetMilestoneConfig(Action action = null)
    {
        _milestoneConfigCallback = action;
        Net_RequesetCommon com = new Net_RequesetCommon();
        GL_ServerCommunication._instance.Send(Cmd.MilestoneConfig, JsonUtility.ToJson(com), CB_MilestoneConfig);
    }

    private void CB_MilestoneConfig(string json)
    {
        Net_CB_MilestoneConfigList msg = JsonUtility.FromJson<Net_CB_MilestoneConfigList>(json);
        if (msg == null && JsonUtility.ToJson(_milestoneConfig) != JsonUtility.ToJson(msg))
            return;
        _milestoneConfig = msg;
        _milestoneConfigCallback?.Invoke();
        _milestoneConfigCallback = null;
    }

    //计算当前的里程碑
    public int GetMilestoneInfo(ref int lastLevel)
    {
        int index = -1;
        //Net_CB_MilestoneInfo result = null;
        if (_milestoneConfig != null)
        {
            for (int i = 0; i < _milestoneConfig.mileposts.Count; i++)
            {
                if(_milestoneConfig.mileposts[i].status != 1)
                {
                    index = i;
                    //result = _milestoneConfig.mileposts[i];
                    if (i == 0)
                        lastLevel = _milestoneConfig.lastGroupLevel;
                    else
                        lastLevel = _milestoneConfig.mileposts[i - 1].level;
                    break;
                }
            }
        }
        return index;
    }

    private Action<Net_CB_DrawMilestone> _drawMilestoneCallback;

    //领取里程碑奖励
    public void DrawMilestone(int id, Action<Net_CB_DrawMilestone> action)
    {
        _drawMilestoneCallback = action;

        Net_DrawMilestone msg = new Net_DrawMilestone();
        msg.milepostId = id;
        GL_ServerCommunication._instance.Send(Cmd.MilestonePost, JsonUtility.ToJson(msg), CB_DrawMilestone);
    }

    private void CB_DrawMilestone(string json)
    {
       
        Net_CB_DrawMilestone msg = JsonUtility.FromJson<Net_CB_DrawMilestone>(json);
        if (msg == null)
            return;

        _drawMilestoneCallback?.Invoke(msg);
        //_drawMilestoneCallback = null;
    }

    #endregion

    #region 按键冷却

    public bool ClickBtn(float timer,int second)
    {
        if ((Time.time - timer) > second)
        {
            return true;
        }
        return false;
    }

    #endregion

    #region 大生产活动

    #region 生产配置

    /// <summary>
    /// 大生产配置
    /// </summary>
    public Net_CB_ProduceConfig NetCbProduceConfig;
    private Action _actionProduceConfig;

    /// <summary>
    /// 查询生产配置信息
    /// </summary>
    /// <param name="action"></param>
    public void GetProduceConfig(Action action = null)
    {

        
        _actionProduceConfig = action;
        Net_RequesetCommon com = new Net_RequesetCommon();
        GL_ServerCommunication._instance.Send(Cmd.ProduceConfig, JsonUtility.ToJson(com), CB_ProduceConfig);
    }
    private void CB_ProduceConfig(string json)
    {
        Net_CB_ProduceConfig msg = JsonUtility.FromJson<Net_CB_ProduceConfig>(json);
        if (msg == null && JsonUtility.ToJson(NetCbProduceConfig) != JsonUtility.ToJson(msg))
            return;
        NetCbProduceConfig = msg;
        _actionProduceConfig?.Invoke();
        _actionProduceConfig = null;
    }

    public int ProduceTime
    {
        get {return Convert.ToInt32(NetCbProduceConfig.time); }
    }

    #endregion

    #region 提现
    
    private Action _actionProduceWithDraw;
    public void ProduceWithdraw(Action action = null)
    {


        _actionProduceWithDraw= action;
        Net_RequesetCommon com = new Net_RequesetCommon();
        GL_ServerCommunication._instance.Send(Cmd.ProduceWithDraw, JsonUtility.ToJson(com), CB_ProdecuWithDraw);
    }
    
    private void CB_ProdecuWithDraw(string param)
    {
        EWithDrawType _eWithDrawType = EWithDrawType.Normal;
        var obj = new object[]
        {
            NetCbProduceConfig.money/100f,
            _eWithDrawType
        };
        UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_WithdrawSuccess, obj);
        
        _actionProduceWithDraw?.Invoke();
        _actionProduceWithDraw = null;
    }
    #endregion

    #region 本月工资条接口

    /// <summary>
    /// 本月工资条接口
    /// </summary>
    public Net_CB_WithDrawHis NetCbWithDrawHis;
    private Action _actionProduceWithDrawHis;
    
    
    /// <summary>
    /// 查询本月工资条配置
    /// </summary>
    /// <param name="action"></param>
    public void GetProduceWithDrawHis(Action action = null)
    {
        _actionProduceWithDrawHis = action;
        Net_RequesetCommon com = new Net_RequesetCommon();
        GL_ServerCommunication._instance.Send(Cmd.ProduceWithDrawHis, JsonUtility.ToJson(com), CB_ProduceDrawHis);
    }
    private void CB_ProduceDrawHis(string json)
    {
        Net_CB_WithDrawHis msg = JsonUtility.FromJson<Net_CB_WithDrawHis>(json);
        if (msg == null && JsonUtility.ToJson(NetCbWithDrawHis) != JsonUtility.ToJson(msg))
            return;
        NetCbWithDrawHis = msg;
        _actionProduceWithDrawHis?.Invoke();
        _actionProduceWithDrawHis = null;
    }

    

    #endregion

    #region 成员前一天的提现金额
    /// <summary>
    /// 成员前一天的提现金额
    /// </summary>
    public Net_CB_MemberPayslip NetCbMemberPayslip;
    private Action _actionProducePayslip;

    
    /// <summary>
    /// 查询成员前一天的提现金额
    /// </summary>
    /// <param name="action"></param>
    public void GetProducePayslip(Action action = null)
    {

        
        _actionProducePayslip = action;
        Net_RequesetCommon com = new Net_RequesetCommon();
        GL_ServerCommunication._instance.Send(Cmd.ProducePayslip, JsonUtility.ToJson(com), CB_ProducePayslip);
    }
    private void CB_ProducePayslip(string json)
    {
        Net_CB_MemberPayslip msg = JsonUtility.FromJson<Net_CB_MemberPayslip>(json);
        if (msg == null && JsonUtility.ToJson(NetCbMemberPayslip) != JsonUtility.ToJson(msg))
            return;
        NetCbMemberPayslip = msg;
        _actionProducePayslip?.Invoke();
        _actionProducePayslip = null;
    }
    
    
    #endregion
    
    #region 上小时收入排行

    public Net_CB_ProduceRanking NetCbProduceRanking;
    private Action _actionProduceRanking;
    /// <summary>
    /// 查询生产配置信息
    /// </summary>
    /// <param name="action"></param>
    public void GetProduceRanking(Action action = null)
    {
        _actionProduceRanking = action;
        Net_RequesetCommon com = new Net_RequesetCommon();
        GL_ServerCommunication._instance.Send(Cmd.ProduceRanking, JsonUtility.ToJson(com), CB_ProduceRanking);
    }
    private void CB_ProduceRanking(string json)
    {
        Net_CB_ProduceRanking msg = JsonUtility.FromJson<Net_CB_ProduceRanking>(json);
        if (msg == null && JsonUtility.ToJson(NetCbProduceRanking) != JsonUtility.ToJson(msg))
            return;
        NetCbProduceRanking = msg;


        
        _actionProduceRanking?.Invoke();
        _actionProduceRanking = null;
    }

    public int MinIndex(List<WithDrawRank> ranks, WithDrawRank n)
    {
        return ranks.Distinct().ToList().IndexOf(n)+1;
    }
    
    #endregion

    #region 字幕显示


    public Net_CB_Barrage _Barrage;
    private Action _actionBarrage;
       
    /// <summary>
    /// 查询本月工资条配置
    /// </summary>
    /// <param name="action"></param>
    public void GetProduceBarrage(Action action = null)
    {
        _actionBarrage = action;
        Net_RequesetCommon com = new Net_RequesetCommon();
        GL_ServerCommunication._instance.Send(Cmd.ProduceLastHour, JsonUtility.ToJson(com), CB_ProduceBarrage);
    }
    private void CB_ProduceBarrage(string json)
    {
        Net_CB_Barrage msg = JsonUtility.FromJson<Net_CB_Barrage>(json);
        if (msg == null && JsonUtility.ToJson(_Barrage) != JsonUtility.ToJson(msg))
            return;
        _Barrage = msg;
        _actionBarrage?.Invoke();
        _actionBarrage= null;
    }
    

    #endregion
    
    #endregion
    
    #region 里程碑任务配置

    private Action _milestoneTaskConfigCallback;

    /// <summary>
    /// 里程碑任务配置
    /// </summary>
    public Net_CB_MilestoneTaskConfigList _milestoneTaskConfig;

    /// <summary>
    /// 获取里程碑任务配置信息
    /// </summary>
    /// <param name="action"></param>
    public void GetMilestoneTaskConfig(Action action = null)
    {
        _milestoneTaskConfigCallback = action;
        Net_RequesetCommon com = new Net_RequesetCommon();
        GL_ServerCommunication._instance.Send(Cmd.MilestoneTaskConfig, JsonUtility.ToJson(com), CB_MilestoneTaskConfig);
    }

    private void CB_MilestoneTaskConfig(string json)
    {
        Net_CB_MilestoneTaskConfigList msg = JsonUtility.FromJson<Net_CB_MilestoneTaskConfigList>(json);
        if (msg == null && JsonUtility.ToJson(_milestoneTaskConfig) != JsonUtility.ToJson(msg))
            return;
        _milestoneTaskConfig = msg;
        _milestoneTaskConfigCallback?.Invoke();
        _milestoneTaskConfigCallback = null;
    }
    
    private Action<Net_CB_DrawMilestone> _drawMilestoneCallbackTask;
    
    /// <summary>
    /// 领取里程碑任务奖励
    /// </summary>
    /// <param name="id"></param>
    /// <param name="action"></param>
    public void DrawMilestoneTask(int id, Action<Net_CB_DrawMilestone> action)
    {
        _drawMilestoneCallbackTask = action;

        Net_DrawMilestoneTask msg = new Net_DrawMilestoneTask();
        msg.taskId = id;
        GL_ServerCommunication._instance.Send(Cmd.MilestoneTaskPost, JsonUtility.ToJson(msg), CB_DrawMilestoneTask);
    }

    private void CB_DrawMilestoneTask(string json)
    {
        Net_CB_DrawMilestone msg = JsonUtility.FromJson<Net_CB_DrawMilestone>(json);
        if (msg == null)
            return;
        _drawMilestoneCallbackTask?.Invoke(msg);
        _drawMilestoneCallbackTask = null;
    }

    #endregion

    #region 邀请配置

    /// <summary>
    /// 邀请配置
    /// </summary>
    public Net_CB_InviteConfig InviteConfig;
    private Action _actionInviteConfig;

    /// <summary>
    /// 查询邀请 配置信息
    /// </summary>
    /// <param name="action"></param>
    public void GetInviteConfig(Action action = null)
    {
        _actionInviteConfig = action;
        Net_RequesetCommon com = new Net_RequesetCommon();
        GL_ServerCommunication._instance.Send(Cmd.InviteConfig, JsonUtility.ToJson(com), CB_InviteConfig);
    }
    private void CB_InviteConfig(string json)
    {
        Net_CB_InviteConfig msg = JsonUtility.FromJson<Net_CB_InviteConfig>(json);
        if (msg == null && JsonUtility.ToJson(InviteConfig) != JsonUtility.ToJson(msg))
            return;
        InviteConfig = msg;
        _actionInviteConfig?.Invoke();
        _actionInviteConfig = null;
    }


    #region 填写邀请码
    private Action _actionInvite;
    

    #endregion
    
    
    #endregion

    #region 提现

    #region 提现配置
    /// <summary>
    /// 所有提现配置
    /// </summary>
    private Dictionary<EWithDrawType, Net_CB_WithDrawList> _withDrawConfigList = new Dictionary<EWithDrawType, Net_CB_WithDrawList>();

    private void RefreshWithDrawConfig(EWithDrawType type, string json)
    {
        Net_CB_WithDrawList msg = JsonUtility.FromJson<Net_CB_WithDrawList>(json);
        if (msg == null)
            return;
        if (_withDrawConfigList == null)
            _withDrawConfigList = new Dictionary<EWithDrawType, Net_CB_WithDrawList>();

        if(_withDrawConfigList.ContainsKey(type))
            _withDrawConfigList[type] = msg;
        else
            _withDrawConfigList.Add(type, msg);

        // if (type == EWithDrawType.Normal || type == EWithDrawType.DailyWithDraw)
            
        CalculateWithDrawTarget(type);

    }
    /// <summary>
    /// 根据类型获取对应的提现配置
    /// </summary>
    public Net_CB_WithDrawList GetWithDrawConfig(EWithDrawType type)
    {
        Net_CB_WithDrawList list;
        _withDrawConfigList.TryGetValue(type, out list);
        return list;
    }
    #endregion

    #region 提现回调
    /// <summary>
    /// 所有提现回调
    /// </summary>
    private Dictionary<EWithDrawType, Action> _withDrawCallbackList;

    private void RefreshWithDrawCallback(EWithDrawType type, Action action)
    {
        if (_withDrawCallbackList == null)
            _withDrawCallbackList = new Dictionary<EWithDrawType, Action>();

        if (action == null)
            return;
        if (_withDrawCallbackList.ContainsKey(type))
            _withDrawCallbackList[type] = action;
        else
            _withDrawCallbackList.Add(type, action);
    }

    //触发提现配置回调
    private void InvokeWithDrawCallback(EWithDrawType type)
    {
        if(_withDrawCallbackList.ContainsKey(type))
        {
            _withDrawCallbackList[type]?.Invoke();
            _withDrawCallbackList[type] = null;
        }
    }
    #endregion 

    /// <summary>
    /// 向服务器更新提现配置
    /// </summary>
    public void SendWithDrawConfig(EWithDrawType type, Action action = null)
    {
        RefreshWithDrawCallback(type, action);

        Net_WithdrawConfig com = new Net_WithdrawConfig();
        com.withDrawType = (int)type;
        GL_ServerCommunication._instance.Send(Cmd.WithDrawConfig, JsonHelper.ToJson(com), (json)=>
        {
            RefreshWithDrawConfig(type, json);
            InvokeWithDrawCallback(type);
        });
    }


    public Dictionary<EWithDrawType,Net_CB_WithDraw> _withDrawTarget = new Dictionary<EWithDrawType, Net_CB_WithDraw>(); //当前档 提现目标>
    public EWithDrawType _withDrawType;     //当前档 提现类型

    //计算当前可提现目标
    public void CalculateWithDrawTarget(EWithDrawType eWithDrawType=EWithDrawType.Normal)
    {
        Net_CB_WithDraw withDraw = null;
        _withDrawType = EWithDrawType.None;

        var list = GetWithDrawConfig( eWithDrawType);
        
        if (list != null)
        {
            foreach (var wd in list.couponWithDraws)
            {
                if (wd.withDrawLimit > 0)
                {
                    withDraw = wd;
                    _withDrawType = EWithDrawType.DailyWithDraw;
                    break;
                }
            }
        }

        if(withDraw != null )
        {
            if (_withDrawTarget==null)
                _withDrawTarget = new Dictionary<EWithDrawType, Net_CB_WithDraw>();
          
            
            if(_withDrawTarget.ContainsKey(eWithDrawType))
                _withDrawTarget[eWithDrawType] = withDraw;
            else
                _withDrawTarget.Add(eWithDrawType, withDraw);
        }
    }
    #endregion

    #region 提现条件

    
    /// <summary>
    /// 是否金币足够
    /// </summary>
    /// <param name="vaule">可提现输出金额，不可提现输出所差金币</param>
    /// <returns></returns>
    public bool IsEnoughCoin()
    {
        // CalculateWithDrawTarget(EWithDrawType.DailyWithDraw);
        if(_withDrawTarget != null)
        {
            if (Coin >= _withDrawTarget[EWithDrawType.DailyWithDraw].coupon)
            {
                return true;
            }
        }

        return false;
    } 
    

    #endregion
    
    #region 刷新每日任务配置

    private UI_IF_Main _main;
    private Action _taskConfigCallback;
    public void GetTaskConfig(Action action=null)
    {
        if (_taskConfigCallback ==null)
        {
            _taskConfigCallback = action;
        }
        Net_RequesetCommon config = new Net_RequesetCommon();
        GL_ServerCommunication._instance.Send(Cmd.TaskConfig, JsonHelper.ToJson(config), CB_TaskConfig);
    }

    private void CB_TaskConfig(string json)
    {
        Net_CB_TaskConfig msg = JsonHelper.FromJson<Net_CB_TaskConfig>(json);
        if (msg == null )
            return;
        TaskConfig = msg;
        
        _taskConfigCallback?.Invoke();
        _taskConfigCallback = null;
        
        if (_main == null)
        {
            _main = UIManager.GetInstance().GetMain();
        }
        _main.ShowTask();
        // GL_GameEvent._instance.SendEvent(EEventID.RefreshMainLimit);
    }
    private Action _moneyConfigCallback;
    public void GetDailyMoneyConfig(Action action=null)
    {
        if (_taskConfigCallback ==null)
        {
            _moneyConfigCallback = action;
        }
        Net_RequesetCommon config = new Net_RequesetCommon();
        GL_ServerCommunication._instance.Send(Cmd.TaskConfig, JsonHelper.ToJson(config), CB_TaskConfig);
    }
    
    private void CB_MoneyConfig(string json)
    {
        Net_CB_TaskConfig msg = JsonHelper.FromJson<Net_CB_TaskConfig>(json);
        if (msg == null)
            return;
        TaskConfig = msg;
        // Minus();
        
    }
    #endregion

    #region 存钱罐配置

    public Net_CB_GoldenpigConfig _goldenpig;

    #endregion

    #region 新视频红包

    #region 冷却时间

    /// <summary>
    /// 获取视频红包CD时间
    /// </summary>
    /// <returns></returns>
    public double GetVideoRedpackCD(EVideoRedpackType type)
    {
        int index = (int)type - 1;
        if (index < 0 || index >= _videoRedpackNextGetTime.Count)
            return -1;

        return _videoRedpackNextGetTime[index];
    }
    private void SetVideoRedpackCD(EVideoRedpackType type)
    {
        var data = GetVideoRedpackConfig(type);
        double time = GL_Time._instance.CalculateSeconds();
        time += data.intervalTime;

        _videoRedpackNextGetTime[(int)type - 1] = time;
        GL_CoreData._instance.SaveData();
    }
    /// <summary>
    /// 新视频红包, 冷却时间
    /// </summary>
    private List<double> _videoRedpackNextGetTime
    {
        set => GL_CoreData._instance._archivedData._videoRedpackNextGetTime = value;
        get => GL_CoreData._instance._archivedData._videoRedpackNextGetTime;
    }
    private void InitVideoRedpack()
    {
        if (_videoRedpackNextGetTime == null)
            _videoRedpackNextGetTime = new List<double>();

        string[] enumLength = System.Enum.GetNames(typeof(EVideoRedpackType));
        for (int i = _videoRedpackNextGetTime.Count; i < enumLength.Length - 1; i++)
        {
            _videoRedpackNextGetTime.Add(-1);
        }
    }
    #endregion

    #region 配置
    /// <summary>
    /// 所有提现配置
    /// </summary>
    private Dictionary<EVideoRedpackType, Net_CB_VideoRed> _videoRedpackConfigList = new Dictionary<EVideoRedpackType, Net_CB_VideoRed>();

    private void RefreshVideoRedpackConfig(EVideoRedpackType type, string json)
    {
        Net_CB_VideoRed msg = JsonUtility.FromJson<Net_CB_VideoRed>(json);
        if (msg == null)
            return;
        if (_videoRedpackConfigList == null)
            _videoRedpackConfigList = new Dictionary<EVideoRedpackType, Net_CB_VideoRed>();

        if (_videoRedpackConfigList.ContainsKey(type))
            _videoRedpackConfigList[type] = msg;
        else
            _videoRedpackConfigList.Add(type, msg);
    }
    /// <summary>
    /// 根据类型获取对应的配置
    /// </summary>
    public Net_CB_VideoRed GetVideoRedpackConfig(EVideoRedpackType type)
    {
        Net_CB_VideoRed list;
        _videoRedpackConfigList.TryGetValue(type, out list);
        return list;
    }
    #endregion

    #region 回调
    /// <summary>
    /// 回调
    /// </summary>
    private Dictionary<EVideoRedpackType, Action> _videoRedpackCallbackList;

    private void RefreshVideoRedpackCallback(EVideoRedpackType type, Action action)
    {
        if (_videoRedpackCallbackList == null)
            _videoRedpackCallbackList = new Dictionary<EVideoRedpackType, Action>();

        if (action == null)
            return;
        if (_videoRedpackCallbackList.ContainsKey(type))
            _videoRedpackCallbackList[type] = action;
        else
            _videoRedpackCallbackList.Add(type, action);
    }

    //触发回调
    private void InvokeVideoRedpackCallback(EVideoRedpackType type)
    {
        if (_videoRedpackCallbackList.ContainsKey(type))
        {
            _videoRedpackCallbackList[type]?.Invoke();
            _videoRedpackCallbackList[type] = null;
        }
    }
    #endregion 
    /// <summary>
    /// 向服务器更新视频红包配置
    /// </summary>
    public void SendVideoRedpackConfig(EVideoRedpackType type, Action action = null)
    {
        RefreshVideoRedpackCallback(type, action);

        Net_Rq_VideoRed com = new Net_Rq_VideoRed();
        com.type = (int)type;
        GL_ServerCommunication._instance.Send(Cmd.VideoRedConfig, JsonHelper.ToJson(com), (json) =>
        {
            RefreshVideoRedpackConfig(type, json);
            InvokeVideoRedpackCallback(type);
        });
    }


    #region 领取新红包奖励

    public void SendGetVideoRedpack(EVideoRedpackType type, bool isView, Action<Net_CB_VideoRedGet> action = null)
    {
      
        Net_Rq_VideoRedGet com = new Net_Rq_VideoRedGet();
        com.type = (int)type;
        com.isView = isView ? 1 : 2;

        GL_ServerCommunication._instance.Send(Cmd.VideoRedGet, JsonHelper.ToJson(com), (json) =>
        {
            Net_CB_VideoRedGet msg = JsonUtility.FromJson<Net_CB_VideoRedGet>(json);
            if (msg == null)
                return;


            //刷新冷却
            SetVideoRedpackCD(type);
            //触发回调
            action?.Invoke(msg);
            //刷新config
            SendVideoRedpackConfig(type);
        });
    }

    #endregion

    #endregion

    #region 通用玩法
    #region 配置
    /// <summary>
    /// 所有配置
    /// </summary>
    private Dictionary<EGamecoreType, Net_CB_GamecoreConfig> _gamecoreConfigList = new Dictionary<EGamecoreType, Net_CB_GamecoreConfig>();

    private void RefreshGamecoreConfig(EGamecoreType type, string json)
    {
        Net_CB_GamecoreConfig msg = JsonUtility.FromJson<Net_CB_GamecoreConfig>(json);
        if (msg == null)
            return;
        if (_gamecoreConfigList == null)
            _gamecoreConfigList = new Dictionary<EGamecoreType, Net_CB_GamecoreConfig>();

        if (_gamecoreConfigList.ContainsKey(type))
            _gamecoreConfigList[type] = msg;
        else
            _gamecoreConfigList.Add(type, msg);
    }
    /// <summary>
    /// 根据类型获取对应的配置
    /// </summary>
    public Net_CB_GamecoreConfig GetGamecoreConfig(EGamecoreType type)
    {
        Net_CB_GamecoreConfig list;
        _gamecoreConfigList.TryGetValue(type, out list);
        return list;
    }
    #endregion

    #region 回调
    /// <summary>
    /// 回调
    /// </summary>
    private Dictionary<EGamecoreType, Action> _gamecoreConfigCallbackList;

    private void RefreshGamecoreConfigCallback(EGamecoreType type, Action action)
    {
        if (_gamecoreConfigCallbackList == null)
            _gamecoreConfigCallbackList = new Dictionary<EGamecoreType, Action>();

        if (action == null)
            return;
        if (_gamecoreConfigCallbackList.ContainsKey(type))
            _gamecoreConfigCallbackList[type] = action;
        else
            _gamecoreConfigCallbackList.Add(type, action);
    }

    //触发回调
    private void InvokeGamecoreConfigCallback(EGamecoreType type)
    {
        if (_gamecoreConfigCallbackList.ContainsKey(type))
        {
            _gamecoreConfigCallbackList[type]?.Invoke();
            _gamecoreConfigCallbackList[type] = null;
        }
    }
    #endregion 
    /// <summary>
    /// 向服务器更新视频红包配置
    /// </summary>
    public void SendGamecoreConfig(EGamecoreType type, Action action = null)
    {
        RefreshGamecoreConfigCallback(type, action);

        Net_GamecoreConfig com = new Net_GamecoreConfig();
        com.mid = (int)type;
        GL_ServerCommunication._instance.Send(Cmd.GamecoreConfig, JsonHelper.ToJson(com), (json) =>
        {
            RefreshGamecoreConfig(type, json);
            InvokeGamecoreConfigCallback(type);
        });
    }


    #region 领取奖励

    public void SendGamecoreAccept(EGamecoreType type, int typeValue = 0, Action<Net_CB_GamecoreAccept> action = null)
    {
        Net_GamecoreAccept com = new Net_GamecoreAccept();
        com.mid = (int)type;
        com.type = typeValue;

        GL_ServerCommunication._instance.Send(Cmd.GamecoreAccept, JsonHelper.ToJson(com), (json) =>
        {
            Net_CB_GamecoreAccept msg = JsonUtility.FromJson<Net_CB_GamecoreAccept>(json);
            if (msg == null)
                return;

            //触发回调
            action?.Invoke(msg);
            //刷新config
            SendGamecoreConfig(type);
        });
    }

    #endregion
    #endregion

    /// <summary>
    /// 用户标识
    /// </summary>
    public string UserSecret
    {
        get
        {
            if (GL_CoreData._instance._archivedData != null)
                return GL_CoreData._instance._archivedData._userSecret;
            return string.Empty;
        }
        set
        {
            GL_CoreData._instance._archivedData._userSecret = value;
        }
    }
    

}

[Serializable]

public class SWeChatArchiveInfo
{
    public string _name;        //昵称
    public string _icon;        //头像
    public string _invitationCode;   //邀请码
    public string _invitationUrl;
    public SWeChatArchiveInfo(Net_CB_WeChatLogin info)
    {

        _name = info.nickname;
        _icon = info.icon;
        _invitationCode = info.invitationCode;
        _invitationUrl = info.invitationUrl;
    }
}
