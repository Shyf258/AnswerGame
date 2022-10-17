#region << 文 件 说 明 >>

/*----------------------------------------------------------------
// 文件名称：UI_IF_WechatWithdrawTip
// 创 建 者：Yangsong
// 创建时间：2022年10月14日 星期五 15:17
// 文件版本：V1.0.0
//===============================================================
// 功能描述：
//		
//
//----------------------------------------------------------------*/

#endregion

using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace SUIFW.Diplomats.Common.Withdraw
{
    /// <summary>
    /// 微信提现提示
    /// </summary>
    public class UI_IF_WechatWithdrawTip : BaseUIForm
    {
        #region UIField

        /// <summary> 提示对象 </summary>
        private Transform _tipObj;
        
        #endregion

        #region Override

        public override void Init()
        {
            CurrentUIType.UIForms_ShowMode   = UIFormShowMode.Normal;
            CurrentUIType.UIForms_Type       = UIFormType.Topside;
            CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Lucency;

            _tipObj   = UnityHelper.GetTheChildNodeComponetScripts<Transform>(gameObject, "_tipObj");
            _startPos = _tipObj.localPosition;
            _endPos   = new Vector3(_startPos.x, _startPos.y - 300);
            
            RigisterButtonObjectEvent("_btnMask",(go => OnBtnMask()));
        }

        public override void RefreshLanguage()
        {

        }

        public override void Refresh(bool recall)
        {
            Show();
        }

        public override void onUpdate()
        {

        }

        #endregion

        #region Event

        private void OnBtnMask()
        {
            OutSide(0);
        }

        #endregion

        #region CustomField

        private Vector3 _startPos;
        private Vector3 _endPos;

        #endregion

        #region Logic
        
        public void Show()
        {
            InSide();
        }

        private void Hide()
        {
            CloseUIForm();
        }

        private void InSide()
        {
            _tipObj.DOLocalMove(_endPos, 0.2f).SetEase(Ease.Linear).onComplete += () => OutSide(5);
        }

        private void OutSide(float time)
        {
            _tipObj.DOLocalMove(_startPos, 0.2f).SetDelay(time).SetEase(Ease.Linear).onComplete += Hide;
        }

        #endregion

    }
}