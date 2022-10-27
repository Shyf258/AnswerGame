#region << 文 件 说 明 >>

/*----------------------------------------------------------------
// 文件名称：UI_IF_Agreement
// 创 建 者：Yangsong
// 创建时间：2021年11月30日 星期二 16:30
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

namespace SUIFW.Diplomats.Agreement
{
    public enum EAgreement
    {
        User,
        Privacy,
        WithdrawRule
    }

    /// <summary>
    /// 协议弹框
    /// </summary>
    public class UI_IF_Agreement : BaseUIForm
    {
        /// <summary> 通知内容 </summary>
        private Text _txtContent;

        /// <summary> 标题 </summary>
        private Text _txtTitle;

        public override void Init()
        {
            _isFullScreen = false;

            CurrentUIType.UIForms_ShowMode = UIFormShowMode.ReverseChange;
            CurrentUIType.UIForms_Type = UIFormType.PopUp;
            CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Lucency;

            _txtContent = UnityHelper.GetTheChildNodeComponetScripts<Text>(gameObject, "_txtContent");
            _txtTitle = UnityHelper.GetTheChildNodeComponetScripts<Text>(gameObject, "_txtTitle");

            RigisterButtonObjectEvent("_btnBack", (go =>
            {
                CloseUIForm();
            }));
        }

        public override void InitData(object data)
        {
            base.InitData(data);

            EAgreement eAgreement = EAgreement.User;

            if (data != null)
                if (data is Func<EAgreement> func) eAgreement = func.Invoke();

            SetAgreementContent(eAgreement);

        }

        /// <summary>
        /// 设置协议文本内容
        /// </summary>
        private void SetAgreementContent(EAgreement eAgreement)
        {
            string path = "";
            switch (eAgreement)
            {
                case EAgreement.User:
                    path = AppData.UserAgreement;
                    _txtTitle.text = string.Format("{0}用户协议", GL_SDK._instance.GetProductName());
                    break;
                case EAgreement.Privacy:
                    path = AppData.PrivacyAgreement;
                    _txtTitle.text = string.Format("{0}隐私协议", GL_SDK._instance.GetProductName());
                    break;
                case EAgreement.WithdrawRule:
                    path = AppData.WithdrawRule;
                    _txtTitle.text = string.Format("{0}提现规则", GL_SDK._instance.GetProductName());
                    break;
            }

            DDebug.Log($"path:{path}---type:{eAgreement}");

            GL_LoadAssetMgr._instance.LoadAsync<TextAsset>(path, (asset =>
            {
                _txtContent.text = string.Format(asset.text, GL_SDK._instance.GetProductName());
            }));
        }


        public override void RefreshLanguage()
        {

        }

        public override void Refresh(bool recall)
        {

        }

        public override void onUpdate()
        {

        }
    }
}
