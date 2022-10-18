#region << 文 件 说 明 >>

/*----------------------------------------------------------------
// 文件名称：UI_IF_GetResult
// 创 建 者：Yangsong
// 创建时间：2022年07月26日 星期二 17:05
// 文件版本：V1.0.0
//===============================================================
// 功能描述：
//		
//
//----------------------------------------------------------------*/

#endregion

using System;
using System.Collections.Generic;
using Logic.Fly;
using SUIFW.Helps;
using UnityEngine;
using UnityEngine.UI;

namespace SUIFW.Diplomats.Common
{
    /// <summary>
    /// 获取单双份奖励
    /// </summary>
    public class UI_IF_GetResult : BaseUIForm
    {
        #region UIField

        private Transform CoinGet;
        private Text _txtGoin;
        
        private Transform RedGet;
        private Text _txtRed;

        #endregion

        #region Override

        public override void Init()
        {
            CurrentUIType.UIForms_ShowMode = UIFormShowMode.Normal;
            CurrentUIType.UIForms_Type = UIFormType.PopUp;
            CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Dark;

            CoinGet = UnityHelper.GetTheChildNodeComponetScripts<Transform>(gameObject, "CoinGet");
            _txtGoin = UnityHelper.GetTheChildNodeComponetScripts<Text>(CoinGet.gameObject, "RewardText");
            
            RedGet = UnityHelper.GetTheChildNodeComponetScripts<Transform>(gameObject, "RedGet");
            _txtRed = UnityHelper.GetTheChildNodeComponetScripts<Text>(RedGet.gameObject, "RewardText");

            RigisterButtonObjectEvent("BtnSure", (go => { OnBtnSure(); }));

        }

        public override void InitData(object data)
        {
            base.InitData(data);
            
            _isActiveMainUp = false;
            _isFinishAd = false;
            if (data is object[] datas)
            {
                if (datas[0] is List<Rewards> config)
                {
                    _curRewards = config;
                    ShowRewards(config);
                }

                if (datas.Length > 1)
                {
                    if (datas[1] is Action action)
                    {
                        _callback = action;
                    }
                }
                
                if (datas.Length > 2)
                {
                    if (datas[2] is bool bl)
                    {
                        _isActiveMainUp = bl;
                    }
                }
                
                if (datas.Length > 3)
                {
                    if (datas[3] is bool playad)
                    {
                        _isFinishAd = playad;
                    }
                }

            }
        }

        public override void RefreshLanguage()
        {

        }

        public override void Refresh(bool recall)
        {
            DDebug.LogError("*****  播放转圈红包奖励原生");
            if (GL_PlayerData._instance._PlayerCostState._costState == CostState.Low)
            {
                GL_AD_Logic._instance.PlayAD(GL_AD_Interface.AD_Native_DragRedPack);
            } 
        }

        public override void onUpdate()
        {

        }

        public override void OnHide()
        {
            base.OnHide();

            GL_AD_Interface._instance.CloseNativeAd();
            
            if (_isFinishAd && GL_PlayerPrefs.GetInt(EPrefsKey.IsReceiveNewPlayer) == 0)
            {
                GL_PlayerData._instance.GetNewPlayerReward();
            }
            
        }

        #endregion

        #region Event

        /// <summary>
        /// 放弃
        /// </summary>
        private void OnBtnSure()
        {
            CloseUIForm();

            if (_curRewards != null)
            {
                GetRewards();
            }
            
            _callback ?.Invoke();
            _callback = null;

            GL_AudioPlayback._instance.Play(19);
        }

        #endregion

        #region CustomField

        public List<Rewards> _curRewards;
        //private Net_CB_VideoRedGet _curRewards;
        private Action _callback;

        /// <summary>
        /// 是否激活mainup
        /// </summary>
        private bool _isActiveMainUp;

        /// <summary>
        /// 是否完成广告播放
        /// </summary>
        private bool _isFinishAd = false;
        
        #endregion

        #region Logic

        //显示奖励
        private void ShowRewards(List<Rewards> rewards)
        {
            CoinGet.SetActive(false);
            RedGet.SetActive(false);
            foreach (var reward in rewards)
            {
                switch ((EItemType)reward.type)
                {
                    case EItemType.Coin:
                        if (reward.num == 0)
                            break;
                        CoinGet.SetActive(true);
                        UI_AnimHelper.AddVauleAnim(EItemType.Coin,_txtGoin, reward.num, 1f, 0);
                        break;
                    case EItemType.Bogus:
                        if (reward.num == 0)
                            break;
                        RedGet.SetActive(true);
                        UI_AnimHelper.AddVauleAnim(EItemType.Bogus,_txtRed, reward.num * 1000 , 0.5f, 0,null,false);
                        break;
                }
            }
        }

        //领取奖励
        private void GetRewards()
        {
            foreach (var reward in _curRewards)
            {
                switch ((EItemType)reward.type)
                {
                    case EItemType.Coin:
                        if (reward.num == 0)
                            break;
                        SRewardData rewardData1 = new SRewardData(EItemType.Coin);
                        GL_RewardLogic._instance.RewardSettlement(rewardData1, reward.num);
                        Fly_Manager._instance.MainUpFly(EFlyItemType.Coin, Vector3.zero,_isActiveMainUp);
                        UI_HintMessage._.ShowMessage($"恭喜您，领取{reward.num}金币！");
                        break;
                    case EItemType.Bogus:
                        if (reward.num == 0)
                            break;
                        SRewardData rewardData2 = new SRewardData(EItemType.Bogus);
                        GL_RewardLogic._instance.RewardSettlement(rewardData2, reward.num);
                        Fly_Manager._instance.MainUpFly(EFlyItemType.Bogus,Vector3.zero,_isActiveMainUp);
                        UI_HintMessage._.ShowMessage($"恭喜您，领取{(reward.num /100f).ToString("0.00") }元红包！");
                        break;
                }
            }

            _curRewards = null;
        }

        #endregion

    }
}