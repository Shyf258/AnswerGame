//2018.08.30    关林
//枚举定义

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region App设定

public enum EAppResourceLoadMode
{
    Resource,       //常规加载
    //AssetDatabase,  //编辑器模式下 AssetDatabase加载
    AssetBundle,   //本地ab包加载
    //HttpAssetBundle,    //网络ab包下载
}

public enum EBuildApp
{
    None,
    RSDYJ = 1, //人生大赢家赚金
    BXDYJ = 2, //冰雪大赢家
    ZYXLZ = 3, //中药小郎中
    CYZDD = 4, //成语赚多多
    NJZW = 5 , //闹剧之王
}

#endregion

/// <summary>
/// 游戏阶段
/// </summary>
public enum EGameState
{
    None,
    GameMain,
    Playing,        //游戏界面
    Settlement,     //游戏结算
    Revive,         //复活
    Pause,          //暂停
    Loading,        //
    Splash,        //闪屏
    LoadSceneing,   //加载场景中

    PureVersion,   //纯净版简单处理
}

public enum EWithDrawType
{
    None = 0,
    Normal = 1, //普通提现
    CashWithDraw = 3,    //现金提现
    MoneyPool = 4,  //财神提现
    //LevelWithDraw = 5,  //关卡提现
    Clockin = 6, //打卡
    DailyWithDraw = 7,  //每日提现
    TipsPage = 8,       //广告提现机会
    WaitWithDraw = 9,
}

public enum EGameEnterType
{
    WaitServer,         //等待服务器通知
    PureVersion,        //纯净版
    OfficialVersion,    //正式版
}

public enum EServerType
{
    Test,   //测试服
    Noraml,//正式服
    Local,  //本地
}

public enum BtnState
{
    /// <summary>
    /// 已领取
    /// </summary>
    HasGet,
    /// <summary>
    /// 未完成
    /// </summary>
    UnFinish,
    /// <summary>
    /// 已完成
    /// </summary>
    Finished
}

public enum EGameModeType
{
    //HexaPuzzle = 0,  
    //ChipsMode = 1,
    Answer = 0,
}

#region 常用

public enum ELanguage
{
    EN,
    CN,
    HK,
    FR,
    DE,
    IT,
    JA,
    PT,
    ES,
    KO,
}

//方向
public enum EDirection
{
    None = -1,
    Left,
    Right,
    Up,
    Down
}
//资源类型
public enum EResourceType
{
    Audio = 1,  //音效
    Effect = 2, //特效
}

//1激励 2插屏 3原生 4banner 5开屏
//广告类型
public enum EADType
{
    None = 0,
    Reward = 1,
    Interstitial,
    Native,
    Banner,
    Splash
}

#endregion

#region Net

public enum EResponseCode
{
    Succeed = 200,    //请求成功
    TokenInvalid = 302, //Token失效
    LoginFail = 301,   //登录失败
    BlackUser = 402,    //黑名单
    WithdrawFailure = 601,  //提现失败
    TipsADS = 603, //提醒看广告
    MoneyGrow =  604, //增加现金上限
}

#endregion

#region 物品

public enum EItemType
{
    None = -1,
    /// <summary>
    /// 人民币 测试用, 服务器不返回
    /// </summary>
    RMB = 0,
    /// <summary>
    /// 金币,
    /// </summary>
    Coin = 1, 
    /// <summary>
    /// 不可提取现金 3
    /// </summary>
    Bogus = 3,      
    /// <summary>
    /// 体力 4
    /// </summary>
    Strength = 4,   
}

/// <summary>
/// 奖励来源
/// </summary>
public enum ERewardSource
{
    None = 0,
    VideoRedpack = 1,
    DragRedpack = 2,
    WithdrawRedpack = 3,
    WithdrawCoin = 4,
    WithDrawGrow = 5,
    TaskReward,         //任务
    OpenRed,            //红包
    Guide,              //新手引导
}
#endregion

#region 新手引导
public enum EGuideTriggerType
{
    None = 0,
    Server = 1, //服务器触发
    UIMain = 2,    //主页触发
    UIGame = 3,     //游戏界面
    UIGameSuccee = 4,   //结算胜利界面
    UIDailyTask = 5,    //每日任务界面
    UIWithdraw = 6,     //提现界面
    UIProduction = 7,   //全民大生产
    NewSign = 11,       //新手签到

}

public enum EGuideType
{
    None = 0,
    ClickObject = 1,    //点击UI
    Dialog = 2,         //对话
    Operate = 3,        //区域操作
    CheckUIClose = 4,   //监测UI关闭
}
#endregion

//新视频红包类型
public enum EVideoRedpackType
{
    None = 0,
    VideoRedpack = 1,
    DragRedpack = 2,
    WithdrawRedpack = 3,
    WithdrawCoin = 4,
    WithDrawGrow = 5,
    TaskReward = 6,
}

public enum EStage
{
    None,
    Stage1,
    Stage2,
    Stage3,
    Stage4,
    Stage5,
}
public enum TaskType
{
    VideoCount =1,
    Pass=2,
    TurnTable=3,
    RedRain=4,
    RedPack=5,
    Strength=6,
    FlyBox=7,
    FreeGetMoney=8,
    DailyTask=9,
    SignIn=10,
    ADClick=11,
    OnLine=12,
    NewPlayer=13,
    Pass01=14,
    Pass02=15,
    Pass03=16,
    DownLoadIdom = 17,
    DownLoadIce = 18,
    DownLoadLifeWin = 19
}


public enum CheckState
{
    /// <summary>
    /// 未完成实名认证
    /// </summary>
    UnCheck,
    /// <summary>
    /// 已通过实名认证（成年）
    /// </summary>
    Finish,
    /// <summary>
    /// 实名认证失败
    /// </summary>
    Fail,
    /// <summary>
    /// 未成年人
    /// </summary>
    Nonage,
    /// <summary>
    /// 完成提示（未成年）
    /// </summary>
    Tips
}
#region 活动

public enum EnumActivity
{
    WatchVideo = 1, //看视频
    RedPacketGroup, //红包群
    AnswerQuest, //答题
}

public enum EnumChat
{
    Family, //家族群
    Official, //官方群
}

#endregion

//通用玩法模块类型
public enum EGamecoreType
{
    Guide = 1,  //新手福利
    NewbieSign = 2,  //新手现金(688活动
    ActivitySign = 3,   //活动页签到
    PureAnswer = 4, //答题进度红包
    ActivityVideo = 5, //活动看视频
    Turntable =9, //抽奖
}