using System;
using System.Collections;
using System.Collections.Generic;
using Logic.Fly;
using Logic.System.NetWork;
using SUIFW;
using SUIFW.Diplomats.Common;
using UnityEngine;

public class FF_Task_Config
{
    // private Net_CB_TaskConfig _taskConfig;
    public List<Net_CB_TaskArr> _taskArrs;
    public Net_CB_TaskArr _taskPlan;
    private UI_IF_Main _main;
    // private UI_IF_EventPage _eventPage;
    public int GetStatus()
    {
        int _count = 0;
        foreach (var VARIABLE in GL_PlayerData._instance.TaskConfig.tasks)
        {
            if (VARIABLE.status == 2)
            {
                _count++;
            }
        }
        return _count;
    }
    public void SetUIMain()
    {
        _main = UIManager.GetInstance().GetUI(SysDefine.UI_Path_Main) as UI_IF_Main;
        // _eventPage= UIManager.GetInstance().GetUI(SysDefine.UI_Path_EventPage) as UI_IF_EventPage;
    }
    public bool GetMainTask()
    {
        foreach (var VARIABLE in GL_PlayerData._instance.TaskConfig.mainTasks)
        {
            if (VARIABLE.status != 1)
            {
                return false;
            }
        }
        return true;
    }


    /// <summary>
    /// 领取奖励回报
    /// </summary>
    /// <param name="number">奖励数值</param>
    /// <param name="ID">任务ID</param>
    /// <param name="type">奖励类型</param>
    public void GetTaskRewardReport(int pagetype,int number, int ID, int type , int taskType ,bool daily , float timeWithDraw = 0,Action actionFresh = null)
    {

        if (daily|| taskType!=0)
        { 
            switch ( (TaskType) taskType) 
            {
            case TaskType.VideoCount: //视频次数
                // GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.DailyTask_AD);
                break;
            case TaskType.Pass: //每日过关
                // GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.DailyTask_Pass);
                break;
            case TaskType.TurnTable: //转盘
                // GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.DailyTask_Draw);
                break;
            case TaskType.RedRain: //红包雨
                // GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.DailyTask_RedRain);
                break;
            case TaskType.RedPack: //免费红包
                    // GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType);
                break;
            case TaskType.Strength: //体力领取
                    // GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType);
                break;
            case TaskType.FlyBox: //飞行宝箱
                // GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.DailyTask_Box);
                break;
            case TaskType.FreeGetMoney: //免费领钱
                    // GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType);
                break;
            case TaskType.DailyTask: //当日完成任务
                // GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.DailyTask_AddUp);
                break;
            case TaskType.SignIn: //签到
                // GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.DailyTask_SignIn);
                break;
            case TaskType.ADClick: //广告点击
                // GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.DailyTask_DownGame);
                break;
            case TaskType.OnLine: //在线奖励
                // GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.DailyTask_Online);
                break;
            case  TaskType.NewPlayer:
                
                break;
            case TaskType.Pass01:
                // GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.DailyMoney_Twenty);
                break;
            case TaskType.Pass02:
                // GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.DailyMoney_Fifty);
                break;
            case TaskType.Pass03:
                // GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.DailyMoney_Hundred);
                break;
            case TaskType.DownLoadIdom:
                // GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.DailyTask_DownIdom);
                break;
            case TaskType.DownLoadIce:
            
                break;
            case TaskType.DownLoadLifeWin:
                // GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.DailyTask_DownLifeWin);
                break; 
            }
        }
        getReward(pagetype,number, ID, type ,daily,timeWithDraw,actionFresh);
    }
    private EFlyItemType _flyItem;
    private void getReward(int pagetype, int number, int ID, int type,bool daily,float timeWithDraw = 0,Action actionFresh = null)
    {
        float count = number;
        // //获取奖励
        // // DDebug.LogError("***** 获取每日任务奖励");
        // Action<int> callBack1 = (int i) =>
        // {
        //     switch ((EItemType) type)
        //     {
        //         case EItemType.Coin:
        //             GL_PlayerData._instance.Coin += number;
        //             _flyItem = EFlyItemType.Coin;
        //             break;
        //         case EItemType.Bogus:
        //             GL_PlayerData._instance.Bogus += number;
        //             _flyItem = EFlyItemType.Bogus;
        //             break;
        //         case EItemType.Strength:
        //             // GL_PlayerData._instance.Strength += number;
        //             // _flyItem = EFlyItemType.Strength;
        //             break;
        //         case EItemType.PromptLimit:
        //             GL_PlayerData._instance.FreeTipCount += number;
        //             _flyItem = EFlyItemType.Null;
        //             break;
        //         case EItemType.CashCoupon:
        //             GL_PlayerData._instance.Dcoupon += number;
        //             _flyItem = EFlyItemType.CashCoupon;
        //             break;
        //     }
        //
        //     GL_GameEvent._instance.SendEvent(EEventID.RefreshCurrency, new EventParam<EFlyItemType>(_flyItem));
        //     // GL_GameEvent._instance.SendEvent(EEventID.RefreshCurrency);
        //     // GL_GameEvent._instance.SendEvent(EEventID.RefreshMainLimit);
        // };
        // //获取奖励
        // // DDebug.LogError("***** 获取每日任务奖励");
        //
        // SRewardData rewardData = new SRewardData((EItemType)type);
        // // GL_RewardLogic._instance.RewardSettlement(rewardData, number);
        // var obj = new object[] { rewardData, number ,callBack1 };
        //
        // UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_GetReward, obj);
        switch ((EItemType) type)
        {
            case EItemType.Coin:
                GL_PlayerData._instance.Coin += number;
                _flyItem = EFlyItemType.Coin;
                break;
            case EItemType.Bogus:
                GL_PlayerData._instance.Bogus += number;
                count /= 100f;
                _flyItem = EFlyItemType.Bogus;
                break;
            case EItemType.Strength:
                // GL_PlayerData._instance.Strength += number;
                // _flyItem = EFlyItemType.Strength;
                break;
        }
        
        // GL_GameEvent._instance.SendEvent(EEventID.RefreshCurrency);
        // GL_GameEvent._instance.SendEvent(EEventID.RefreshMainLimit);
        // UI_HintMessage._.ShowMessage("恭喜领取奖励："+count.ToString("0.00")+"元");
        if (daily)
        {
            //设置奖励参数
            Net_TaskRewardReport report = new Net_TaskRewardReport();
            report.drawNum = number;
            report.taskId = ID;
            report.drawType = type;
            //上报
            GL_ServerCommunication._instance.Send(Cmd.TaskReward, JsonUtility.ToJson(report), (string param) =>
            {
                GL_PlayerData._instance.GetTaskConfig();
            });
        }
        else
        {
            // Net_DrawMilestoneTask report= new Net_DrawMilestoneTask();
            // report.taskId = ID;
            // //上报
            // GL_ServerCommunication._instance.Send(Cmd.MilestoneTaskPost, JsonUtility.ToJson(report), (string param) =>
            // {
            //     GL_PlayerData._instance.GetMilestoneTaskConfig();
            //     
            //     Action action = () => {CoinRepeat();};
            //     UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_GetCoin,new object[]{count,action,timeWithDraw,actionFresh,pagetype});
            // });
        }
        
   
    }

    
    
    private void CoinRepeat()
    {
        // GL_PlayerData._instance.IsPlayIdiomConjCoinRewardAD(true);
        // DDebug.LogError("***** 观看广告成功");
        // GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.Right_AwardDouble);
        //获取多倍奖励
        YS_NetLogic._instance.UpgradeDouble(GL_PlayerData._instance.CurLevel,1, rewards =>
        {
            ShowCoin(rewards);
        });
    }

    private void ShowCoin(Rewards rewards)
    {
        GL_PlayerData._instance.Coin += rewards.num;
        GL_GameEvent._instance.SendEvent(EEventID.RefreshCurrency,new EventParam<EFlyItemType>(EFlyItemType.Coin));
        // if (GL_PlayerData._instance._canWithDraw)
        // {
        //     GL_PlayerData._instance._showWithDraw = false;
        //     UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_Withdraw);
        // }
        string text = "获得奖励金币：" + rewards.num;
        UI_HintMessage._.ShowMessage(/*_uiIfMain.transform,*/text);
       
    }
    
    #region 任务跳转

    

 
    /// <summary>
    /// 播放激励视频
    /// </summary>
    private void Video(Action callback=null)
    {
        //GL_SDK._instance.PopUp();
        GL_AD_Logic._instance.PlayAD(GL_AD_Interface.AD_Reward_GetLevelReward,(delegate(bool b)
        {
            if (b)
            {
                callback ?.Invoke();
            }
        }));
    }
       /// <summary>
        /// 执行任务
        /// </summary>
        /// <param name="eTaskType"></param>
        public void ExeTask(TaskType eTaskType)
        {
            try
            {
                switch (eTaskType)
                {
                    case TaskType.VideoCount:
                        Video();
                        break;
                    case TaskType.Pass:
                        _main.ChangePage("Main");
                        break;
                    case TaskType.TurnTable:
                        break;
                    case TaskType.RedRain:
                        // UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_RedRain);
                        break;
                    case TaskType.RedPack:
                        //打开可领红包  停用
                        // _main.OnClickGetRedEnvelope();
                        break;
                    case TaskType.Strength:
                        break;
                    case TaskType.FlyBox:
                        //判断飞行宝箱 并且打开
                        // _main.FlyTaskClick();
                        break;
                    case TaskType.FreeGetMoney:
                        //免费领钱
                        // _main.GetFreeMoney();
                        break;
                    case TaskType.DailyTask:
                        UI_HintMessage._.ShowMessage(/*_main.transform,*/"继续完成任务可以领取奖励");
                        break;
                    case TaskType.SignIn:
                        //打开签到界面
                        // UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_NewSignInPage);
                        // GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.Main_SignIn);
                        break;
                    case TaskType.OnLine:
                        //判断在线奖励是否可领取，直接领取  
                        //停用
                        // _main.OnClickOnline();
                        break;
                    case TaskType.ADClick:
                        Video();
                        break;
                    case TaskType.NewPlayer:
                        break;
                }
            }
            catch 
            {
               SetUIMain();
               ExeTask(eTaskType);
            }
         
            
            // YS_NetLogic._instance.RefreshTask();
        }
       
       // //打开任务页面
       // private void ShowUITask()
       // {
       //     _main.ChangePage("Task");
       // }
       
       
       #endregion
}



// /// <summary>
// /// 任务类型
// /// </summary>
// public enum ETaskType
// {
//     /// <summary>
//     /// 视频次数
//     /// </summary>
//     Video = 1,
//     /// <summary>
//     /// 每日过关
//     /// </summary>
//     Pass = 2,
//     /// <summary>
//     /// 转盘
//     /// </summary>
//     Draw = 3,
//     /// <summary>
//     /// 红包雨
//     /// </summary>
//     RedRain = 4,
//     /// <summary>
//     /// 免费红包
//     /// </summary>
//     RedPack = 5,
//     /// <summary>
//     /// 体力领取
//     /// </summary>
//     Energy = 6,
//     /// <summary>
//     /// 飞行宝箱
//     /// </summary>
//     FlyBox = 7,
//     /// <summary>
//     /// 免费领钱
//     /// </summary>
//     GetMoney = 8,
//     /// <summary>
//     /// 完成全部任务
//     /// </summary>
//     AllFinish = 9,
//     /// <summary>
//     /// 签到
//     /// </summary>
//     SignIn = 10,
//     /// <summary>
//     /// 在线奖励
//     /// </summary>
//     Online = 12,
//     /// <summary>
//     /// 下载其他
//     /// </summary>
//     DownGame = 11,
// }