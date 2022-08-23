#region << 文 件 说 明 >>

/*----------------------------------------------------------------
// 文件名称：UI_IF_GlobalMask
// 创 建 者：Yangsong
// 创建时间：2021年12月17日 星期五 15:59
// 文件版本：V1.0.0
//===============================================================
// 功能描述：
//		
//
//----------------------------------------------------------------*/

#endregion

using UnityEngine.UI;

namespace SUIFW.Diplomats.Common
{
    /// <summary>
    /// 全局遮罩
    /// </summary>
    public class UI_IF_GlobalMask : BaseUIForm
    {
        #region UIField
        

        #endregion

        #region Override

        public override void Init()
        {
            CurrentUIType.UIForms_ShowMode = UIFormShowMode.Normal;
            CurrentUIType.UIForms_Type = UIFormType.Topside;
            CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Lucency;
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

       

        #endregion

        #region CustomField



        #endregion

        #region Logic



        #endregion

    }
}