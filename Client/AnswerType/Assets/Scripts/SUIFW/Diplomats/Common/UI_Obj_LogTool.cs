#region << 文 件 说 明 >>

/*----------------------------------------------------------------
// 文件名称：UI_Obj_LogTool
// 创 建 者：Yangsong
// 创建时间：2022年06月27日 星期一 12:13
// 文件版本：V1.0.0
//===============================================================
// 功能描述：
//		
//
//----------------------------------------------------------------*/

#endregion

using UnityEngine;
using UnityEngine.UI;

namespace SUIFW.Diplomats.Common
{
    /// <summary>
    /// log工具
    /// </summary>
    public class UI_Obj_LogTool : MonoBehaviour
    {
        #region UIField

      

        #endregion

        #region Override
        
        

        public void InitObjectNode()
        {
            Transform tfLeft =  UnityHelper.FindTheChildNode(this.gameObject, "_btnLeft");
            tfLeft.GetComponent<Button>().onClick.AddListener(OnBtnLeft);
            Transform tfRight =  UnityHelper.FindTheChildNode(this.gameObject, "_btnRight");
            tfRight.GetComponent<Button>().onClick.AddListener(OnBtnRight);
            Transform tfReset =  UnityHelper.FindTheChildNode(this.gameObject, "_btnReset");
            tfReset.GetComponent<Button>().onClick.AddListener(OnBtnReset);
        }

        #endregion

        #region Event
        
        private void OnBtnLeft()
        {
            if (_password.Length < _passwordLen)
            {
                _password += InputLeft();
                Verify();
            }
        }
        
        private void OnBtnRight()
        {
            if (_password.Length < _passwordLen)
            {
                _password += InputRight();
                Verify();
            }
        }

        private void OnBtnReset()
        {
            _password = "";
            UI_HintMessage._.ShowMessage("password reset");
        }
        
        #endregion

        #region CustomField
        
        //密码长度
        private readonly int _passwordLen = 6;
        //验证密码 左左右右左右
        private readonly string _verifyPassword = "112212";

        private string _password = "";

        #endregion

        #region Logic

        private string InputLeft()
        {
            return "1";
        }

        private string InputRight()
        {
            return "2";
        }

        //验证
        private void Verify()
        {
            if (_password.Equals(_verifyPassword))
            {
                bool state = GL_PlayerPrefs.GetBool(EPrefsKey.IsOpenLogToTool);
                GL_PlayerPrefs.SetBool(EPrefsKey.IsOpenLogToTool,!state);
                if (!state)
                {
                    UI_HintMessage._.ShowMessage("log is open");
                }
                else
                {
                    UI_HintMessage._.ShowMessage("log is close");
                }
                
                return;
            }

            if (_password.Length == _passwordLen)
            {
                UI_HintMessage._.ShowMessage("password input wrong");
            }
        }

        #endregion

    }
}