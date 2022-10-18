#region << 文 件 说 明 >>

/*----------------------------------------------------------------
// 文件名称：UI_IF_Fail
// 创 建 者：Yangsong
// 创建时间：2021年11月30日 星期二 19:27
// 文件版本：V1.0.0
//===============================================================
// 功能描述：
//		
//
//----------------------------------------------------------------*/

#endregion

using System;
using System.Collections.Generic;
using DataModule;
using Logic.Fly;
using Logic.System.NetWork;
using SUIFW.Diplomats.Common;
using UnityEngine.UI;

namespace SUIFW.Diplomats.Game
{
    /// <summary>
    /// 失败弹框
    /// </summary>
    public class UI_IF_Fail : BaseUIForm
    {
        #region UIField
        
        private Action<int> _action;

        /// <summary>
        /// 普通领取
        /// </summary>
        //private Action _actionNormal;
        /// <summary>
        /// 获得的红包
        /// </summary>
        private float _rewardCount;
        
        /// <summary>
        /// 奖励红包数值
        /// </summary>
        private int _rewardLevel;


        private List<string> _description = new List<string>()
        {
            "虽然答错，但情有可原\n奖励减半，获得<color=#ff0000> <size=55>{0}</size> </color>元红包",
            "虽然答错，但情有可原\n奖励减半，获得一半金币",
        };
        
        /// <summary>
        /// 金币按键
        /// </summary>
        private Button _coinBtn;

        private Text _rewardText;

        private UI_IF_MainUp _mainUp;

        private Text _doubleText;

        private Text _normalText;
        
        #endregion

        #region Override

        public override void Init()
        {
            CurrentUIType.UIForms_ShowMode = UIFormShowMode.Normal;
            CurrentUIType.UIForms_Type = UIFormType.PopUp;
            CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Dark;

            RigisterButtonObjectEvent("_btnAgain", (go => { OnBtnAgain(); }));

            RigisterButtonObjectEvent("NextPass", (go => { OnBtnClose(); }));
            RigisterButtonObjectEvent("ClosePage", (go => { OnBtnClose(); }));

            _normalText = UnityHelper.GetTheChildNodeComponetScripts<Text>(gameObject, "NextBtnText");
            _doubleText = UnityHelper.GetTheChildNodeComponetScripts<Text>(gameObject, "AgainText");
            _uiIfMain = UIManager.GetInstance().GetMain();
            
            // _coinBtn = UnityHelper.GetTheChildNodeComponetScripts<Button>(gameObject, "CoinBtn");
            // RigisterButtonObjectEvent(_coinBtn, go =>
            // {
            //     OnBtnAgain();
            // });
            _rewardText = UnityHelper.GetTheChildNodeComponetScripts<Text>(gameObject, "RewardCount");
            _mainUp = UIManager.GetInstance().GetMainUp();
        }

        public override void InitData(object data)
        {

            var datas = data as Object[];
            if (datas == null)
                return;
            if (datas.Length > 0 && datas[0] is Action<int> action)
            {
                _action = action;
            }
            if (datas.Length > 1 && datas[1] is Rewards reward)
            {
                switch (reward.type)
                {
                    case (int) EItemType.Bogus:
                        _rewardCount = reward.num / 100f;
                        _rewardText.text = string.Format(_description[0], _rewardCount);
                        _normalText.text = "红包领取";
                        _doubleText.text = "金币领取";
                        break;
                    case (int) EItemType.Coin:
                        _rewardCount = reward.num;
                        _rewardText.text = _description[1];
                        _normalText.text = $"{_rewardCount}金币";
                        _doubleText.text = "领取15000";
                        break;
                    
                }
            }
        }

        public override void RefreshLanguage()
        {

        }

        public override void Refresh(bool recall)
        {
            DDebug.LogError("*****  播放通关奖励原生");
            // GL_AD_Logic._instance.PlayAD(GL_AD_Interface.AD_Banner_AnswerReward);
            if (GL_PlayerData._instance._PlayerCostState._costState == CostState.Low)
            {
                GL_AD_Logic._instance.PlayAD(GL_AD_Interface.AD_Native_LevelReward);
            }
           
           
           
        }

        public override void onUpdate()
        {

        }

        public override void OnHide()
        {
            base.OnHide();
            GL_AD_Interface._instance.CloseNativeAd();
        }

        public override void Hiding()
        {
            base.Hiding();
        }

        #endregion

        #region Event


        private void CB_AD_Double(bool set)
        {
            if (set)
            {
                _action?.Invoke(1);
                _action = null;
            }
            else
            {
                _action?.Invoke(2);
                _action = null;
            }

        }

        /// <summary>
        /// 复活
        /// </summary>
        private void OnBtnAgain()
        {
          
            CloseUIForm();
            GL_PlayerData._instance.IsPlayIdiomConjCoinRewardAD(true);

            GL_AD_Logic._instance.PlayAD(GL_AD_Interface.AD_Reward_Reright, CB_AD_Double);
        }
        
        /// <summary>
        /// 不复活，下一关
        /// </summary>
        private void OnBtnClose()
        {
            CloseUIForm();
            CB_AD_Double(false);
        }
        
        private void ReRight(bool full)
        {
            // GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.Right_AwardDouble);
            //获取多倍奖励
            int level = GL_PlayerData._instance.CurLevel;
            YS_NetLogic._instance.UpgradeDouble(level, 3, (rewards =>
            {
                ShowCoin(rewards);
            }));

            CloseUIForm();
        }
        private void ShowCoin(Rewards rewards)
        {
            // Action callBack1 = () =>
            // {
            //     GL_PlayerData._instance.Bogus += rewards.num;
            //     GL_GameEvent._instance.SendEvent(EEventID.RefreshCurrency,new EventParam<EFlyItemType>(EFlyItemType.Bogus));
            //     GL_GameEvent._instance.SendEvent(EEventID.RefreshMainLimit);
            //     if (GL_PlayerData._instance._canWithDraw)
            //     {
            //         GL_PlayerData._instance._showWithDraw = false;
            //         UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_Withdraw);
            //     }
            //     
            //     //新手引导,没什么办法,  生写吧
            //     GL_GuideManager._instance.TriggerGuide11();
            // };
            // Object[] objects = { rewards.num, callBack1 };
            // UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_Coin,objects);
            
          
            GL_PlayerData._instance.Coin += rewards.num;
           
            GL_GameEvent._instance.SendEvent(EEventID.RefreshCurrency,new EventParam<EFlyItemType>(EFlyItemType.Coin));
            // if (GL_PlayerData._instance._canWithDraw)
            // {
            //     GL_PlayerData._instance._showWithDraw = false;
            //     UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_Withdraw);
            // }
            string text = "获得奖励金币：" + rewards.num;
            UI_HintMessage._.ShowMessage(/*_uiIfMain.transform,*/text);
            //新手引导,没什么办法,  生写吧
            GL_GuideManager._instance.TriggerGuide11();
          
        }
        //private void CB_ReRight()
        //{
        //    YS_NetLogic._instance.SearchRightConfig(() => 
        //    { 
        //        // _uiIfMain.RightContinue();
        //        ShowRightCount();
                
        //   });
            
        //}

        private UI_IF_Main _uiIfMain;
        //private void ShowRightCount()
        //{
        //    _action?.Invoke();
        //    UI_Diplomats._instance.CloseUI(SysDefine.UI_Path_NetLoading);
        //}

        #endregion

        #region CustomField



        #endregion

        #region Logic



        #endregion

    }
}