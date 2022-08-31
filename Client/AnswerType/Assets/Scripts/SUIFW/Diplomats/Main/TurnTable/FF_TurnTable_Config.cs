//11.24 转盘配置

using SUIFW;
using System;
using System.Collections;
using System.Collections.Generic;
using Logic.System.NetWork;
using UnityEngine;
using Object = System.Object;

public class FF_TurnTable_Config  
{
    // private Net_CB_DrawConfig _drawConfig;
    private Net_CB_Redpacks _redpacks;
    public  List<EItemType> _itemList = new List<EItemType>();

    public void Init()
    {
      
        _itemList.Add(EItemType.Coin);
        _itemList.Add(EItemType.Coin);
        _itemList.Add(EItemType.Bogus);
        _itemList.Add(EItemType.Strength);
        _itemList.Add(EItemType.PromptLimit);
        _itemList.Add(EItemType.Bogus);
        _itemList.Add(EItemType.CashCoupon);
        // YS_NetLogic._instance.SearchDraw();
        GetRedPackConfig();
    }

    #region 读取配置
    // /// <summary>
    // /// 读取转盘配置
    // /// </summary>
    // public void GetConfig()
    // {
    //     Net_RequesetCommon com = new Net_RequesetCommon();
    //     GL_ServerCommunication._instance.Send(Cmd.DrawConfig, JsonUtility.ToJson(com), DrawConfig);
    // }
    // public void DrawConfig(string json)
    // {
    //     Net_CB_DrawConfig msg = JsonUtility.FromJson<Net_CB_DrawConfig>(json);
    //     if (msg == null)
    //         return;
    //     _drawConfig = msg;
    //     // DDebug.Log("****** 读取配置" +json);
    // }
    
    
    /// <summary>
    /// 读取抽奖红包配置
    /// </summary>
    public void GetRedPackConfig()
    {
        Net_RequesetCommon com = new Net_RequesetCommon();
        GL_ServerCommunication._instance.Send(Cmd.RedpPacks, JsonUtility.ToJson(com), RedPackConfig);
    }
    
    public void RedPackConfig(string json)
    {
        Net_CB_Redpacks message = JsonUtility.FromJson<Net_CB_Redpacks>(json);
        if (message == null) 
            return;
        _redpacks = message;
    }
    #endregion
   


    #region 接口

    // //抽奖总次数
    // public int GetDrawTotalNumber()
    // {
    //
    //     if (_drawConfig == null)
    //     {
    //         DDebug.LogError("***** 获取值失败");
    //         return 0;
    //     }
    //
    //     return _drawConfig.drawLimit;
    // }
    
    // /// <summary>
    // /// 抽奖配置
    // /// </summary>
    // /// <returns></returns>
    // public Net_CB_DrawConfig GetDrawConfig()
    // {
    //     if (_drawConfig ==null)
    //     {
    //        DDebug.LogError("***** 抽奖配置获取失败");
    //     }
    //     return _drawConfig;
    // }

    public Net_CB_Redpacks GetRedPacks()
    {
        if (_redpacks==null)
        {
            // DDebug.LogError("***** 抽奖红包配置获取失败");
        }
        return _redpacks;
        
    }
    

    //广告播放成功, 激励生效
    public void CB_PlayAD()
    {
        //显示实际奖励界面

        //发送 提交抽奖接口,  服务器消耗
    }

    #endregion

    //跟进当前奖励数值, 随机数值下标
    public int RangeReward()
    {
        List<int> list = new List<int>();
        for (int i = 0; i < _itemList.Count; i++)
        {
            if ((int)_itemList[i] == GL_PlayerData._instance.DrawConfig.winRewardType)
            {
                list.Add(i);
            }
        }
        if (list.Count == 0)
            return 0;
        return GL_Tools.RandomList(list);
    }


    /// <summary>
    /// 转盘上报数据 获取到奖励
    /// </summary>
    public void ReportDraw()
    {
        //获取奖励
        
        var config = GL_PlayerData._instance.DrawConfig;
        SRewardData rewardData = new SRewardData((EItemType)config.winRewardType);
        GL_RewardLogic._instance.RewardSettlement(rewardData,config.winNum);

        
        // var obj = new object[] { rewardData, GL_PlayerData._instance.DrawConfig.winNum };
        // UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_GetReward, obj);
        
        // YS_NetLogic._instance.DrawWindraw(config,2,(delegate(int num)
        // {
        //     Action<int> callBack1 = (int i) =>
        //     {
        //         switch ((EItemType)config.winRewardType)
        //         {
        //             case EItemType.Coin:
        //                 GL_PlayerData._instance.Coin += config.winNum;
        //                 break;
        //             case EItemType.Bogus:
        //                 GL_PlayerData._instance.Bogus += config.winNum;
        //                 break;
        //             case EItemType.Strength:
        //                 GL_PlayerData._instance.Strength += config.winNum;
        //                 break;
        //             case EItemType.PromptLimit:
        //                 GL_PlayerData._instance.FreeTipCount += config.winNum;
        //                 break;
        //             case EItemType.CashCoupon:
        //                 GL_PlayerData._instance.Dcoupon += config.winNum;
        //                 break;
        //         }
        //         
        //         GL_GameEvent._instance.SendEvent(EEventID.RefreshCurrency);
        //         GL_GameEvent._instance.SendEvent(EEventID.RefreshMainLimit);
        //     };
        //     Object[] objects = { rewardData,config.winNum,callBack1 };
        //     UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_GetReward,objects);
        // }));
        
        //数据上报
        // #region TurbTable_Coinfg 读取的当前参数
        //
        // // Net_RequesetCommon com = new Net_RequesetCommon();
        // // GL_ServerCommunication._instance.Send(Cmd.DrawConfig, JsonUtility.ToJson(com), CB_GetConfig);
        //
        // Net_GetTableReward reward = new Net_GetTableReward();
        //
        // //***** 设置上传参数
        // reward.drawNum = GL_PlayerData._instance.DrawConfig.winNum;   //抽中的红包数量
        // reward.type = 2; //领取的类型 是否翻倍
        // reward.drawType = GL_PlayerData._instance.DrawConfig.winRewardType; // 抽中的奖励类型
        // #endregion
        // GL_ServerCommunication._instance.Send(Cmd.DrawWindraw, JsonUtility.ToJson(reward));
        // //更新配置
        // YS_NetLogic._instance.SearchDraw();
        
        
        GetRedPackConfig();
    }
    /// <summary>
    /// 开启转盘红包
    /// </summary>
    public void OpenRed(int id)
    {
        
        
        Net_OpenRedReward reward = new Net_OpenRedReward();
        //设置上传参数
        reward.drawNum = _redpacks.redPacks[id].winNum;
        reward.packId = _redpacks.redPacks[id].id;
        reward.drawType = _redpacks.redPacks[id].winRewardType;
        Debug.LogError(JsonUtility.ToJson(reward));
        //上传
        GL_ServerCommunication._instance.Send(Cmd.OpenRedPack, JsonUtility.ToJson(reward));
        
        //获取奖励
        SRewardData rewardData = new SRewardData((EItemType)_redpacks.redPacks[id].winRewardType);
        GL_RewardLogic._instance.RewardSettlement(rewardData, _redpacks.redPacks[id].winNum);

        var obj = new object[] { rewardData, _redpacks.redPacks[id].winNum };
        // UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_GetReward, obj);
        
        //更新配置
        GetRedPackConfig();
    }


}


