//2018.12.13    关林
//数据分析

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GL_Analytics_Logic : Singleton<GL_Analytics_Logic>
{
    
    //打点逻辑
    public void SendLogEvent(EAnalyticsType type, int suffix)
    {
        GL_SDK._instance.LogEvent(GetAnalyticsData(type) + suffix, string.Empty);
    }
    public void SendLogEvent(EAnalyticsType type, string suffix)
    {
        GL_SDK._instance.LogEvent(GetAnalyticsData(type) + suffix, string.Empty);
    }

    public void SendLogEvent(string type)
    {
        GL_SDK._instance.LogEvent(type, "");
    }

    public void SendLogEvent(EAnalyticsType type)
    {
        GL_SDK._instance.LogEvent(GetAnalyticsData(type), string.Empty);
    } 

    private string GetAnalyticsData(EAnalyticsType type)
    {
        return AnalyticsEvents.stores[type];
    }
}
/// <summary>
/// 上报类型
/// </summary>
public enum EAnalyticsType
{
    /// <summary> 首次获取appconfig </summary>
    GetAppConfig,
    /// <summary> appconfig结果</summary>
    AppConfigResult,
    /// <summary> 首次登陆</summary>
    FirstSendLogin,
    /// <summary> 登陆结果</summary>
    LoginResult,
    /// <summary> 防沉迷验证</summary>
    CkeckFCM,
    /// <summary> 场景加载完成 </summary>
    LoadGameSceneComplete,
    /// <summary> 创建关卡 </summary>
    CreateLevel,
    /// <summary> 完成关卡 </summary>
    CompleteLevel,
    /// <summary>/// 激活上报/// </summary>
    ActiveGame,
    /// <summary>/// 微信登录成功/// </summary>
    WeChatLoginResult,

 
    /// <summary>/// 里程碑奖励手动领取/// </summary>
    MilestoneClick,
    /// <summary>/// 里程碑奖励领取/// </summary>
    MilestonceGet,
    /// <summary>/// 升级/// </summary>
    LevelUp,
    
    
    
    /// <summary>/// 转圈红包点击/// </summary>
    TurnGetRed,
    /// <summary>/// 转圈红包领取/// </summary>
    TurnIcon,
    /// <summary>/// 转圈红包领取/// </summary>
    TurnRedGetFinish,
    
    /// <summary>/// 任务视频播放/// </summary>
    TaskVideo,
    /// <summary>/// 通关任务完成/// </summary>
    TaskPassFinish,
    /// <summary>/// 每日登录任务/// </summary>
    TaskDaily,
    /// <summary>/// 视频金币完成/// </summary>
    VideoCoin,
    
    /// <summary>/// 红包提现/// </summary>
    WithDrawRed,
    /// <summary>/// 直接领红包按键点击/// </summary>
    GetRedCoin,
    /// <summary>/// 直接领红包按键点击/// </summary>
    GetRedCoinFinish,
    
    /// <summary>/// 金币提现/// </summary>
    WithDrawCoin,
    /// <summary>/// 直接领红包按键点击/// </summary>
    GetCoinCoin,
    /// <summary>/// 直接领红包按键点击/// </summary>
    GetCoinCoinFinish,
    
    
    /// <summary>/// 弹出/// </summary>
    WithDrawTipPopup,
    
    /// <summary>/// 提现点击/// </summary>
    WithDrawTipClick,
    
    /// <summary>/// 提现完成 /// </summary>
    WithDrawTipFinish,
    
    /// <summary>/// 提现完成 /// </summary>
    WithDrawTipClose,

    #region 活动
    
    ActivityVideoClick,
    ActivityVideoPlayStart,
    ActivityVideoPlayEnd,
    
    ActivitySignClick,
    ActivitySignGet,
    ActivitySignClose,
    
    ActivityAnswerClick,
    ActivityAnswerRedClick,
    ActivityAnswerRedCloseGet,
    ActivityAnswerRedGetSuccess,

    #endregion
    
    /// <summary>/// 玩家成功点击新人红包/// </summary>
    NewPlaySign,
    /// <summary>/// 点击领取新人红包奖励/// </summary>
    NewPlayClickRed,
    /// <summary>/// 点击“抽”进行选择/// </summary>
    NewPlayGetRed,
    /// <summary>/// 点击688“点击到账”按键/// </summary>
    NewPlayReceive,
    /// <summary>/// 选择7天模式按键/// </summary>
    Change7,
    /// <summary>/// 选择14天模式按键/// </summary>
    Change14,
    /// <summary>/// 选择360天模式按键/// </summary>
    Change360,
    /// <summary>/// 7天模式中点击快速签到/// </summary>
    GetRewardPlayAd7,
    /// <summary>/// 7天模式中观看视频观看成功/// </summary>
    GetRewardFinishAd7,
    /// <summary>/// （7天模式中）成功领取第七天奖励/// </summary>
    GetReward7,
    /// <summary>/// 14天模式中点击快速签到/// </summary>
    GetRewardPlayAd14,
    /// <summary>/// 14天模式中观看视频观看成功/// </summary>
    GetRewardFinishAd14,
    /// <summary>/// （14天模式中）成功领取第十四天奖励/// </summary>
    GetReward14,
    /// <summary>/// （360天模式中）成功领取第360天奖励/// </summary>
    GetReward360,
    
    
    /// <summary>/// 微信登录点击/// </summary>
    WeChatClick,
    /// <summary>/// 微信登录成功/// </summary>
    WeChatSuccess,
    /// <summary>/// 微信登录失败/// </summary>
    WeChatFails,
    /// <summary>/// 微信登录成功进入游戏/// </summary>
    WecChatLogin,
    /// <summary>/// 微信登录成功进入游戏失败/// </summary>
    WecChatLoginFails,
    
    
    /// <summary>/// 关卡领取金币/// </summary>
    GetCoin,
    /// <summary>/// 关卡领取红包/// </summary>
    GetRed,
    
    /// <summary>/// 0.38提现点击/// </summary>
    WithDrawLow,
    /// <summary>/// 0.38提现成功/// </summary>
    WithDrawLowSuccess,
    
    /// <summary>/// 0.68提现点击/// </summary>
    WithDrawMedium,
    /// <summary>/// 0.68提现成功/// </summary>
    WithDrawMediumSuccess,
    
    
    /// <summary>/// 0.88提现点击/// </summary>
    WithDrawHigh,
    /// <summary>/// 0.88提现成功/// </summary>
    WithDrawHighSuccess,
    
    //财神
    /// <summary>/// 财神ICON点击/// </summary>
    MoneyPoolIcon,
    /// <summary>/// 财神点击多倍领取按键/// </summary>
    MoneyPoolGrowBTN,
    /// <summary>/// 财神视频播放广告请求/// </summary>
    MoneyPoolPlayVideo,
    /// <summary>/// 财神视频回调成功/// </summary>
    MoneyPoolVideoCBFinish,
    /// <summary>/// 财神视频回调失败/// </summary>
    MoneyPoolVideoCBFail,
    /// <summary>/// 财神视频播放失败/// </summary>
    MoneyPoolVideoFinish,
    /// <summary>/// 财神视频播放成功/// </summary>
    MoneyPoolVideoFail,
    
    /// <summary>
    /// 加速提现
    /// </summary>
    GrowMoneyPlan,
    /// <summary>
    /// 加速提现点击
    /// </summary>
    GrowMoneyPlanClick,
    /// <summary>
    /// 加速提现完成观看视频
    /// </summary>
    GrowMoneyPlanFinish,
    /// <summary>/// 金币提现解锁/// </summary>
    CoinDailySuccess,
    
    
    #region 新手签到领取

    /// <summary>
    /// 主界面签到图标点击
    /// </summary>
    NewPlayerSign,
    
    /// <summary>
    /// 普通签到领取
    /// </summary>
    LoginReceive,
    
    /// <summary>
    /// 5天登录签到领取
    /// </summary>
    LoginReceiveAccumulated,
    
    /// <summary>
    /// 新手福利金币
    /// </summary>
    NewPlayerReceive,

    #endregion

}


/// <summary>
/// 上报事件
/// </summary>
public static class AnalyticsEvents
{
    public static Dictionary<EAnalyticsType, string> stores = new Dictionary<EAnalyticsType, string>()
    {
        {EAnalyticsType.GetAppConfig,"GetAppConfig"},
        {EAnalyticsType.AppConfigResult,"AppConfigRusult"},
        {EAnalyticsType.FirstSendLogin,"FirstSendLogin"},
        {EAnalyticsType.LoginResult,"LoginResult"},
        {EAnalyticsType.CkeckFCM,"CkeckFCM"},
        {EAnalyticsType.LoadGameSceneComplete,"LoadGameSceneComplete"},
        {EAnalyticsType.CreateLevel,"CreateLevel"},
        {EAnalyticsType.CompleteLevel,"CompleteLevel"},

        {EAnalyticsType.ActiveGame,"jihuoxinxi_anjian_01"},
        {EAnalyticsType.WeChatLoginResult,"weixindengluchenggong_001"},
        
        {EAnalyticsType.MilestoneClick,"lichengbei_dianji_01"},
        {EAnalyticsType.MilestonceGet,"lichengbei_dianji_02"},
        
        
        {EAnalyticsType.LevelUp,"chuangguan_renjun_01"},
        
        {EAnalyticsType.TurnIcon,"zhuanquanhongbao_icon_01"},
        {EAnalyticsType.TurnGetRed,"zhuanquanhongbao_shipin_01"},
        {EAnalyticsType.TurnRedGetFinish,"zhuanquanhongbao_shipin_02"},
        
        {EAnalyticsType.TaskDaily,"renwu_meiridenglu_01"},
        {EAnalyticsType.VideoCoin,"renwu_shipinjinbi_01"},
        // {EAnalyticsType.TaskVideoFinish,"renwu_tongguan_01"},
        {EAnalyticsType.TaskPassFinish,"renwu_tongguan_01"},
        {EAnalyticsType.TaskVideo,"renwu_shipin_01"},
        
        {EAnalyticsType.WithDrawRed,"hongbao_anjian_01"},
        {EAnalyticsType.GetRedCoin,"linghongbao_shipin_01"},
        {EAnalyticsType.GetRedCoinFinish,"linghongbao_shipinOK_01"},
        
        {EAnalyticsType.WithDrawCoin,"jinbi_anjian_01"},
        {EAnalyticsType.GetCoinCoin,"lingshipin_anjian_01"},
        {EAnalyticsType.GetCoinCoinFinish,"lingjinbi_shipinOK_01"},
      
        {EAnalyticsType.WithDrawTipPopup,"guanggaoshifang_01"},
        {EAnalyticsType.WithDrawTipClick,"guanggaoshifang_02"},
        {EAnalyticsType.WithDrawTipFinish,"guanggaoshifang_03"},
        {EAnalyticsType.WithDrawTipClose,"guanggaoshifang_05"},
        
        //活动
        {EAnalyticsType.ActivityVideoClick,"kanshipinyouqian_01"},
        {EAnalyticsType.ActivityVideoPlayStart,"kanshipinyouqian_02"},
        {EAnalyticsType.ActivityVideoPlayEnd,"kanshipinyouqian_03"},
        
        {EAnalyticsType.ActivitySignClick,"7tianjinbilingqu_01"},
        {EAnalyticsType.ActivitySignGet,"7tianjinbilingqu_02"},
        {EAnalyticsType.ActivitySignClose,"7tianjinbilingqu_03"},
        
        {EAnalyticsType.ActivityAnswerClick,"kanshipinyouqian_01"},
        {EAnalyticsType.ActivityAnswerRedClick,"kanshipinyouqian_02"},
        {EAnalyticsType.ActivityAnswerRedCloseGet,"kanshipinyouqian_03"},
        {EAnalyticsType.ActivityAnswerRedGetSuccess,"kanshipinyouqian_04"},
        
        //688
        {EAnalyticsType.NewPlaySign,"xinrenhongbao_01"},
        {EAnalyticsType.NewPlayClickRed,"xinrenhongbao_02"},
        {EAnalyticsType.NewPlayGetRed,"xinrenhongbao_03"},
        {EAnalyticsType.NewPlayReceive,"xinrenhongbao_04"},
        {EAnalyticsType.Change7,"qiandaomoshi_01"},
        {EAnalyticsType.Change14,"qiandaomoshi_02"},
        {EAnalyticsType.Change360,"qiandaomoshi_03"},
        {EAnalyticsType.GetRewardPlayAd7,"7_qiandao_01"},
        {EAnalyticsType.GetRewardFinishAd7,"7_qiandao_02"},
        {EAnalyticsType.GetReward7,"7_qiandao_03"},
        {EAnalyticsType.GetRewardPlayAd14,"14_qiandao_01"},
        {EAnalyticsType.GetRewardFinishAd14,"14_qiandao_02"},
        {EAnalyticsType.GetReward14,"14_qiandao_03"},
        {EAnalyticsType.GetReward360,"360_qiandao_01"},
        
        
        {EAnalyticsType.WeChatClick,"yonghudenglu_06"},
        {EAnalyticsType.WeChatSuccess,"yonghudenglu_07"},
        {EAnalyticsType.WeChatFails,"yonghudenglu_08"},
        {EAnalyticsType.WecChatLogin,"yonghudenglu_09"},
        {EAnalyticsType.WecChatLoginFails,"yonghudenglu_10"},
        
        {EAnalyticsType.GetCoin,"guanqiajiesuan_01"},
        {EAnalyticsType.GetRed,"guanqiajiesuan_02"},
        
        {EAnalyticsType.WithDrawLow,"tixian_0.38_anjian"},
        {EAnalyticsType.WithDrawLowSuccess,"tixian_0.38_chenggong"},
        {EAnalyticsType.WithDrawMedium,"tixian_0.68_anjian"},
        {EAnalyticsType.WithDrawMediumSuccess,"tixian_0.68_chenggong"},
        {EAnalyticsType.WithDrawHigh,"tixian_0.88_anjian"},
        {EAnalyticsType.WithDrawHighSuccess,"tixian_0.88_chenggong"},
        
        //财神  MoneyPoolIcon,
        {EAnalyticsType.MoneyPoolIcon,"caishen_icon_01"},
        {EAnalyticsType.MoneyPoolGrowBTN,"caishen_duobeidianji_01"},
        {EAnalyticsType.MoneyPoolPlayVideo,"caishen_bofang_qingqiu_01"},
        {EAnalyticsType.MoneyPoolVideoCBFinish,"caishen_huidiaochenggong_01"},
        {EAnalyticsType.MoneyPoolVideoCBFail,"caishen_huidiaoshibai_01"},
        {EAnalyticsType.MoneyPoolVideoFinish,"caishen_bofangshibai_01"},
        {EAnalyticsType.MoneyPoolVideoFail,"tcaishen_bofangchenggong_01"},
        
        
        {EAnalyticsType.GrowMoneyPlan,"jiasuanjian_01"},
        {EAnalyticsType.GrowMoneyPlanClick,"jiasuanjian_02"},
        {EAnalyticsType.GrowMoneyPlanFinish,"txsp_001"},
        {EAnalyticsType.CoinDailySuccess,"tixian_0.3_chenggong"}, 
        
        #region 新手签到领取
        {EAnalyticsType.NewPlayerReceive,"xinshoufuli_001"},
        {EAnalyticsType.NewPlayerSign,"qiandao_001"},
        {EAnalyticsType.LoginReceive,"qiandao_002"},
        {EAnalyticsType.LoginReceiveAccumulated,"5tianqiandao_001"},
        #endregion
    };
}