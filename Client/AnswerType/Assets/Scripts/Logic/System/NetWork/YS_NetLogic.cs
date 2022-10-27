#region << 文 件 说 明 >>

/*----------------------------------------------------------------
// 文件名称：YS_NetLogic
// 创 建 者：Yangsong
// 创建时间：2021年12月01日 星期三 17:52
// 文件版本：V1.0.0
//===============================================================
// 功能描述：
//		
//
//----------------------------------------------------------------*/

#endregion


using System;
using DataModule;
using Newtonsoft.Json;
using SUIFW;
using UnityEngine;

namespace Logic.System.NetWork
{
    /// <summary>
    /// 网络逻辑
    /// </summary>
    public class YS_NetLogic : Singleton<YS_NetLogic>
    {
        #region 体力

        /// <summary>
        /// 查询体力
        /// </summary>
        public void SearchEnergy(Action endCb = null)
        {
            Net_RequesetCommon req = new Net_RequesetCommon();
            
            GL_ServerCommunication._instance.Send(Cmd.GetEnergy, JsonHelper.ToJson(req), (delegate(string json)
            {
                Net_CB_SearchEnergy msg = JsonHelper.FromJson<Net_CB_SearchEnergy>(json);
                if (msg == null)
                    return;
                
                GL_PlayerData._instance.EnergyConfig = msg;
                GL_PlayerData._instance.Strength = msg.strength;
                
                endCb?.Invoke();
            }));
        }

         private int _earnStrengthType;
         private Action<int> _earnEnergy;
        
        /// <summary>
        /// 赚取体力
        /// </summary>
        public void EarnEnergy(int earnStrengthType,Action<int> endCb = null)
        {
            _earnStrengthType = earnStrengthType;
            _earnEnergy = endCb;
            
            if (_earnStrengthType == 1) //少量
            {
                if (GL_PlayerData._instance.EnergyConfig.fewStrengthLimit > 0)
                {
                    GL_PlayerData._instance.EnergyConfig.fewStrengthLimit -= 1;
                    DDebug.Log($"少量体力限制：" + GL_PlayerData._instance.EnergyConfig.fewStrengthLimit);
                }
            }
            else if(_earnStrengthType == 2) //大量
            {
                if (GL_PlayerData._instance.EnergyConfig.largeStrengthLimit > 0)
                {
                    GL_PlayerData._instance.EnergyConfig.largeStrengthLimit -= 1;
                }
            }
            
            UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_NetLoading);
            Net_EarnEnergy();
        }

        private void Net_EarnEnergy()
        {
            Net_Rq_EarnEnergy req = new Net_Rq_EarnEnergy();
            req.earnStrengthType = _earnStrengthType;

            GL_ServerCommunication._instance.Send(Cmd.EarnEnergy, JsonHelper.ToJson(req), CB_EarnEnergy);
        }
        
        private void CB_EarnEnergy(string json)
        {
            int msg = JsonHelper.FromJson<int>(json);

            UI_Diplomats._instance.CloseUI(SysDefine.UI_Path_NetLoading);
            MethodExeTool.CancelInvoke(Net_EarnEnergy);
            
            _earnEnergy?.Invoke(msg);
        }
        
        /// <summary>
        /// 消耗体力
        /// </summary>
        public void CostEnergy(Action endCb = null)
        {
            GL_PlayerData._instance.Strength -= 1;
            endCb?.Invoke();
            
            Net_RequesetCommon req = new Net_RequesetCommon();
            
            GL_ServerCommunication._instance.Send(Cmd.CostEnergy, JsonHelper.ToJson(req));
            
        }

        #endregion

        #region 提现记录

        /// <summary>
        /// 提现记录
        /// </summary>
        public void WithdrawRecord(Action<Net_CB_WithdrawRecordConfig> endCb)
        {
            //UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_NetLoading);
            
            Net_RequesetCommon req = new Net_RequesetCommon();

            GL_ServerCommunication._instance.Send(Cmd.WithdrawRecord, JsonHelper.ToJson(req), (delegate(string json)
            {
                Net_CB_WithdrawRecordConfig msg = JsonHelper.FromJson<Net_CB_WithdrawRecordConfig>(json);
                if (msg == null)
                    return;
                
                endCb?.Invoke(msg);
                
                //UI_Diplomats._instance.CloseUI(SysDefine.UI_Path_NetLoading);
            }));
        }

        #endregion
        
        #region 升级

        private int _level;
        private int _type;
        private Action<Rewards> _upgradeDouble;


        /// <summary>
        /// 升级多倍
        /// </summary>
        public void UpgradeDouble(int level,int type,Action<Rewards> endCb = null)
        {
            _level = level;
            _type = type;    
            _upgradeDouble = endCb;
            
            UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_NetLoading);
            Net_UpgradeDouble();
        }

        public void Net_UpgradeDouble()
        {
            UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_NetLoading);
            Net_Rq_UpgradeDouble req = new Net_Rq_UpgradeDouble();
            req.level = _level;
            req.type = _type;
            // _upgradeDouble = endCb;
            GL_ServerCommunication._instance.Send(Cmd.UpgradeDouble, JsonHelper.ToJson(req), CB_UpgradeDouble);
        }

        private void CB_UpgradeDouble(string json)
        {
            // DDebug.LogError("***** 服务器回调");
            Net_CB_Reward msg = JsonHelper.FromJson<Net_CB_Reward>(json);
            if (msg == null)
                return;
            UI_Diplomats._instance.CloseUI(SysDefine.UI_Path_NetLoading);
            MethodExeTool.CancelInvoke(Net_UpgradeDouble);
            _upgradeDouble?.Invoke(msg.rewards[0]);
        }

        #endregion

        #region 打卡

        // /// <summary>
        // /// 获取今日打卡配置
        // /// </summary>
        // /// <param name="action"></param>
        // public void SearchClockin(Action action = null)
        // {
        //
        //     Net_RequesetCommon req = new Net_RequesetCommon();
        //     GL_ServerCommunication._instance.Send(Cmd.ClockinConfig, JsonHelper.ToJson(req), (delegate(string json)
        //     {
        //         Net_CB_ClockinConfig msg = JsonHelper.FromJson<Net_CB_ClockinConfig>(json);
        //         if (msg == null)
        //             return;
        //         
        //         GL_PlayerData._instance.SigNetCbClockinConfig = msg;
        //         // GL_PlayerData._instance.DrawLimit = msg.drawLimit;
        //         
        //         action?.Invoke();
        //     }));
        // }
        // /// <summary>
        // /// 签到
        // /// </summary>
        // /// <param name="action"></param>
        // public void Clockin(Action action=null)
        // {
        //     action += () =>
        //     {
        //         UI_Diplomats._instance.CloseUI(SysDefine.UI_Path_NetLoading);
        //     };
        //     UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_NetLoading);
        //     // float plan = (float) GL_PlayerData._instance.SystemConfig.viewAds /
        //     //              GL_PlayerData._instance.SigNetCbClockinConfig.needViewAd;
        //     // if (plan<1)
        //     // {
        //     //     action?.Invoke();
        //     //     return;
        //     // }
        //
        //     if (GL_PlayerData._instance.SigNetCbClockinConfig.hasClock!=1)
        //     {
        //         GL_PlayerData._instance.ClockInReport(action);
        //     }
        //    
        // }
        #endregion
        
        #region 存钱罐

        /// <summary>
        /// 存钱罐配置
        /// </summary>
        public void GoldenpigConfig(Action<Net_CB_GoldenpigConfig> endCb)
        {
            UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_NetLoading);
            
            Net_RequesetCommon req = new Net_RequesetCommon();
            
            GL_ServerCommunication._instance.Send(Cmd.GoldenpigConfig, JsonHelper.ToJson(req), (delegate(string json)
            {
                Net_CB_GoldenpigConfig msg = JsonHelper.FromJson<Net_CB_GoldenpigConfig>(json);
                if (msg == null)
                    return;
                GL_PlayerData._instance._goldenpig = msg;
                endCb?.Invoke(msg);
                
                UI_Diplomats._instance.CloseUI(SysDefine.UI_Path_NetLoading);
            }));
        }

        /// <summary>
        /// 存钱罐提现
        /// </summary>
        /// <param name="endCb"></param>
        public void GoldenpigWithdraw(Action endCb)
        {
            Net_RequesetCommon req = new Net_RequesetCommon();
            
            GL_ServerCommunication._instance.Send(Cmd.GoldenpigWithdraw, JsonHelper.ToJson(req), (delegate(string json)
            {
                endCb?.Invoke();
            }));
        }

        #endregion
    }
}