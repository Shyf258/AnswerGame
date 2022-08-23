#region << 文 件 说 明 >>

/*----------------------------------------------------------------
// 文件名称：UI_IF_Coin
// 创 建 者：Yangsong
// 创建时间：2021年12月01日 星期三 14:58
// 文件版本：V1.0.0
//===============================================================
// 功能描述：
//		
//
//----------------------------------------------------------------*/

#endregion

using System;
using Helpser;
using Logic.Fly;
using UnityEngine;
using UnityEngine.UI;

namespace SUIFW.Diplomats.Common
{
    /// <summary>
    /// 获得金币
    /// </summary>
    public class UI_IF_GetCoin : BaseUIForm
    {
        #region UIField

        /// <summary> 获得奖励 </summary>
        private Text _txtGetReward;

        private Action _action;

        private Action _actionHide;

        private TaskPageType _taskPageType;
        // private Button _getReward;
        //
        // private Button _closePage;
        // /// <summary> 总金币 </summary>
        // private Text _txtTotalCoin;

        // /// <summary> 飞行位置 </summary>
        // private Image _flyPos;

        // private Action _rewardCallBack;

        #endregion

        #region Override

        public override void Init()
        {
            CurrentUIType.UIForms_ShowMode = UIFormShowMode.Normal;
            CurrentUIType.UIForms_Type = UIFormType.PopUp;
            CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Dark;
            // _isOpenMainUp = true;

            // _txtGetCoin = UnityHelper.GetTheChildNodeComponetScripts<Text>(gameObject, "_txtGetCoin");
            //
            // _txtTotalCoin = UnityHelper.GetTheChildNodeComponetScripts<Text>(gameObject, "_txtTotalCoin");
            
            // _flyPos = UnityHelper.GetTheChildNodeComponetScripts<Image>(gameObject, "_flyPos");

            _txtGetReward = UnityHelper.GetTheChildNodeComponetScripts<Text>(gameObject, "RewardCount");
            

            RigisterButtonObjectEvent("GetRewardBtn", (go => { OnBtnGetReward(); }));

            RigisterButtonObjectEvent("ClosePage", (go => { OnBtnCloseBot(); }));
            
            RigisterButtonObjectEvent("CloseTop", (go => { OnBtnCloseBot(); }));
        }

        public override void InitData(object data)
        {
            base.InitData(data);

            if (data is object[] datas)
            {
                if (datas.Length > 0 && datas[0] != null)
                {
                    float num = (float) datas[0];
                    
                    _txtGetReward.text = string.Format("恭喜获得<color=#ff0000>{0}</color>元现金红包",num);
                }
                if (datas.Length > 1 && datas[1] != null)
                {
                    _action = (Action)datas[1];
                }

                if (datas.Length > 2 && datas[2] != null)
                {
                    float count = (float) datas[2];
                    if (count>0)
                    {
                        _txtGetReward.text = string.Format("恭喜您成功获得<color=#ff0000>{0}</color>元现金红包提现机会",count);
                    }
                   
                    
                }
                if (datas.Length > 3 && datas[3] != null)
                {
                    _actionHide = (Action)datas[3];
                }
                if (datas.Length > 4 && datas[4] != null)
                {
                    _taskPageType = (TaskPageType)datas[4] ;
                }
            }
        }

        public override void OnHide()
        {
            base.OnHide();
          
            GL_GameEvent._instance.SendEvent(EEventID.RefreshCurrency, new EventParam<EFlyItemType>(EFlyItemType.Bogus));
            // GL_AD_Interface._instance.CloseBannerAd();
            GL_AD_Interface._instance.CloseNativeAd();
            if ( _actionHide!= null)
            {
                _actionHide?.Invoke();
                
                _actionHide = null;
            }
        }

        public override void RefreshLanguage()
        {

        }

        public override void Refresh(bool recall)
        {
            GL_AudioPlayback._instance.PlayTips(11);
            GL_AudioPlayback._instance.Play(7);
            // GL_AD_Logic._instance.PlayAD(GL_AD_Interface.AD_Banner_MyCoin); 
            GL_AD_Logic._instance.PlayAD(GL_AD_Interface.AD_Native_LevelReward); 
        }

        public override void onUpdate()
        {

        }

        #endregion

        #region Event

        private void OnBtnGetReward()
        {
            string adsite ;
            switch ( _taskPageType)
            {
                case TaskPageType.RedPage:
                    adsite = GL_AD_Interface.AD_Reward_TaskRedPage;
                    break;
                case TaskPageType.TaskPage:
                    adsite = GL_AD_Interface.AD_Reward_TaskNormal;
                    break;
                default:
                    adsite = GL_AD_Interface.AD_Reward_GetLevelReward;
                    break;
            }
            GL_SDK._instance.PopUp();
            GL_AD_Logic._instance.PlayAD(adsite, go =>
            {
                _action?.Invoke();
                CloseUIForm();
            });
          
        }

        /// <summary>
        /// 关闭
        /// </summary>
        private void OnBtnCloseBot()
        {
            CloseUIForm();
        }

        #endregion

        #region CustomField



        #endregion

        #region Logic



        #endregion

    }

    public enum TaskPageType
    {
        RedPage  = 1 ,
        TaskPage = 2 ,
    }
}