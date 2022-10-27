#region << 文 件 说 明 >>

/*----------------------------------------------------------------
// 文件名称：UI_IF_Goldenpig
// 创 建 者：Yangsong
// 创建时间：2022年02月24日 星期四 14:50
// 文件版本：V1.0.0
//===============================================================
// 功能描述：
//		
//
//----------------------------------------------------------------*/

#endregion

using System;
using Logic.Fly;
// using Helper;
using Logic.System.NetWork;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using SUIFW.Diplomats.Common;
using UnityEngine;
using UnityEngine.UI;
using Object = System.Object;

namespace SUIFW.Diplomats.Main
{
    /// <summary>
    /// 存钱罐
    /// </summary>
    public class UI_IF_Goldenpig : BaseUIForm
    {
        #region UIField

        // private UI_NumberImage _txtWithdrawMax;
        //
        // private UI_NumberImage _txtWithdrawGet;

        private Text _txtTip;

        private Button _btnWithdraw;

        private Image _btnWithdrawImg;

        private Text _btnWithdrawText;

        private string _tips =
            "今日已经过<color=#fff000><size=55>{0}</size></color>关，还差<color=#fff000><size=55>{1}</size></color>关提现。";
        public Sprite[] BtnIcon;
        public string[] _btnTips = {"<color=#01AF9D>立即提现</color>","<color=#b97408>明日可提现</color>" };

        /// <summary>
        /// 元宝数量
        /// </summary>
        private Text _coinCount;

        private Action _action;
        
        #endregion

        #region Override

        public override void Init()
        {
            CurrentUIType.UIForms_ShowMode = UIFormShowMode.Normal;
            CurrentUIType.UIForms_Type = UIFormType.PopUp;
            CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Light;

            _txtTip = UnityHelper.GetTheChildNodeComponetScripts<Text>(gameObject, "_txtTip");
            // _txtWithdrawGet = UnityHelper.GetTheChildNodeComponetScripts<UI_NumberImage>(gameObject, "_txtWithdrawGet");
            // _txtWithdrawGet.Init(true);
            //
            // _txtWithdrawMax = UnityHelper.GetTheChildNodeComponetScripts<UI_NumberImage>(gameObject, "_txtWithdrawMax");
            // _txtWithdrawMax.Init(true);

            RigisterButtonObjectEvent("_btnClose", (go =>
            {
                CallBack();
                OnBtnClose();
            }));
            _coinCount = UnityHelper.GetTheChildNodeComponetScripts<Text>(gameObject, "CoinCount");
            _btnWithdraw = UnityHelper.GetTheChildNodeComponetScripts<UI_Button>(gameObject, "_btnWithdraw");
            _btnWithdrawImg = _btnWithdraw.GetComponent<Image>();
            _btnWithdrawText = _btnWithdraw.GetComponentsInChildren<Text>()[0];
            RigisterButtonObjectEvent(_btnWithdraw, (go => { OnBtnWithdraw(); }));
        }

        public override void InitData(object data)
        {
            base.InitData(data);

            var datas = data as Object[];
            if (datas == null) 
                return;
            
            if (datas[0] is Net_CB_GoldenpigConfig goldenpigConfig)
            {
                _goldenpigConfig = null;
                _goldenpigConfig = goldenpigConfig;
            }

            // if (datas[1] is Action action)
            // {
            //     _action = action;
            // }
            
            RefreshUI();
        }

        private void CallBack()
        {
            _action?.Invoke();
            _action = null;
        }

        public override void RefreshLanguage()
        {

        }

        public override void Refresh(bool recall)
        {
            // GL_AD_Logic._instance.PlayAD(GL_AD_Interface.AD_Banner_GoldenPig); //原生广告
            GL_AD_Logic._instance.PlayAD(GL_AD_Interface.AD_Native_LevelReward); 
        }

        public override void onUpdate()
        {

        }

        public override void OnHide()
        {
            base.OnHide();
            // GL_AD_Interface._instance.CloseBannerAd();
            GL_AD_Interface._instance.CloseNativeAd();
        }

        #endregion

        #region Event

        /// <summary>
        /// 关闭
        /// </summary>
        private void OnBtnClose()
        {
            CloseUIForm();
        }

        /// <summary>
        /// 放弃
        /// </summary>
        private void OnBtnWithdraw()
        {
            Withdraw();
        }

        #endregion

        #region CustomField

        private Net_CB_GoldenpigConfig _goldenpigConfig;

        private string _tipFormat = "今日已经过<color=#fff000><size=30>{0}</size></color>关，" +
                                    "还差<color=#fff000><size=35>{1}</size></color>关提现。";

        /// <summary>开始提现上限</summary>
        private int _startWithdrawMax = 0;
        
        /// <summary>增加提现限制关卡</summary>
        private int _addLimit = 10;
        
        /// <summary>一次加多少</summary>
        private float _oneTimeAdd = 0.1f;

        #endregion

        #region Logic

        private void RefreshUI()
        {
            WithdrawMax();
            
            if (_goldenpigConfig == null)
            {
                return;
            }

            if (_goldenpigConfig.hasWithdraw == 1)
            {
                // _txtWithdrawGet.SetNumber(_goldenpigConfig.storeMoney * 0.01f);
            }
            else if (_goldenpigConfig.hasWithdraw == 2)
            {
                // _txtWithdrawGet.SetNumber(_goldenpigConfig.money * 0.01f);
            }

            
            if (_goldenpigConfig.needLeve <= 0)
            {
                _txtTip.SetActive(false);
            }
            else
            {
                _txtTip.text = _tipFormat;
                _txtTip.text = string.Format(_tips, GL_PlayerData._instance.SystemConfig.userDayLevel, _goldenpigConfig.needLeve);
            }

            if (IsCanWithdraw(false,out float num))
            {
                //立即提现
                _btnWithdrawImg.sprite = BtnIcon[0];
                _btnWithdrawText.text = _btnTips[0];
            }
            else
            {
                //明日可提现
                _btnWithdrawImg.sprite = BtnIcon[1];
                _btnWithdrawText.text = _btnTips[1];
            }

            _coinCount.text = WithdrawMax().ToString() + "元";
        }

        private float WithdrawMax()
        {
            float number = 0;
            if (_goldenpigConfig.hasWithdraw == 1)
            {
                number = _goldenpigConfig.storeMoney / 100f;
            }
            else
            {
                number = _goldenpigConfig.money / 100f;
            }
            
            return number;
        }

        
        
        private void Withdraw()
        {
            if (!IsCanWithdraw(true,out float num))
            {
                return;
            }
            

            if (!GL_PlayerData._instance.IsLoginWeChat())
            {
                //登陆微信
                UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_WeChatLogin);
            }
            else
            {
                //提现
                YS_NetLogic._instance.GoldenpigWithdraw((delegate
                {
                    UI_HintMessage._.ShowMessage($"成功提现{num}元");
                    // _txtWithdrawGet.SetNumber(_goldenpigConfig.storeMoney);
                    _goldenpigConfig.hasWithdraw = 1;
                    _btnWithdrawImg.sprite = BtnIcon[1];
                    _btnWithdrawText.text = _btnTips[1];
                    _coinCount.text = WithdrawMax()+ "元";
                }));
            }
        }

        /// <summary>
        /// 是否能够提现
        /// </summary>
        /// <returns></returns>
        private bool IsCanWithdraw(bool isTip,out float num)
        {
            num = _goldenpigConfig.money / 100f;
            
            if (_goldenpigConfig == null)
            {
                return false;
            }
            
            if (num <= 0)
            {
                if (isTip) UI_HintMessage._.ShowMessage($"请明日再来领取");
                return false;
            }

            if (_goldenpigConfig.needLeve > 0)
            {
                if (isTip) UI_HintMessage._.ShowMessage(/*$"今日已经过{GL_PlayerData._instance.CurLevel}关，" +*/
                                                        $"还差{_goldenpigConfig.needLeve}关领取奖励");
                return false;
            }

            // if (num < 0.3f)
            // {
            //     if (isTip) UI_HintMessage._.ShowMessage($"最低提现0.3元。");
            //     return false;
            // }
            
            if (_goldenpigConfig.hasWithdraw == 1)
            {
                if (isTip) UI_HintMessage._.ShowMessage($"今日已领取");
                return false;
            }
            
            return true;
        }

        #endregion

    }
}