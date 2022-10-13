#region << 文 件 说 明 >>

/*----------------------------------------------------------------
// 文件名称：UI_Obj_Chat
// 创 建 者：Yangsong
// 创建时间：2022年08月03日 星期三 17:21
// 文件版本：V1.0.0
//===============================================================
// 功能描述：
//		
//
//----------------------------------------------------------------*/

#endregion

using UnityEngine;
using UnityEngine.UI;

namespace SUIFW.Diplomats.Common.RedGroup
{
    /// <summary>
    /// 对话组件
    /// </summary>
    public class UI_Obj_Chat : UIObjectBase
    {
        #region UIField

        /// <summary> 头像 </summary>
        private Image _icon;
        
        /// <summary> 名称 </summary>
        private Text _name;

        /// <summary> 内容 </summary>
        private Text _content;

        private Transform _chat;

        private UI_Button _openRed;

        #endregion

        #region Override
        
        public override void InitObjectNode()
        {
            _icon = UnityHelper.GetTheChildNodeComponetScripts<Image>(gameObject, "_icon");
            _chat = UnityHelper.GetTheChildNodeComponetScripts<Transform>(gameObject, "_chat");
            _name = UnityHelper.GetTheChildNodeComponetScripts<Text>(_chat.gameObject, "_name");
            _content = UnityHelper.GetTheChildNodeComponetScripts<Text>(_chat.gameObject, "_content");
            _openRed = UnityHelper.GetTheChildNodeComponetScripts<UI_Button>(gameObject, "_openRed");
            RigisterButtonObjectEvent(_openRed,(go => OnOpenRed()));
        }

        public void RefreshNode()
        {
            
        }

        #endregion

        #region Event

        private void OnOpenRed()
        {
            UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_OpenRedPack);
        }
        
        public void OpenRedPack()
        {
            GL_AD_Logic._instance.PlayAD(GL_AD_Interface.AD_Reward_OpenRedPack,(success =>
            {
                
            }));
        }

        #endregion

        #region CustomField



        #endregion

        #region Logic

        /// <summary>
        /// 设置显示状态
        /// </summary>
        /// <param name="isChat"></param>
        private void SetShowState(bool isChat)
        {
            _chat.SetActive(isChat);
            _openRed.SetActive(!isChat);
        }

        #endregion
    }
}