/***
 * 
 *    Title: "SUIFW"UI框架项目
 *           主题： 事件触发监听      
 *    Description: 
 *           功能： 实现对于任何对象的监听处理。
 *    Date: 2017
 *    Version: 0.1版本
 *    Modify Recoder: 
 *    
 *   
 */
using UnityEngine;
using UnityEngine.EventSystems;

namespace SUIFW
{
    public class EventTriggerListener : UnityEngine.EventSystems.EventTrigger
    {
        public delegate void VoidDelegate(GameObject go);
        public delegate void VoidDelegateDrag(PointerEventData eventBaseData);
        public VoidDelegate onClick;
        public VoidDelegate onDown;
        public VoidDelegate onEnter;
        public VoidDelegate onExit;
        public VoidDelegate onUp;
        public VoidDelegate onSelect;
        public VoidDelegate onUpdateSelect;
        public VoidDelegateDrag onDrag;




        /// <summary>
        /// 得到“监听器”组件
        /// </summary>
        /// <param name="go">监听的游戏对象</param>
        /// <returns>
        /// 监听器
        /// </returns>
        public static EventTriggerListener Get(GameObject go)
        {
            EventTriggerListener lister = go.GetComponent<EventTriggerListener>();
            if (lister == null)
            {
                lister = go.AddComponent<EventTriggerListener>();
            }
            return lister;
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            if (GL_CoreData._instance._isItemMoving) return;
            if (onClick != null)
            {
                //弥补当按钮为不可点击时依然响应点击事件的bug
                var btn = gameObject.GetComponent<UnityEngine.UI.Button>();
                if (btn != null && !btn.interactable)
                {
                    return;
                }
                if (onClick != null)
                {
                    onClick(gameObject);
                }
            }
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            if (onDown != null)
            {
                onDown(gameObject);
            }
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            if (onEnter != null)
            {
                onEnter(gameObject);
            }
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            if (onExit != null)
            {
                onExit(gameObject);
            }
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            if (onUp != null)
            {
                onUp(gameObject);
            }
        }

        public override void OnSelect(BaseEventData eventBaseData)
        {
            if (onSelect != null)
            {
                onSelect(gameObject);
            }
        }

        public override void OnUpdateSelected(BaseEventData eventBaseData)
        {
            if (onUpdateSelect != null)
            {
                onUpdateSelect(gameObject);
            }
        }

        public override void OnDrag(PointerEventData eventBaseData)
        {
            if (onDrag != null)
            {
                onDrag(eventBaseData);
            }
        }

    }//Class_end
}
