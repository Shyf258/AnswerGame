#region << 文 件 说 明 >>

/*----------------------------------------------------------------
// 文件名称：UI_IF_OpenRedPack
// 创 建 者：Yangsong
// 创建时间：2021年12月01日 星期三 11:10
// 文件版本：V1.0.0
//===============================================================
// 功能描述：
//		
//
//----------------------------------------------------------------*/

#endregion

using System;
using UnityEngine;
using UnityEngine.UI;

namespace SUIFW.Diplomats.Common
{
    public class UI_IF_OpenRedPack : BaseUIForm
    {
        #region UIField
        private bool _isIgnoreAD = false;   //是否跳过广告
        private bool _isShowClose = true;   //是否显示关闭按钮

        private Button _btnClose;

        /// <summary>是否成功播放了激励视频</summary>
        private bool _isPlayVideo;

        private Image _openImg;
        private Text _openContent;
        public Sprite[] Sprites;
        private string[] _contents = new[] {"观看完整视频获得高额奖励","无需看视频即可领取奖励"};

        private ERewardSource _curRewardSource;
        
        #endregion

        #region Override

        //private Animator _animator;
        public override void Init()
        {
            CurrentUIType.UIForms_ShowMode = UIFormShowMode.Normal;
            CurrentUIType.UIForms_Type = UIFormType.PopUp;
            CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Dark;

            var _btnOpenRedPack = UnityHelper.GetTheChildNodeComponetScripts<UI_Button>(gameObject, "_btnOpenRedPack");
            _openImg = _btnOpenRedPack.GetComponent<Image>();
            RigisterButtonObjectEvent(_btnOpenRedPack, (go => { OnBtnOpenRedPack(); }));
            _btnClose = UnityHelper.GetTheChildNodeComponetScripts<Button>(gameObject, "_btnClose");

            RigisterButtonObjectEvent(_btnClose, (go => { OnBtnClose(); }));
            _openContent = UnityHelper.GetTheChildNodeComponetScripts<Text>(gameObject, "RewardDescription");
        }

        public override void InitData(object data)
        {
            base.InitData(data);
            
            _isIgnoreAD = false;
            _isShowClose = true;

            var datas = data as object[];
            if (datas == null)
                return;
            
            //奖励
            if (datas.Length > 0 && datas[0] is Action<bool> action)
            {
                _rewardCallBack = action;
            }
            if (datas.Length > 1 && datas[1] is bool show)
            {
                _isShowClose = show;
            }

            if (datas.Length > 2 && datas[2] is bool set)
            {
                _isIgnoreAD = set;
            }
            
            if (datas.Length > 3 && datas[3] is ERewardSource rewardSource)
            {
                _curRewardSource = rewardSource;
            }

            Show();
        }

        private void Show()
        {
            switch (_curRewardSource)
            {
                case ERewardSource.OpenRed:
                    _openImg.sprite = Sprites[0];
                    _openContent.text = _contents[0];
                    break;
                case ERewardSource.Guide:
                    int showMode =0;
                    // if (_isIgnoreAD)
                    // {
                    //     showMode = 1;
                    // }
                    showMode = 1;
                    _openImg.sprite = Sprites[showMode];
                    _openContent.text = _contents[showMode];
                    break;
            }
        }
        
        public override void PoppedRefresh()
        {
            base.PoppedRefresh();
            _animator.Play("UI_IF_PopUp_Shake");
        }
        public override void OnHide()
        {
            base.OnHide();

            _rewardCallBack = null;
        }

        public override void RefreshLanguage()
        {

        }

        public override void Refresh(bool recall)
        {
            _btnClose.gameObject.SetActive(_isShowClose);
            if(GL_Game._instance.GameState == EGameState.GameMain
                && GL_PlayerData._instance.CurLevel > 1)
                // GL_AD_Logic._instance.PlayAD(GL_AD_Interface.AD_Banner_GoldenPig);
                GL_AD_Logic._instance.PlayAD(GL_AD_Interface.AD_Native_LevelReward); 
            _isPlayVideo = false;
        }
        public override void Hiding()
        {
            base.Hiding();
            if (GL_Game._instance.GameState == EGameState.GameMain)
                // GL_AD_Interface._instance.CloseBannerAd();
                GL_AD_Interface._instance.CloseNativeAd();
        }

        public override void onUpdate()
        {

        }

        #endregion

        #region Event

        private float _nextOpenRed;
        /// <summary>
        /// 打开红包
        /// </summary>
        private void OnBtnOpenRedPack()
        {
            //播放打开音效
            GL_AudioPlayback._instance.Play(8);
            
            
            //----------------------- 按键间隔

            if (_nextOpenRed<=0)
            {
                _nextOpenRed = Time.time;
            }
            else
            {
                if (GL_PlayerData._instance.ClickBtn( _nextOpenRed,GL_PlayerData._instance.CbPlayAd.waitTime))
                {
                    _nextOpenRed = Time.time;
                    // DDebug.LogError("***** 当前按键生效！");
                }
                else
                {
                    UI_HintMessage._.ShowMessage(
                        "冷却中，请稍等"
                    );
                    // DDebug.LogError("***** 当前按键冷却中...");
                    return;
                }
            }
            
            if(_isIgnoreAD)
            {
                DDebug.LogError("~~~CloseUIForm");
                CloseUIForm();
                _rewardCallBack?.Invoke(true);
                _rewardCallBack = null;
            }
            else
            {
                if (_curRewardSource == ERewardSource.OpenRed)
                {
                    GL_AD_Logic._instance.PlayAD(GL_AD_Interface.AD_Reward_OpenRedPack, (set) =>
                    {
                        ADCallback(set);
                    });
                }
                else if (_curRewardSource == ERewardSource.Guide)
                {
                    // GL_AD_Logic._instance.PlayAD(GL_AD_Interface.AD_Reward_CommonOpenRed, (set) =>
                    // {
                    //     ADCallback(set);
                    // });
                    
                    ADCallback(true);
                }
            }
        }

        private void ADCallback(bool isSuccess)
        {
            CloseUIForm();
            _isPlayVideo = isSuccess;
            if (isSuccess)
                _rewardCallBack?.Invoke(true);
            _rewardCallBack = null;
        }

        /// <summary>
        /// 关闭
        /// </summary>
        private void OnBtnClose()
        {
            // if (IsPlayAd(false))
            // {
            //     GL_AD_Logic._instance.PlayAD(GL_AD_Interface.AD_Reward_Reright, (set) =>
            //     {
            //         DoClose();
            //         if (set)
            //             IsPlayAd(true);
            //     });
            // }
            // else
            // {
            //     DoClose();
            // }
                
            
            // //需求只在主页才弹出插屏
            // if (_sdkAD != null && !_isPlayVideo && GL_Game._instance.GameState == EGameState.Playing)
            //     _sdkAD.PlayAD(GL_AD_Interface.AD_Interstitial_OpenRedBack,180);
            
            CloseUIForm();
        }

        private void DoClose()
        {
            CloseUIForm();
            _rewardCallBack?.Invoke(false);
        }
        #endregion

        #region CustomField

       

        #endregion

        #region Logic

        /// <summary>
        /// 奖励回调，bool是否在游戏界面
        /// </summary>
        private Action<bool> _rewardCallBack;
        
        /// <summary>
        /// 是否播放间隔次数广告
        /// </summary>
        /// <returns></returns>
        private bool IsPlayAd(bool isReset, bool isAddNumber = true)
        {
            return GL_PlayerData._instance.IsPlayIdiomConjCoinRewardAD(isReset,isAddNumber);
        } 
        
        #endregion

    }
}