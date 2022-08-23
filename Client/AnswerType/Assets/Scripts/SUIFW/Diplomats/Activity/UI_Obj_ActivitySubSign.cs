#region << 文 件 说 明 >>

/*----------------------------------------------------------------
// 文件名称：UI_Obj_ActivitySign
// 创 建 者：Yangsong
// 创建时间：2022年07月29日 星期五 18:45
// 文件版本：V1.0.0
//===============================================================
// 功能描述：
//		
//
//----------------------------------------------------------------*/

#endregion

using System;
using DataModule;
using SUIFW.Diplomats.Common;
using UnityEngine;
using UnityEngine.UI;

namespace SUIFW.Diplomats.Main
{
    /// <summary>
    /// 活动签到子对象
    /// </summary>
    public class UI_Obj_ActivitySubSign : UIObjectBase
    {
        #region UIField

        /// <summary> 金币 </summary>
        private Text _txtCoin;

        /// <summary> 第几天 </summary>
        private Text _txtDay;
        
        /// <summary> 等待签到 </summary>
        private Text _txtWaitSign;

        private Toggle _toggle;

        #endregion

        #region Override

        public override void InitObjectNode()
        {
            _toggle = GetComponent<Toggle>();
            _txtCoin = UnityHelper.GetTheChildNodeComponetScripts<Text>(gameObject, "_txtCoin");
            _txtDay = UnityHelper.GetTheChildNodeComponetScripts<Text>(gameObject, "_txtDay");
            _txtWaitSign = UnityHelper.GetTheChildNodeComponetScripts<Text>(gameObject, "_txtWaitSign");
        }

        public void RefreshNode(TableActivitySignData data,int curSignDay,int signProgress)
        {
            _txtCoin.text = "+" + data.Reward;
            _txtDay.text = $"第{data.ID}天";

            if (curSignDay > data.ID)
            {
                _txtWaitSign.text = "已签到";
                SetSignState(true);
            }
            else if (curSignDay == data.ID)
            {
                if (signProgress != 0)
                {
                    _txtWaitSign.text = "已签到";
                    SetSignState(true);
                }
                else
                {
                    _txtWaitSign.text = "待签到";
                    SetSignState(true);
                }
            }
            else
            {
                SetSignState(false);
            }
            SetSignSelect(curSignDay == data.ID);
        }

        #endregion

        #region Event

        

        #endregion

        #region CustomField

        
        
        

        #endregion

        #region Logic

        /// <summary>
        /// 设置签到状态
        /// </summary>
        private void SetSignState(bool isSigned)
        {
            _txtWaitSign.SetActive(isSigned);
            _txtCoin.SetActive(!isSigned);
        }
        
        /// <summary>
        /// 当前签到选择
        /// </summary>
        /// <param name="isCanSign"></param>
        private void SetSignSelect(bool isCanSign)
        {
            _toggle.isOn = isCanSign;
        }

        #endregion
    }
}