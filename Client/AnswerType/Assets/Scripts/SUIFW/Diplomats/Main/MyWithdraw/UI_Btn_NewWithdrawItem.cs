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
using UnityEngine;
using UnityEngine.UI;

namespace SUIFW.Diplomats.Main.MyWithdraw
{
    /// <summary>
    /// 我的提现组件
    /// </summary>
    public class UI_Btn_NewWithdrawItem : UIObjectBase
    {
        #region UIField

        /// <summary> 金额 </summary>
        private Text _txtMoney;
        private string _moneyFormat = "{0}<size=45>元</size>";
        /// <summary> 金额 </summary>
        private Text _txtLimit;
        private Transform _tfLimit;
        private string[] _formatLimits = {"今日剩余{0}次","无限制"};

        /// <summary> 该组件节点 </summary>
        public Transform _itemNode;

        private UI_Button _btnItem;

        private Image _imgBtn;

        /// <summary> 能提现特效 </summary>
        private Transform _tfEffectCanWithdraw;

        private Animation _animation;
        
        /*奖励金额颜色*/
        public Color32[] textColors = {new Color32(255, 255, 255,255), new Color32(127,33,0,255)};

        #endregion

        #region Override

        public override void InitObjectNode()
        {
            _itemNode = UnityHelper.GetTheChildNodeComponetScripts<Transform>(gameObject, "_itemNode");
            _txtMoney = UnityHelper.GetTheChildNodeComponetScripts<Text>(gameObject, "_txtMoney");
            
            _tfLimit = UnityHelper.GetTheChildNodeComponetScripts<Transform>(gameObject, "_tfLimit");
            _txtLimit = UnityHelper.GetTheChildNodeComponetScripts<Text>(_tfLimit.gameObject, "_txtLimit");

            _tfEffectCanWithdraw = UnityHelper.GetTheChildNodeComponetScripts<Transform>(_itemNode.gameObject, "_tfEffectCanWithdraw");

            _animation = _itemNode.GetComponent<Animation>();

            _btnItem = _itemNode.GetComponent<UI_Button>();
            _imgBtn = _btnItem.GetComponent<Image>();
            _btnItem.onClick.AddListener(OnBtnItem);
        }

        private void OnDisable()
        {
            //排除假现金第三个不显示的
            if (_myWithdrawData.EnumMyWithdraw == EnumMyWithdraw.Red && _myWithdrawData.Index == 2 ||
                _myWithdrawData.EnumMyWithdraw == EnumMyWithdraw.Red && _myWithdrawData.Index == 0)
            {
                return;
            }
            
            // if (!gameObject.activeSelf)
            //     gameObject.SetActive(true);
            // if (!_itemNode.gameObject.activeSelf)
            //     _itemNode.SetActive(true);
        }

        #endregion

        #region Event

        private void OnBtnItem()
        {
            _myWithdrawData.Action?.Invoke();
            
            switch (_myWithdrawData.EnumMyWithdraw)
            {
                case EnumMyWithdraw.Red:
                    _myWithdraw.SetRedSelectState();
                    break;
                case EnumMyWithdraw.Gold:
                    _myWithdraw.SetGoldSelectState();
                    break;
            }
        }

        /// <summary>
        /// 选择状态
        /// </summary>
        /// <param name="isSelect"></param>
        public void Selectable(bool isSelect)
        {
            _isSelected = isSelect;

            if (isSelect)
            {
                _imgBtn.sprite = _btnItem.spriteState.pressedSprite;
                _txtMoney.color = textColors[0];
            }
            else
            {
                _imgBtn.sprite = _btnItem.spriteState.disabledSprite;
                _txtMoney.color = textColors[1];
            }
        }
        
        public bool IsSelected()
        {
            return _isSelected;
        }


        #endregion

        #region CustomField
        
        private bool _isSelected;
        
        private UI_IF_NewWithdraw _myWithdraw;

        private MyWithdrawData _myWithdrawData;
        
        private Timer _redRewardTimer;

        private string _rewardIdleAnim = "an_tixian_idle_01";

        private DateTime _startTime;
        private DateTime _leaveTime;
        private int _rewardTime = 300;

        #endregion 

        #region Logic

        public void Init(BaseUIForm baseUIForm,MyWithdrawData data)
        {
            _myWithdraw = (UI_IF_NewWithdraw)baseUIForm;
            Selectable(false);
            _myWithdrawData = data;

            if (data.IsCanWithdraw)
            {
                _btnItem.SetPlayIdle(true);
                _tfEffectCanWithdraw.SetActive(true);
                _animation.Play(_rewardIdleAnim);
            }
            else
            {
                _btnItem.SetPlayIdle(false);
                _tfEffectCanWithdraw.SetActive(false);
            }
            
            switch (data.EnumMyWithdraw)
            {
                case EnumMyWithdraw.Red:
                    if (data.Index == 0) //设置默认点击显示
                        Selectable(true);
                    float money = data.WithDraw.money <= 0 ? 0 : data.WithDraw.money * 0.01f;
                    _txtMoney.text = string.Format(_moneyFormat,money);
                    if (data.WithDraw.withDrawLimit == -1)
                    {
                        _txtLimit.text = _formatLimits[1];
                    }
                    else if (data.WithDraw.withDrawLimit == 0)
                    {
                        _tfLimit.SetActive(false);
                    }
                    else
                    {
                        _txtLimit.text = string.Format(_formatLimits[0],data.WithDraw.withDrawLimit);
                    }
                    break;
                case EnumMyWithdraw.Gold:
                    if (data.Index == 0) //设置默认点击显示
                        Selectable(true);
                    _txtMoney.text = string.Format(_moneyFormat,data.WithDraw.money * 0.01f);
                    if (data.WithDraw.withDrawLimit == -1)
                    {
                        _txtLimit.text = _formatLimits[1];
                    }
                    else if (data.WithDraw.withDrawLimit == 0)
                    {
                        _tfLimit.SetActive(false);
                    }
                    else
                    {
                        _txtLimit.text = string.Format(_formatLimits[0],data.WithDraw.withDrawLimit);
                    }
                    break;
            }

            //金币的第一个 需要新手引导
            if(data.EnumMyWithdraw == EnumMyWithdraw.Gold 
                && data.Index == 0)
            {
                GL_Tools.GetComponent<UI_GuideObject>(_btnItem.gameObject);
            }
        }

        public MyWithdrawData GetWithdrawData()
        {
            return _myWithdrawData;
        }

        #endregion

    }
}