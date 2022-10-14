/// <summary>
/// 协议号
/// </summary>
public enum Cmd
{
    /// <summary> 游客登录 </summary>
    LoginGuest = 20001,

    /// <summary> 用户配置 </summary>
    SystemConfig = 20002,

    /// <summary> 微信登录 </summary>
    LoginWeChat = 20003,

    /// <summary> 应用配置接口 </summary>
    AppConfig,

    /// <summary>/// 注销微信登录  /// </summary>
    LogOutWeChat = 20005,
    
    /// <summary> 获得关卡 </summary>
    GetLevel = 30010,

    /// <summary> 升级 </summary>
    Upgrade = 30011,

    /// <summary> 升级多倍 </summary>
    UpgradeDouble = 30012,

    /// <summary> 获得体力 </summary>
    GetEnergy = 30020,

    /// <summary> 赚取体力 </summary>
    EarnEnergy = 30021,

    /// <summary> 消耗体力 </summary>
    CostEnergy = 30022,
    
    /// <summary> 每日任务配置 </summary>
    TaskConfig = 30034,
    
    /// <summary> 领取每日任务奖励 </summary>
    TaskReward = 30035,
    
    /// <summary> 下载其他游戏任务完成 </summary>
    TaskDownloadComplete = 30036,

    /// <summary>/// 每日打卡配置/// </summary>
    ClockinConfig = 30103,
    
    /// <summary>/// 打卡领取/// </summary>
    Clockin = 30104,
    
    /// <summary> 提现配置 </summary>
    WithDrawConfig = 30110,

    /// <summary> 提现 </summary>
    WithDraw = 30111,

    /// <summary> 提现记录 </summary>
    WithdrawRecord = 30130,

    /// <summary> 看广告 </summary>
    WatchAd = 30140,
    /// <summary> 广告下载 </summary>
    DownloadAd = 30141,

    /// <summary> 里程碑配置 </summary>
    MilestoneConfig = 40110,
    
    /// <summary> 里程碑领取 </summary>
    MilestonePost = 40111,

    /// <summary> 里程碑配置 </summary>
    MilestoneTaskConfig = 40112,
    
    /// <summary> 里程碑领取 </summary>
    MilestoneTaskPost = 40113,
    
    /// <summary>/// 广告播放配置/// </summary>
    PlayAd = 70001,
    
    /// <summary> 存钱罐配置 </summary>
    GoldenpigConfig = 70100,
    
    /// <summary> 存钱罐提现 </summary>
    GoldenpigWithdraw = 70101,
    
    /// <summary>/// 邀请配置接口/// </summary>
    InviteConfig = 80100,
    
    /// <summary>/// 填写邀请码接口/// </summary>
    InviteInput = 80101,
    
    /// <summary>/// 大生产配置/// </summary>
    ProduceConfig = 80501,
    
    /// <summary>/// 生产提现接口/// </summary>
    ProduceWithDraw = 80502,
    
    /// <summary>/// 本月工资条接口/// </summary>
    ProduceWithDrawHis = 80503,
    
    /// <summary>/// 成员前一天的提现金额/// </summary>
    ProducePayslip = 80504,
    
    /// <summary>/// 上小时收入排行/// </summary>
    ProduceRanking = 80505,
    
    /// <summary>/// 弹幕文字/// </summary>
    ProduceLastHour = 80506,
    
    /// <summary> 新视频红包配置 </summary>
    VideoRedConfig = 80701,
    /// <summary> 新视频红包领取 </summary>
    VideoRedGet = 80702,

    /// <summary> 通用玩法配置 </summary>
    GamecoreConfig = 90001,
    /// <summary> 通用玩法领取 </summary>
    GamecoreAccept = 90002,
    
    /// <summary>/// 云控配置/// </summary>
    AppControl = 90101,
    
    /// <summary>/// 体现增幅配置/// </summary>
    WithDrawGrowConfig = 90301,
        
    /// <summary>/// 提现增幅配置/// </summary>
    LoginWithDrawConfig = 90401,

}