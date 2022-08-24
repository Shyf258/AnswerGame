#region << 文 件 说 明 >>

/*----------------------------------------------------------------
// 文件名称：UI_IF_Notice
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
using SUIFW.Diplomats.Common;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SUIFW.Diplomats.Agreement
{
    /// <summary>
    /// 通知弹框
    /// </summary>
    public class UI_IF_Notice : BaseUIForm,IPointerClickHandler
    {
        /// <summary> 通知内容 </summary>
        private TextMeshProUGUI _txtNotice;

        private Camera _uiCamera;

        private bool _agree = false;
        
        /// <summary>
        /// 确认回调
        /// </summary>
        //private Action _sureCallBack;
        /// <summary>
        /// 同意协议
        /// </summary>
        private Toggle _agreeToggle;
        public override void Init()
        {
            CurrentUIType.UIForms_ShowMode = UIFormShowMode.Normal;
            CurrentUIType.UIForms_Type = UIFormType.PopUp;
            CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Light;
            _uiCamera = UnityHelper.GetUICamera();
            _txtNotice = UnityHelper.GetTheChildNodeComponetScripts<TextMeshProUGUI>(gameObject, "_txtNotice");
            
            SetNoticeText();

            RigisterButtonObjectEvent("_btnCancel",(go =>
            {
                //DDebug.Log("@@@@@@取消,退出游戏");
                _agree = false;
                CloseUIForm();
            }));
        
            RigisterButtonObjectEvent("_btnSure",(go =>
            {
                //DDebug.Log("@@@@@@确定，进入Loading");
                _agree = true;
                CloseUIForm();
            }));
            
        }

        public override void OnHide()
        {
            base.OnHide();
            GL_Game._instance._sceneSwitch.SetNotice(_agree);
        }

        /// <summary>
        /// 设置通知文本内容
        /// </summary>
        private void SetNoticeText()
        {
            GL_LoadAssetMgr._instance.LoadAsync<TextAsset>(AppData.NoticeAssetPath, (asset =>
            {
                string notice = String.Format(asset.text,GL_SDK._instance.GetProductName());
                _txtNotice.text = notice;
            }));
        }

        private void OnClickUserAgreement(string typeName)
        {
            DDebug.Log($"@@@@@@点击了{typeName}");
            
            switch (typeName)
            {
                case "<用户协议>":
                    Func<EAgreement> typeCallBack0 = () => { return EAgreement.User; };
                    UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_Agreement,typeCallBack0);
                    break;
                case "<隐私协议>":
                    Func<EAgreement> typeCallBack1 = () => { return EAgreement.Privacy; };
                    UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_Agreement,typeCallBack1);
                    break;
            }
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            Vector3 pos = new Vector3(eventData.position.x, eventData.position.y, 0);
            int linkIndex = TMP_TextUtilities.FindIntersectingLink(_txtNotice, pos, _uiCamera); //--UI相机
            if (linkIndex > -1)
            {
                TMP_LinkInfo linkInfo = _txtNotice.textInfo.linkInfo[linkIndex];
                OnClickUserAgreement(linkInfo.GetLinkText());
            }
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
