#region << 文 件 说 明 >>

/*----------------------------------------------------------------
// 文件名称：UI_IF_RedGroup
// 创 建 者：Yangsong
// 创建时间：2022年08月03日 星期三 17:20
// 文件版本：V1.0.0
//===============================================================
// 功能描述：
//		
//
//----------------------------------------------------------------*/

#endregion

using UnityEngine.UI;

namespace SUIFW.Diplomats.Common.RedGroup
{
    /// <summary>
    /// 红包群弹窗
    /// </summary>
    public class UI_IF_RedGroup : BaseUIForm
    {
        #region UIField
        


        #endregion

        #region Override

        public override void Init()
        {
            CurrentUIType.UIForms_ShowMode = UIFormShowMode.Normal;
            CurrentUIType.UIForms_Type = UIFormType.PopUp;
            CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Lucency;
            
            RigisterButtonObjectEvent("_btnFamily", (go => { OnBtnFamily(); }));
            RigisterButtonObjectEvent("_btnOfficial", (go => { OnBtnOfficial(); }));
            RigisterButtonObjectEvent("_btnBack", (go => { OnBtnClose(); }));
        }

        public override void InitData(object data)
        {
            base.InitData(data);

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

        #endregion

        #region Event

        /// <summary>
        /// 家族群
        /// </summary>
        private void OnBtnFamily()
        {
            CloseUIForm();
        }
        
        /// <summary>
        /// 官方群
        /// </summary>
        private void OnBtnOfficial()
        {
            CloseUIForm();
        }
        
        private void OnBtnClose()
        {
            CloseUIForm();
        }

        #endregion

        #region CustomField



        #endregion

        #region Logic



        #endregion

    }
}