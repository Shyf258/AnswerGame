#region << 文 件 说 明 >>

/*----------------------------------------------------------------
// 文件名称：UI_Obj_RecordItem
// 创 建 者：Yangsong
// 创建时间：2022年04月07日 星期四 17:16
// 文件版本：V1.0.0
//===============================================================
// 功能描述：
//		
//
//----------------------------------------------------------------*/

#endregion

using System;
using UnityEngine.UI;

namespace SUIFW.Diplomats.Main.MyWithdraw
{
    /// <summary>
    /// 提现记录组件
    /// </summary>
    public class UI_Obj_RecordItem : UIObjectBase
    {
        #region UIField

        /// <summary> 成功 </summary>
        private Text _txtSuccess;
        
        /// <summary> 审核 </summary>
        private Text _txtCheck;
        
        /// <summary> 时间 </summary>
        private Text _txtTime;
        private string _formatTime = "{0}	      {1}";

        /// <summary> 金额 </summary>
        private Text _txtMoney;

        #endregion

        #region Override

        public override void InitObjectNode()
        {
            _txtSuccess = UnityHelper.GetTheChildNodeComponetScripts<Text>(gameObject, "_txtSuccess");
            _txtCheck = UnityHelper.GetTheChildNodeComponetScripts<Text>(gameObject, "_txtCheck");
            _txtTime = UnityHelper.GetTheChildNodeComponetScripts<Text>(gameObject, "_txtTime");
            _txtMoney = UnityHelper.GetTheChildNodeComponetScripts<Text>(gameObject, "_txtMoney");
        }

        #endregion

        #region Event


        #endregion

        #region CustomField
        
        

        #endregion

        #region Logic

        public void Init(WithdrawRecord withdrawRecord)
        {
            _txtCheck.SetActive(false);
            _txtSuccess.SetActive(false);
            _txtMoney.text = withdrawRecord.withDrawNum * 0.01f + "元";
            var dateTime = GL_Tools.GetTime(withdrawRecord.withDrawTime,true);
            _txtTime.text = dateTime.ToString();;
            
            switch (withdrawRecord.status)
            {
                case 1:
                    _txtCheck.SetActive(true);
                    break;
                case 2:
                    _txtSuccess.SetActive(true);
                    break;
            }
        }
        
        #endregion

    }
}