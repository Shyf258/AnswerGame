/***
 * 
 *    Title: "SUIFW" UI框架项目
 *           主题： 框架核心参数  
 *    Description: 
 *           功能：
 *           1： 系统常量
 *           2： 全局性方法。
 *           3： 系统枚举类型
 *           4： 委托定义
 *                          
 *    Date: 2017
 *    Version: 0.1版本
 *    Modify Recoder: 
 *    
 *   
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SUIFW
{
    #region 系统枚举类型

    /// <summary>
    /// UI窗体（位置）类型
    /// </summary>
    public enum UIFormType
    {
        //普通窗体
        Normal,
        //固定窗体                              
        Fixed,
        //弹出窗体
        PopUp,
        //最上层窗体
        Topside,
    }

    /// <summary>
    /// UI窗体的显示类型
    /// </summary>
    public enum UIFormShowMode
    {
        //普通
        Normal,
        //反向切换
        ReverseChange,
        //隐藏其他
        HideOther
    }

    /// <summary>
    /// UI窗体透明度类型
    /// </summary>
    public enum UIFormLucenyType
    {
        //完全透明，不能穿透
        Lucency,
        //深透明，不能穿透
        Dark,
        //浅透明度，不能穿透
        Light,
        //可以穿透
        Pentrate
    }

    //UI窗口的状态
    public enum UIBaseState
    {
        None,
        Opening,    //正在播放打开动画
        Opened,     //打开中
        Closeing,   //正在播放关闭动画
        Closeed,    //已关闭
    }
    #endregion

    public class SysDefine : MonoBehaviour
    {
        /* 路径常量 */
        public const string SYS_PATH_CANVAS = "SUIFW/Canvas";
        public const string SYS_PATH_UIFORMS_CONFIG_INFO = "SUIFW/Json/UIFormsConfigInfo";
        //public const string SYS_PATH_CONFIG_INFO = "SUIFW/Json/SysConfigInfo";
        //public const string SYS_PATH_LANGUAGE = "SUIFW/Json/LauguageJSONConfig";

        /* 标签常量 */
        public const string SYS_TAG_CANVAS = "TagCanvas";
        /* 节点常量 */
        public const string SYS_NORMAL_NODE = "Normal";
        public const string SYS_FIXED_NODE = "Fixed";
        public const string SYS_POPUP_NODE = "PopUp";
        public const string SYS_TOPSIDE_NODE = "Topside";
        public const string SYS_SCRIPTMANAGER_NODE = "_ScriptMgr";
        /* 遮罩管理器中，透明度常量 */

        //遮罩透明度 深色
        public const float UiMask_Dark_Color_A = 0.7f;
        //遮罩透明度 浅色
        public const float UiMask_Light_Color_A = 0.5f;

        /* 摄像机层深的常量 */

        /* 全局性的方法 */
        //Todo...

        /* 委托的定义 */
        //Todo....

        /* UI PATH*/
        public const string UI_Path_Task = "Task";
        public const string UI_Path_TurnTable = "TurnTable";
        public const string UI_Path_RedRain = "RedRain";
        public const string UI_Path_Loading = "Loading";
        public const string UI_Path_Splash = "Splash";
        public const string UI_Path_Game = "Game";
        public const string UI_Path_Main = "Main";
        public const string UI_Path_MainUp = "MainUp";
        public const string UI_Path_PositionDetail = "PositionDetail";

        public const string UI_Path_RedWithdraw = "RedWithdraw";      //提现界面
        public const string UI_Path_Withdraw = "Withdraw";      //提现界面
        public const string UI_Path_PlayAdWithDraw = "PlayAdWithDraw";
        public const string UI_Path_WeChatLogin = "WeChatLogin";      //提现界面
        public const string UI_Path_WithdrawSuccess = "WithdrawSuccess";      //提现界面
        public const string UI_Path_WaitTips = "WaitTips";      //储存提示
        public const string UI_Path_Date = "Date";
        public const string UI_Path_PlayInfo = "PlayInfo";          //角色信息
        public const string UI_Path_Share = "Share";
        public const string UI_Path_ShareResult = "ShareResult";
        public const string UI_Path_Arrive = "Arrive";
        public const string UI_Path_RightReward = "RightReward";
        public const string UI_Path_SignIn = "SignIn";
        public const string UI_Path_NewSignInPage = "NewSignInPage";
        public const string UI_Path_Ranking = "Ranking";
        public const string UI_Path_FeedBack = "FeedBack";
        public const string UI_Path_Setting = "Setting";
        public const string UI_Path_GetReward = "GetReward";
        public const string UI_Path_GetResult = "GetResult"; 
        public const string UI_Path_GetNormal = "GetNormal";
        public const string UI_Path_Notice = "Notice";
        public const string UI_Path_Agreement = "Agreement";
        // public const string UI_Path_Success = "Success";
        public const string UI_Path_AnswerRight = "AnswerRight";
        public const string UI_Path_Fail = "Fail";
        public const string UI_Path_NoEnergy = "NoEnergy";
        public const string UI_Path_Cash = "Cash";
        public const string UI_Path_Coin = "Coin";
        public const string UI_Path_OpenRedPack = "OpenRedPack";
        public const string UI_Path_SurpriseRedpack = "SurpriseRedpack";
        public const string UI_Path_LimitNewGift = "LimitNewGift";
        public const string UI_Path_ChangeSignType = "ChangeSignType";
        public const string UI_Path_NewbieSign = "NewbieSign";
        public const string UI_Path_DragRedpack = "DragRedpack";
        public const string UI_Path_Explain = "Explain";
        public const string UI_Path_CoinBox = "CoinBox";
        public const string UI_Path_EventPage = "EventPage";
        public const string UI_Path_TipsADTimes = "TipsADTimes";
        public const string UI_Path_GrowMoneyPage = "GrowMoneyPage";
        public const string UI_Path_Check = "Check";
        public const string UI_Path_GetCoin = "GetCoin";
        public const string UI_Path_TimeActivity = "TimeActivity";
        public const string UI_Path_Production = "Production";


        public const string UI_Path_GM = "GM";
        public const string UI_Path_Guide = "Guide";
        public const string UI_Path_GuideTip = "GuideTip";
        public const string UI_Path_GetChips = "GetChips";
        public const string UI_Path_ChipsPage = "ChipsPage";
        public const string UI_Path_AboutUS = "AboutUS";
        public const string UI_Path_DictionaryPage = "DictionaryPage";
        public const string UI_Path_PureGame = "PureGame";
        public const string UI_Path_PureAnswer = "PureAnswer";
        public const string UI_Path_MainPage = "MainPage";
        public const string UI_Path_TipsPage = "TipsPage";
        public const string UI_Path_DailyMoney = "DailyMoney";
        public const string UI_Path_NetLoading = "NetLoading";
        public const string UI_Path_GlobalMask = "GlobalMask";
        
        public const string UI_IF_MoneyPool = "MoneyPool";
        public const string UI_IF_Goldenpig = "Goldenpig";

        public const string UI_Path_Salary = "Salary";
        public const string UI_Path_Description = "Description";
        
        public const string UI_IF_GoldIngot = "GoldIngot";
        
        public const string UI_IF_MyWithdrawRecord = "MyWithdrawRecord";

        public const string UI_Path_NewWithdraw = "NewWithdraw";

        public const string UI_Path_Activity = "Activity";
        public const string UI_Path_ActivitySign = "ActivitySign";
        
        public const string UI_Path_RedGroup = "RedGroup";
        public const string UI_Path_RedGroupChat = "RedGroupChat";
        public const string UI_Path_WithDrawJudge = "RedGroupChat";
    }
}