//2018.09.30    关林
//游戏数据
//包括 表格数据
//      存档数据
//      临时数据

using System;
using System.Collections.Generic;
using DataModule;

public class GL_CoreData : Singleton<GL_CoreData>
{
    public void Init()
    {
        LoadArchivedData();
    }

    //读取存档后, 角色数据初始化
    private void Init_CharacterData()
    {
        
    }

    public void DoUpdate(float dt)
    {
    }

    #region 数据接口
    
    //增加游戏次数
    public void AddGameNormalNumber()
    {
        _archivedData._gameNumber += 1;

        SaveData();
    }

    //过去了一天
    public void TheDayPassed()
    {
    }

    #endregion
         
    #region 临时数据

    public static TableGlobalVariableData _globalData;

    public bool _isFirstGame = false; //是否第一次游戏
    public bool _isADPlayer = false; //是否检测插屏
    public float _totalGameTime; //本次游戏总时间

    public int _theOfflineEarnings = 0; //本次离线收益

    public bool _isItemMoving; //组件是否在移动

    public bool _isSave = false;

    private string _sdkAuthorization;    //sdk返回的头信息
    public string SdkAuthorization
    {
        get
        {
            if(string.IsNullOrEmpty(_sdkAuthorization))
            {
                _sdkAuthorization = GL_SDK._instance.GetAuthorization();
            }
            return _sdkAuthorization;
        }
    }
    //开始新一局游戏
    public void StartNewGame()
    {
        AddGameNormalNumber();
        //GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.Playing, _archivedData._gameNumber);
        //_curGameData = new SCurrentGameData();
        //_curGameData.InitData();
        //_adTimer = 0;
    }


    public bool Nonage
    {
        get
        {
            return _archivedData._nonage;
        }
        set
        {
            _archivedData._nonage = value;
        }
    }

    public float _adECPM
    {
        set => _archivedData._adECPM = value;
        get => _archivedData._adECPM;
    }

    private string _reqAdid = "";
    public string _adReqAdid
    {
        set => _reqAdid = value;
        get => _reqAdid;
    }


    public void SetECPM(string adEcpm)
    {
        float value = 0;
        float.TryParse(adEcpm, out value);
        _adECPM = value / 100f;
    }
    public bool IsEcpm => _adECPM > 30f;

    
    
    #endregion
    
    #region 角色数据

    /// <summary>
    /// 角色数据(金币 等级 记录等
    /// </summary>
    public ArchivedData_Character _archivedData = new ArchivedData_Character();

    private GL_FileReadWirte _fileReadWirte_Character; //文件读写


    //删除角色数据
    public void ClearData_Character()
    {
        _archivedData = new ArchivedData_Character();
        SaveData();
    }

    public void RefreshRateUsIndex(int index)
    {
        _archivedData._isRateUs = index;
        SaveData();
    }

    
    /// <summary>
    /// 资源md5
    /// </summary>
    /// <returns></returns>
    public string GetResMd5()
    {
        return GL_Tools.GetMd5Val(_fileReadWirte_Character._filePath);
    }

    //IOS 广告播放时, 刷新音效
    public void RefreshAudio(bool set)
    {
        //音效开关打开时
        if (AudioOn)
        {
            if (set)
            {
                //广告播完
                GL_AudioPlayback._instance.AudioOnOff(true);
            }
            else
            {
                //广告开始
                GL_AudioPlayback._instance.AudioOnOff(false);
            }
        }

        if (BGMAudioOn)
        {
            if (set)
            {
                //广告播完
                GL_AudioPlayback._instance.BGMOnOff(true);
            }
            else
            {
                //广告开始
                GL_AudioPlayback._instance.BGMOnOff(false);
            }
        }
    }

    //每一分钟保存一次时间
    //public void RecordMinute()
    //{
    //    _archivedData._recordMinute = GL_Time._instance.CalculateMinute();
    //    SaveData();
    //}
    /// <summary>
    /// abTest状态 是否是A包
    /// </summary>
    public bool AbTest
    {
        get
        {
            if (GL_Game._instance._netCommonInfo.abTest == "b")
                return false;
            else
                return true;
        }
    }

    public int TipsCount
    {
        get { return _archivedData.tipsCount; }
        set
        {
            _archivedData.tipsCount = value;
            SaveData();
        }
    }

    //音效开关
    public bool AudioOn
    {
        set
        {
            _archivedData._isAudio = value;
            GL_AudioPlayback._instance.AudioOnOff(value);
            SaveData();
        }
        get { return _archivedData._isAudio; }
    }

    public int NowRewardCount
    {
        get { return _archivedData._nowRewardCount; }
        set { _archivedData._nowRewardCount = value; }
    }

    //背景音乐开关
    public bool BGMAudioOn
    {
        set
        {
            _archivedData._isAudioBGM = value;
            GL_AudioPlayback._instance.BGMOnOff(value);
            SaveData();
        }
        get { return _archivedData._isAudioBGM; }
    }

    //public bool IsVibrate
    //{
    //    set
    //    {
    //        _archivedData._isVibrate = value;
    //        SaveData();
    //    }
    //    get { return _archivedData._isVibrate; }
    //}

    //语言模式！
    private ELanguage _language;

    public ELanguage Language
    {
        get { return _language; }
        set
        {
            _language = value;
            //SUIFW.LanguageMgr.GetInstance().Language = (SUIFW.Language)value;
            _archivedData._languageIndex = (int) value;
            SaveData();
            //change!
            GL_GameEvent._instance.SendEvent(EEventID.EID_ChangeLanguage, new EventParam<ELanguage>(value));
        }
    }


    public bool IsNoAds
    {
        get { return false; }
        set { _= value; }
        //get { return _archivedData._isNoAds; }
        //set
        //{
        //    if (value != IsNoAds)
        //    {
        //        _archivedData._isNoAds = value;
        //        SaveData();
        //    }
        //}
    }

    #endregion

    #region 防沉迷

    public int AntiTime
    {
        get
        {
            return _archivedData._antiTime;
        }
        set
        {
            _archivedData._antiTime = value;
            SaveData();
        }
    }

    public bool Anti
    {
        get { return _archivedData.anti; }
        set
        {
            _archivedData.anti = value;
            SaveData();
        }
    }
    

    #endregion
    
    #region 活动签到

    /// <summary>
    /// 活动签到天数
    /// </summary>
    public int ActivitySignDay
    {
        set
        {
            _archivedData.ActivitySignDay = value;
            SaveData();
        }
        get => _archivedData.ActivitySignDay;
    }

    #endregion

    #region 存档数据

    //加载存档数据
    private void LoadArchivedData()
    {
        _globalData = GameDataTable.GetTableGlobalVariableData(1);
        //角色数据
        _fileReadWirte_Character = new GL_FileReadWirte();
        _fileReadWirte_Character.Init("CharacterData");
        var data1 = (ArchivedData_Character) _fileReadWirte_Character.Read();
        if (data1 != null)
        {
            _archivedData = data1;
            Language = (ELanguage) _archivedData._languageIndex;

            if (_archivedData._appVersion == "0.0.1")
                _archivedData._appVersion = GL_SDK._instance.GetAppVersion();
        }
        else
        {
            //首次登陆 读取默认数值
            //_archivedData._moneyNumber = _globalData.MoneyNumber;

            //RecordMinute();
            _isFirstGame = true;
            _archivedData._appVersion = GL_SystemInfo._instance.AppVersion;
            DateTime dt = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            _archivedData._appFirstDate = GL_Time._instance.CalculateSeconds(dt);
            //GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.NewUser, 0);
        }

        

        Init_CharacterData();

        _totalGameTime = 0;
    }



    //保存角色数据
    public void SaveData()
    {
        DDebug.LogWarning("SaveData-----------------------");
        _isSave = true;
    }

    public void RealSaveData()
    {
        if (_isSave)
        {
            _fileReadWirte_Character.Wirte(_archivedData);
            _isSave = false;
        }
    }

    #endregion
}

#region 角色数据

[Serializable]
public class ArchivedData_Character
{
    public bool _isAudio = true; //音效开关
    public bool _isAudioBGM = true; //背景音乐开关
    public int _languageIndex; //语言设置

    public int _gameNumber; //游戏次数
    //public bool _isNoAds; //是否购买 noads
 
    //public bool _isVibrate = true;

    public int _isRateUs = -1; //是否弹出评价(-1不在弹出评价  0时, 第三关弹出.  1时第六关弹出)

    public int tipsCount = 3;
    public int _levelIndex =1 ; //当前关卡等级

    public int _dialogTimes; //插屏累计次数
    public int _settlementRewardIndex = 1 ;  //结算界面次数累计
    public float _adECPM;   //激励视频广告 ecpm
    
    public string _appVersion;  //记录首次启动版本号
    public double _appFirstDate;     //首次启动日期
    public string _userSecret;  //用户唯一标识
    
    public SWeChatArchiveInfo _weChatInfo;  //微信相关
    public SGuideArchiveInfo _guideArchiveInfo; //引导存档
    public SNewbieSignArchiveInfo _newbieSignArchiveInfo;   //新手签到存档

    //当前等级,  当前世界索引, 当前关卡索引
    public int _curLevelIndex, _worldValue, _levelValue;

    public SDateArchiveInfo _dateArchiveInfo;
    
    /// <summary>活动签到天数/// </summary>
    public int ActivitySignDay;

    public int checker; //1是审核员 2不是

    public List<double> _videoRedpackNextGetTime;   //视频红包下次领取时间

    public int _nowRewardCount =0 ;

    #region 纯净版数据储存
    public int _answerCount =0; //已答题目数量
    #endregion


    public int _antiTime = 0;

    public bool anti = false;
    
    public bool _nonage = false;


    public BankConfig _bankConfig;
    
}

#endregion

#region 当前游戏局数据

public class SCurrentGameData
{
    public float _gameTime; //当前游戏局 游戏时间

    public int _extraReviveNum; //额外复活次数
    public int _vipReviveNum; //vip复活
    public int _reviveNum; //复活次数

    public int _hpNum; //生命数量
    public int _goldNum; //金币数


    public void InitData()
    {
        _gameTime = 0;
        _reviveNum = 0;
        _vipReviveNum = 0;
        _extraReviveNum = 0;
        //_hpNum = GL_CoreData._globalData.MaxHP;

        _goldNum = 0;
        //_vipReviveNum = GL_IAP_Logic._instance._isNoAds ? 1 : 0;

        //_isPlayBGM = true;
    }

    //检查是否可以复活
    public bool CheckRevive()
    {
        //检测层数 是否可以复活
        //10层以上才可复活
        //if (_rowNum < 10)
        //    return false;

        //检测是否复活广告

        //if (!GL_AD_Logic._instance.IsAvailableAD(GL_ConstData.AD_AddHP))
        //    return false;

        //检测复活次数
        //if (_reviveNum >= GL_ConstData.ReviveNumber)
        //    return false;

        return true;
    }

    //检测额外复活
    public bool CheckExtraRevive()
    {
        //检测复活次数
        if (_extraReviveNum > 0)
        {
            _extraReviveNum -= 1;
            return true;
        }


        return false;
    }

    //检测VIP复活
    public bool CheckVipRevive()
    {
        if (_vipReviveNum > 0)
        {
            _vipReviveNum -= 1;
            return true;
        }


        return false;
    }

    /// <summary>
    /// 复活成功
    /// </summary>
    public void ChangeReviveNum()
    {
        _reviveNum += 1;
    }

    //游戏中, 临时开关背景音乐
    public void SetPlayBGM(bool set)
    {
        //_isPlayBGM = set;
        //if (set)
        //    GL_AudioPlayback._instance.BGM_Refresh();
        //else
        //    GL_AudioPlayback._instance.BGM_Stop();
    }
}

#endregion

//生涯累计数据
[Serializable]
public class SAddUpData
{
    public int spendGold; //花费金币  1
    public int login; // 登录 1

    public int _rotateDraw; // 转盘抽奖
    public int _coinDraw; // 参加金币小游戏
    public int _getAchievement; //领取成就
    public int _getDailyTaskBox;   //领取每日活跃宝箱

    public Dictionary<int, int> _purchaseSucceed = new Dictionary<int, int>();  //内购购买存档
}

