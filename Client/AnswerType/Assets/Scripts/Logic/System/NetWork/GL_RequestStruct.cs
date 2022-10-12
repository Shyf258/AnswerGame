//2021.2.4  关林
//协议的结构体

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class Net_CB_ErrorInfo
{
    public int message;
    public int status_code;
    public object debug;
}

/// <summary>
/// 通用消息体
/// </summary>
public class Net_Normal
{
    /// <summary>
    /// 状态
    /// </summary>
    public bool status { get; set; }
    /// <summary>
    /// 状态码
    /// </summary>
    public int code { get; set; }
    /// <summary>
    /// 消息
    /// </summary>
    public string message { get; set; }
    ///// <summary>
    ///// 数据
    ///// </summary>
    //public string data { get; set; }
}

#region sdk 网络通信结构
/// <summary>
/// 通用接口请求json数据
/// </summary>
public class Net_SDK_JsonData
{
    public string url;
    public string requestData;
    public string idx;
}

/// <summary>
/// 通用接口远端回调json数据
/// </summary>
public class Net_SDK_CB_JsonData
{
    public int idx;
    public string response;
}

#endregion


//通讯基础结构
public class Net_RequesetCommon
{
    public string cityId;
    public string provinceId;
    public string deSecret;
    public string userSecret;
    public string channel;
    public string pkgName;
    public string vn;
    public int vc;
    public string installTime;
    public string shuSecret;
    public string planId;
    public string abTest;
    public string sessId;
    public Net_RequesetCommon()
    {
        if (GL_Game._instance._netCommonInfo == null)
            return;
        cityId = GL_Game._instance._netCommonInfo.cityId;
        provinceId = GL_Game._instance._netCommonInfo.provinceId;
        deSecret = GL_Game._instance._netCommonInfo.deSecret;
        userSecret = GL_PlayerData._instance.UserSecret;
        channel = GL_Game._instance._netCommonInfo.channel;
        pkgName = GL_Game._instance._netCommonInfo.pkgName;
        vn = GL_Game._instance._netCommonInfo.vn;
        vc = GL_Game._instance._netCommonInfo.vc;
        installTime = GL_Game._instance._netCommonInfo.installTime;
        shuSecret = GL_Game._instance._netCommonInfo.shuSecret;
        planId = GL_Game._instance._netCommonInfo.planId;
        abTest = GL_Game._instance._netCommonInfo.abTest;
    }
}

//基础奖励回调
public class Net_CB_Reward
{
    public List<Rewards> rewards;
}

[Serializable]
public class Rewards
{
    public int type;
    public int num;
}
#region 登录

/// <summary>
/// 游客登录回调
/// </summary>
public class Net_CB_LoginGuest : Net_CB_WeChatLogin
{
    public int channel; //渠道号
    public string usersecret;   //token
}


public class Net_WeChatLogin : Net_RequesetCommon
{
    public string code;
    public string wxAppId = GL_ConstData.WeChatAppId;
}

/// <summary>
/// 微信登陆回调
/// </summary>
public class Net_CB_WeChatLogin
{
    public string nickname; //昵称
    public string icon;     //头像
    public string invitationCode;   //邀请码
    public string invitationUrl;
}
/// <summary>
/// 应用配置信息回调
/// </summary>
public class Net_CB_AppConfig
{
    public int isClean; //是否纯净版 1.是 2.否 
    public int isNotice; //是否是审核员 1. 是 2否
    public int isLogout;  //是否可注销 1.可注销 2.不可注销
    public int isRealname; //是否实名校验 1.实名 2.不需要
    /// <summary>隐藏被动视频 1.隐藏 2.不隐藏</summary>
    public int isPassive;
}

/// <summary>
/// 广告播放配置
/// </summary>
public class Net_CB_PlayAD
{
    public int hasopen;
    public int induceRate; //万分之几概率诱导点击 随机分子
    public long ecpm =0 ; //ecpm价格
    public int waitTime =0;
}

#endregion

#region 系统配置

/// <summary>
/// 系统通讯回调
/// </summary>
public class Net_CB_SystemConfig
{
    public int coupon;  //红包卷
    public int strength;    //体力
    public int userLevel;   //用户等级
    public int bogus;       //假的现金
    //public int cash;        //奖金池现金, 废弃
    public int viewAds;     //当天已看广告次数
    //public int dcoupon;     //提现劵
    public int userDayLevel; //当天通关数
}
#endregion

#region 云控配置

// public class Net_Appcontrol : Net_RequesetCommon
// {
//
// }

public class AppControl
{
    /// <summary>
    /// 是否审核员 1.是 2.否
    /// </summary>
    public int isNotice = 1;
}

#endregion

#region 提现

//提现配置
public class Net_WithdrawConfig : Net_RequesetCommon
{
    public int withDrawType = 1;   
}

/// <summary>
/// 金币
/// </summary>
public class Net_CB_WithDrawList
{
    public List<Net_CB_WithDraw> couponWithDraws;
    public int arpu;
    public int viewNum;
}

//提现配置
public class Net_WithDraw : Net_RequesetCommon
{
    //1.红包券 2.奖池 3.现金提现 4.新年财神 5.关卡提现 6.打卡提现 7.每日刷新提现
    public int withDrawType = 1;
    public int withDrawId;      //提现id
    /// <summary>
    /// 进入提现类型 1.提现页点击提现 2.直接提现 3.看视频增加上限
    /// </summary>
    public int type ;
    /// <summary>
    /// 视频ecpm
    /// </summary>
    public int ecpm ;
    /// <summary>
    /// 视频请求唯一id
    /// </summary>
    public string reqAdId;
}

[Serializable]
public class Net_CB_WithDraw
{
    public int id;          //提现的id
    public int money;       //提现的金额
    public int coupon;      //消耗的红包券数量
    public int viewAdTimes; //需要看广告次数
    public int level;       //需要关卡数
    //public int needFriend;  //需要好友数
    public int needDcoupon; //需要提现劵
    public int withDrawLimit;//剩余可提现次数
    public int needPosition; //需要官职
    public int arpu;
    public int needAd = 0;    //解锁提现需要的看广告次数
    public int ecpm = 0;
    public int needDay = 0;
}

/// <summary>
/// 提现记录
/// </summary>
public class Net_CB_WithdrawRecordConfig
{
    public List<WithdrawRecord> withDraws;
}

public class WithdrawRecord
{
    public int id;
    /// <summary>提现的金额（分）</summary>
    public int withDrawNum;
    /// <summary>提现时间</summary>
    public long withDrawTime;
    /// <summary>1.审核中 2.提现成功</summary>
    public int status;
}

public class Net_CB_WithDrawTipsData
{
    public int needView;
    public string notifyMessage;
    public int money;
}

/// <summary>
/// 登录增幅配置
/// </summary>
public class Net_CB_WithDrawGrowConfig
{
    /// <summary>///红包剩余金额（单位分） /// </summary>
    public int money;
    /// <summary>///视频增幅比率（小数） /// </summary>
    public double videoGrowth;
    /// <summary>///登录增幅比率（小数） /// </summary>
    public double loginGrowth;
    /// <summary>///增幅（小数） /// </summary>
    public double growth;
    /// <summary>///日期 /// </summary>
    public int day;
    /// <summary>///视频数 /// </summary>
    public int view;
    /// <summary>///倒计时（秒） /// </summary>
    public int countDown;
}


#endregion

#region 里程碑

//里程碑配置信息
public class Net_CB_MilestoneConfigList
{
    public List<Net_CB_MilestoneInfo> mileposts;  //里程碑数组
    public int lastGroupLevel;   //上一组里程碑最后等级
    public int position;        
}
[Serializable]
public class Net_CB_MilestoneInfo
{
    public int id;      //里程碑id
    public int group;   //里程碑组id
    public List<Net_Reward> winRewards;
    public int level;   //所需关卡等级
    public int status;
}

public class Net_DrawMilestone : Net_RequesetCommon
{
    public int milepostId;  //里程碑id
}

#endregion

#region 里程碑任务

/// <summary>
/// 里程碑任务配置
/// </summary>
public class Net_CB_MilestoneTaskConfigList
{
    public List<Net_CB_MilestoneTaskInfo> milepostTasks;  //里程碑数组
    // public int level; //当前用户等级
    // public int lastGroupLevel;   //上一组里程碑最后等级
    // public int position;        
}

/// <summary>
/// 里程碑任务奖励数组
/// </summary>
[Serializable]
public class Net_CB_MilestoneTaskInfo
{
    public int id;      //里程碑id
    // public int group;   //里程碑组id
    // public int winRewardType; //奖励类型
    // public int winNum;    //奖励数量
    public List<Rewards> winRewards;
    public int level;   //所需关卡等级
    public int status;  //领取状态 
}
/// <summary>
/// 里程碑任务领取
/// </summary>
public class Net_DrawMilestoneTask : Net_RequesetCommon
{
    public int taskId;  //里程碑任务id
}       

public class Net_CB_DrawMilestone
{
    public List<Net_Reward> rewards;
}
[Serializable]
public class Net_Reward
{
    public int type;
    public int num;
}
#endregion

#region 每日任务

public class Net_Rq_TaskConfig : Net_RequesetCommon
{
    /// <summary>
    /// 类型
    /// </summary>
    public int type;
}

public class Net_CB_TaskConfig
{
    public List<Net_CB_TaskArr> tasks;
    public List<Net_CB_TaskArr> mainTasks;
    public List<Net_CB_TaskArr> dayAccepts;
}

[Serializable]
public class Net_CB_TaskArr
{
    
    // /// <summary>
    // /// 游戏名
    // /// </summary>
    // public string desc;
    /// <summary>
    /// 游戏下载链接
    /// </summary>
    public string downLoadUrl;
    // /// <summary>
    // /// 包名
    // /// </summary>
    // public string pkgName;
    
    /// <summary>
    /// 任务id
    /// </summary>
    public int id;
    /// <summary>
    /// 要求数量
    /// </summary>
    public int condition;
    /// <summary>
    /// 已完成进度
    /// </summary>
    public int progress;
    /// <summary>
    /// 任务类型 1视频次数2每日过关3转盘4红包雨5免费红包6体力领取7飞行宝箱8免费领钱
    /// </summary>
    public int type;
    // /// <summary>
    // /// 领取数量
    // /// </summary>
    // public int winNum;
    // /// <summary>
    // /// 领取奖励类型 1.红包券 2 .可提现现金 3.不可提现现金 4.体力 5.免费提示 6.谢谢参与 7. 提现券
    // /// </summary>
    // public int winRewardType;
    /// <summary>
    /// 状态 1.已领取 2，未领取 3。未完成
    /// </summary>
    public int status;

    public List<Rewards> winRewards;
}

public class Net_TaskRewardReport :Net_RequesetCommon
{
    /// <summary>
    /// 奖励数值
    /// </summary>
    public int drawNum;
    /// <summary>
    /// 任务ID
    /// </summary>
    public int taskId;
    /// <summary>
    /// 奖励类型
    /// </summary>
    public int drawType;
}


/// <summary>
/// 下载其他游戏的任务
/// </summary>
public class Net_TaskDownloadComplete: Net_RequesetCommon
{
    /// <summary>
    /// 任务ID
    /// </summary>
    public int taskId;
}


#endregion

#region 登录领现金

public class Net_CB_LoginConfig
{
    /// <summary>
    /// 登录第几天
    /// </summary>
    public int day;

    /// <summary>
    /// 今日已观看视频次数
    /// </summary>
    public int viewAds;
    
    /// <summary>
    /// 提现配置
    /// </summary>
    public List<Net_CB_WithDraw> withDraws;

}

#endregion

#region 每日打卡


/// <summary>
/// 打卡配置
/// </summary>
public class Net_CB_ClockinConfig
{
    /// <summary>
    /// 当前第几天打卡
    /// </summary>
    public int day;
    /// <summary>
    /// 需要看广告次数
    /// </summary>
    public int needViewAd;
    /// <summary>
    /// 是否已打卡 1成功 2未打卡
    /// </summary>
    public int hasClock;

    /// <summary>
    /// 今日打卡观看视频次数
    /// </summary>
    public int hasViewAd;
}


/// <summary>
/// 打卡上报
/// </summary>
public class Net_Clockin : Net_RequesetCommon
{
  
}


#endregion

#region 每日签到

public class Net_CB_SignInConfig
{
    public int day;
    public Net_CB_EveryhDay[] signDays;
}
[Serializable]
public class Net_CB_EveryhDay
{
    /// <summary>
    /// 第几天
    /// </summary>
    public int day;
    /// <summary>
    /// 签到是否展示广告 1.展示 2.不展示
    /// </summary>
    public int needView;
    /// <summary>
    /// 签到条件，需要看几次视频
    /// </summary>
    public int viewAds;
    /// <summary>
    /// 是否已签到 1.已签到 2.未签到
    /// </summary>
    public int hasSigned;
    /// <summary>
    /// 红包券数量
    /// </summary>
    public int coupon;
    /// <summary>
    /// 奖励类型
    /// </summary>
    public int winRewardType;
    /// <summary>
    /// 奖励数量
    /// </summary>
    public int winNum;
    /// <summary>
    /// 领取的免费id
    /// </summary>
    public int acceptId;
}


public class Net_SignInReport : Net_RequesetCommon
{
    public int day;
    public int type;
}


#endregion

#region 体力

public class Net_Rq_EarnEnergy : Net_RequesetCommon
{
    /// <summary>1.赚取少量 2.赚取大量</summary>
    public int earnStrengthType;
}

/// <summary>
/// 查询体力
/// </summary>
public class Net_CB_SearchEnergy
{
    /// <summary>
    /// 免费获取少量体力次数
    /// </summary>
    public int fewStrengthLimit;
        
    /// <summary>
    /// 免费获取大量体力次数
    /// </summary>
    public int largeStrengthLimit;
        
    /// <summary>
    /// 剩余体力
    /// </summary>
    public int strength;

    /// <summary>
    /// 增加体力频次（秒）
    /// </summary>
    public int strengthInterval;

    /// <summary>
    /// 自动增加体力值上限
    /// </summary>
    public int upperLimit;
}

#endregion

#region 升级

public class Net_Rq_Upgrad : Net_RequesetCommon
{
    public int type;
}

/// <summary>
/// 关卡奖励多倍领取
/// </summary>
public class Net_Rq_UpgradeDouble : Net_RequesetCommon
{
    /// <summary>当前等级</summary>
    public int level;

    public int type;
}

#endregion

#region 邀请配置接口

public class Net_CB_InviteConfig
{
    // public long coupon;
  
    // public long totalNum;
    // public long yearNum;

    /// <summary>
    /// 邀请人信息
    /// </summary>
    public PlayUser parentUser;
    
    public int friendNum;
}

public class Net_Rq_InputInvoke : Net_RequesetCommon
{
    /// <summary>
    /// 邀请码
    /// </summary>
    public int inviteCode;
}

[Serializable]
public class PlayUser
{
    public string nickName;

    public string headImgUrl;
    
    public string invitationCode;
}

#endregion

#region 存钱罐

public class Net_CB_GoldenpigConfig
{
    /// <summary>昨日存储金额</summary>
    public int money;
    /// <summary>今日存储金额</summary>
    public int storeMoney;

    /// <summary>
    /// 昨日存储元宝数量
    /// </summary>
    public int coupon;
    /// <summary>
    /// 今日存储元宝数量
    /// </summary>
    public int storeCoupon;
    
    /// <summary>今日是否已提现1.已提现2.未提现</summary>
    public int hasWithdraw;
    /// <summary>提现需要通关关卡数</summary>
    public int needLeve;
}

#endregion

#region 新视频红包

#region 视频红包配置
public class Net_Rq_VideoRed : Net_RequesetCommon
{
    public int type;
}
/// <summary>
/// 视频红包配置
/// </summary>
public class Net_CB_VideoRed
{
    /// <summary>
    /// 下次间隔时间 秒
    /// </summary>
    public int intervalTime;
    /// <summary>
    /// 领取红包剩余次数
    /// </summary>
    public int videoRedLimit;
    /// <summary>
    /// 最多可得金币
    /// </summary>
    public int mostCoupon;
    /// <summary>
    /// 最多可得假现金
    /// </summary>
    public int mostBougs;
    /// <summary>
    /// 普通领取红包券 分
    /// </summary>
    public int bougs;
    /// <summary>
    /// 普通领取金币
    /// </summary>
    public int coupon;
    /// <summary>
    /// 领取红包最大次数
    /// </summary>
    public int videoRedSize = 20;
}
#endregion

#region 视频红包领取

public class Net_Rq_VideoRedGet : Net_RequesetCommon
{
    /// <summary>
    /// 类型 1.观看视频得金币 2.在线红包 3.提现页直接领红包 4.提现页直接领金币
    /// </summary>
    public int type;
    /// <summary>
    /// 是否看视频领取 1.已关看 2.未观看
    /// </summary>
    public int isView;
}


public class Net_CB_VideoRedGet
{
    public List<Rewards> rewards;
}

#endregion

#endregion

#region 广告任务逻辑

public class Net_ViewAD : Net_RequesetCommon
{
    public string adid;     //广告位id
    public int operatorType = 1;
}


#endregion

#region 大生产活动


/// <summary>
/// 生产配置接口
/// </summary>
public class Net_CB_ProduceConfig
{
    /// <summary>
    /// 本轮收入
    /// </summary>
    public int totalNum;
    
    /// <summary>
    /// 存折收入
    /// </summary>
    public int money;
    
    /// <summary>
    /// 本小时召集人数
    /// </summary>
    public int inviteNum;

    /// <summary>
    /// 本小时预计收入
    /// </summary>
    public long expectMoney;

    /// <summary>
    /// 本小时剩余次数
    /// </summary>
    public long viewLimit;
    
    /// <summary>
    /// 倒计时时间
    /// </summary>
    public long time;
}

/// <summary>
/// 本月工资条
/// </summary>
public class Net_CB_WithDrawHis
{
    public List<WithDrawHis> withDraws;
}


[Serializable]
public class WithDrawHis
{
    /// <summary>
    /// 提现的id
    /// </summary>
    public int id;
    /// <summary>
    /// 提现的金额（分）
    /// </summary>
    public int withDrawNum;
    /// <summary>
    /// 提现时间
    /// </summary>
    public long withDrawTime;
    /// <summary>
    /// 1.审核中 2.提现成功
    /// </summary>
    public int status;
}

/// <summary>
/// 前一天的提现金额
/// </summary>
public class Net_CB_MemberPayslip
{
    public List<Payslip> withDraws;
}

[Serializable]
public class Payslip
{
    /// <summary>
    /// 微信昵称
    /// </summary>
    public string nickName;
    /// <summary>
    ///  	提现的金额（分）
    /// </summary>
    public int withDrawNum;
    /// <summary>
    /// 提现时间
    /// </summary>
    public long withDrawTime;
    /// <summary>
    /// 1.审核中 2.提现成功
    /// </summary>
    public int status;
}


public class Net_CB_ProduceRanking
{
    public List<WithDrawRank> withDraws;
}

/// <summary>
/// 排行榜中的用户信息
/// </summary>
[Serializable]
public class WithDrawRank
{
    /// <summary>
    /// 微信昵称
    /// </summary>
    public string nickName;
    /// <summary>
    /// 邀请码
    /// </summary>
    public string inviteCode;
    /// <summary>
    /// 提现的金额（分）
    /// </summary>
    public int withDrawNum;
    /// <summary>
    /// 提现时间
    /// </summary>
    public long withDrawTime;
    /// <summary>
    /// 1.审核中 2.提现成功
    /// </summary>
    public int status;

    /// <summary>
    /// 加成系数
    /// </summary>
    public float rate;
}


public class Net_CB_Barrage
{
    public List<WithDrawRank> withDraws;
}

#endregion

#region 验证sessId

public class SDK_Packet
{
    public DataModule.TableNetworkRequestData requestData;

    public Cmd type;
    public string postDataJson;
    public Action<string> action;
    public List<UrlParams> urlParams;
}

#endregion

#region 通用玩法

public class Net_GamecoreConfig : Net_RequesetCommon
{
    public int mid; //模块id EGamecoreType
}

[Serializable]
public class Net_CB_GamecoreConfig
{
    public int dayProgress; //当日完成进度
    public int dayAcceptTimes;  //当日领取限制次数 0为无限制次数
    public int progress;        //当前总进度
    public int acceptTimes;     //总限制次数 0为无限制
    public List<Rewards> rewards;     //奖励
    public int intervalTime; //间隔时间
}

public class Net_GamecoreAccept : Net_RequesetCommon
{
    public int mid;     //模块id EGamecoreType
    public int type;    //操作类型针对指定业务阶段性操作
}

[Serializable]
public class Net_CB_GamecoreAccept : Net_CB_Reward
{

}

#endregion
