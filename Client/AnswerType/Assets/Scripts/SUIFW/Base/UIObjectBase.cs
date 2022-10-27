#region << 文 件 说 明 >>

/*----------------------------------------------------------------
// 文件名称：UIItemBase
// 创 建 者：Yangsong
// 创建时间：2022年03月02日 星期三 15:00
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

namespace SUIFW
{
    /// <summary>
    /// UI组件基类
    /// </summary>
    public abstract class UIObjectBase : MonoBehaviour,IUIBase
    {
        public abstract void InitObjectNode();

        public virtual void Refresh(){}

        public void RigisterButtonObjectEvent(Button button, EventTriggerListener.VoidDelegate delHandle)
        {
            if (button == null)
            {
                DDebug.LogError("~~~找不到UI 界面: ");
                return;
            }
            
            RigisterButtonEvent(button.gameObject, delHandle);
        }

        public void RigisterButtonObjectEvent(string buttonName, EventTriggerListener.VoidDelegate delHandle)
        {
            Transform tf = UnityHelper.FindTheChildNode(this.gameObject, buttonName);
            if (tf == null)
            {
                DDebug.LogError("~~~找不到UI 界面: " + buttonName);
                return;
            }

            RigisterButtonEvent(tf.gameObject, delHandle);
        }
        
        private void RigisterButtonEvent(GameObject obj, EventTriggerListener.VoidDelegate delHandle)
        {
            //给按钮注册事件方法
            if (obj != null)
            {
                EventTriggerListener.Get(obj).onClick += delHandle;
                EventTriggerListener.Get(obj).onClick += ClickSound;
            }
        }

        private void ClickSound(GameObject go)
        {
            GL_AudioPlayback._instance.Play(2);
        }
    }
}