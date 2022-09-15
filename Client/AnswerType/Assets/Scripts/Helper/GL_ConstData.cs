//2018.08.31    关林
//常量

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GL_ConstData
{

    /// <summary>
    /// Vector3 错误常量
    /// </summary>
    public static Vector3 VectorError = new Vector3(3.141516f, 3.141516f, 3.141516f);


#if !UNITY_IOS
    public const string URL_TermsOfUse = "https://planbie.com/term-of-use/";
    public const string URL_Privacy = "https://planbie.com/privacy-policy-2/";
#else
        public const string URL_TermsOfUse = "https://cacacats.com/terms-of-use";
        public const string URL_Privacy = "https://cacacats.com/privacy/";
#endif


    public static string PackageName = "com.ciyunjinchan.bxdyjfour";
    public static string WeChatAppId = "wx9016768c8f632e0c";

    //音效 路径
    public const string Path_AudioPrefab = "Prefab/Audio/";
    //特效
    public const string EffectPrefabPath = "Prefab/Effect/";

    //网络图片保存地址(主要是头像
    public const string ServerTexturePath = "ServerTexture/";
    //音效片段
    public const string AuidoClipPath = "AudioClip/";

    //答题图片
    public const string LevelImagePath = "Art/Assetbundle/Levelimage";
    /// <summary>
    /// 飞行预制件路径
    /// </summary>
    public const string FlyPrefabPath = "Prefab/FlyItem/";
    
    //广告场景-通用
    public const int SceneID_Normal = 1000000;
    //广告场景-财神提现
    public const int SceneID_MoneyPool = 1000001;
        
    //广告场景-生产
    public const int SceneID_Production = 1000004;
    //广告场景-打卡
    public const int SceneID_Sign =  1000005;
    
    //广告场景-装盘
    public const int SceneID_Turn = 1000008;
       

    #region 游戏数值设定
    //单元格尺寸
    public const int CellSize = 75;
    //体力恢复间隔(分钟
    public const int StrengthInterval = 15;
    //截图储存位置文件名
    public const string ScreenshotTexture = "bzsk";
    //任务领取 有激励视频时的,提现劵条件数量
    public const int TaskRewardAdNumber = 1;

    public const int MaxGetRewardCount = 5 ;
    
    public const int IgnorAds = 5;
    //普通领取播放广告间隔
    // public const int NormalRewardTimes = 5;
    //插屏时间间隔
    public const float DialogTime = 120;
    
    /// <summary>
    /// 提现券数量描述
    /// </summary>
    public const int CashCouponTips = 22;
    /// <summary>
    /// 现金数量描述
    /// </summary>
    public const int BogusTips = 23;
    /// <summary>
    /// 金币数量描述
    /// </summary>
    public const int CoinTips = 24;

    public const float CoolDown = 25;

    public const int AntiTime = 5400;
    
    public const string AnswerPage = "Main";
    public const string TaskPage = "Task";
    // public const string PersonPage = "Person";
    #endregion

    #region Layer赋值
    public const string Layer_DrawingBoard = "DrawingBoard";
    public const string Layer_Playing = "Playing";
    public const string Layer_UILight = "UI1";

    public static readonly int Layer_UI = LayerMask.NameToLayer("UI");

    public static int LayerMask_UI = 1 << Layer_UI;
    #endregion

    #region 网络通信默认值
    //协议开头
    public const string HTTP = "http://";
    #endregion

}
