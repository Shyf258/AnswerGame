#region << 文 件 说 明 >>

/*----------------------------------------------------------------
// 文件名称：UI_Btn_MyWithdrawItem
// 创 建 者：Yangsong
// 创建时间：2022年04月07日 星期四 15:27
// 文件版本：V1.0.0
//===============================================================
// 功能描述：
//		
//
//----------------------------------------------------------------*/

#endregion

using System;
using System.Collections.Generic;
using SUIFW.Diplomats.Common;
using UnityEngine;
using UnityEngine.UI;

namespace SUIFW.Diplomats.Main.MyWithdraw
{
    /// <summary>
    /// 我的提现组件B版
    /// </summary>
    public class UI_Btn_NewWithdrawItemB : UIObjectBase
    {
        #region UIField

        /// <summary> 金额 </summary>
        private Text _txtMoney;
        private string _moneyFormat = "{0}<size=45>元</size>";
        //b新增
        private Slider _sldVideo;
        private Text _txtNeedVideo;
        private Text _sldTextVideo;

        #endregion

        #region Override

        public override void InitObjectNode()
        {
            _txtMoney = UnityHelper.GetTheChildNodeComponetScripts<Text>(gameObject, "_txtMoney");
            _sldVideo = UnityHelper.GetTheChildNodeComponetScripts<Slider>(gameObject, "_sldVideo");
            _txtNeedVideo = UnityHelper.GetTheChildNodeComponetScripts<Text>(gameObject, "_txtNeedVideo");
            _sldTextVideo = UnityHelper.GetTheChildNodeComponetScripts<Text>(_sldVideo.gameObject, "Text");
            var btnWithdraw = UnityHelper.GetTheChildNodeComponetScripts<Button>(gameObject, "_btnGoldWithdraw");
            btnWithdraw.onClick.AddListener(OnBtnWithdraw);
        }

        #endregion

        #region Event

        private void OnBtnWithdraw()
        {
            GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.WithDrawCoin);
            switch (_myWithdrawData.WithDraw.money)
            {
                case 38:
                    GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.WithDrawLow);
                    break;
                case 68:
                    GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.WithDrawMedium);
                    break;
                case 88:
                    GL_Analytics_Logic._instance.SendLogEvent(EAnalyticsType.WithDrawHigh);
                    break;
            }
            if (_myWithdraw.IsGoldCanWithdraw(_myWithdrawData, true))
            {
                //判断是否微信登陆.
                if (!GL_PlayerData._instance.IsLoginWeChat())
                {
                    //登陆微信
                    Action show =()=> _myWithdraw.RefreshPlayer();
                    UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_WeChatLogin,show);
                }
                else
                {
                    Net_WithDraw draw = new Net_WithDraw();
                    draw.withDrawId = _myWithdrawData.WithDraw.id;
                    draw.withDrawType = 7;
                    draw.type = 2;
                    GL_ServerCommunication._instance.Send(Cmd.WithDraw, JsonUtility.ToJson(draw), (s =>
                    {
                        _myWithdraw.CB_RedWithDraw(s);
                    }));
                }
            }
        }
        
        #endregion

        #region CustomField

        private UI_IF_NewWithdraw _myWithdraw;

        private MyWithdrawData _myWithdrawData;

        #endregion 

        #region Logic

        public void Init(BaseUIForm baseUIForm, MyWithdrawData data)
        {
            _myWithdraw = (UI_IF_NewWithdraw)baseUIForm;
            _myWithdrawData = data;
            _txtMoney.text = string.Format(_moneyFormat,data.WithDraw.money * 0.01f);
            int needAd = data.WithDraw.viewAdTimes - data.ViewNum;
            if (needAd > 0)
            {
                int offsetAd = data.WithDraw.viewAdTimes - needAd;
                offsetAd = offsetAd > 0 ? offsetAd : 0;
                _txtNeedVideo.text = $"还需<color=#ff0000>{needAd}</color>次视频";
                _sldVideo.value = (float)offsetAd / data.WithDraw.viewAdTimes;
                _sldTextVideo.text = $"{offsetAd}/{data.WithDraw.viewAdTimes}";
            }
            else
            {
                _txtNeedVideo.text = "可以提现";
                _sldVideo.value = 1;
                _sldTextVideo.text = $"{data.WithDraw.viewAdTimes}/{data.WithDraw.viewAdTimes}";
            }
        }

        #endregion

    }
}