/***
 * 
 *    Title: "SUIFW" UI框架项目
 *           主题: UI窗体的父类
 *    Description: 
 *           功能：定义所有UI窗体的父类。
 *           定义四个生命周期
 *           
 *           1：Display 显示状态。
 *           2：Hiding 隐藏状态
 *           3：ReDisplay 再显示状态。
 *           4：Freeze 冻结状态。
 *           
 *                  
 *    Date: 2017
 *    Version: 0.1版本
 *    Modify Recoder: 
 *    
 *   
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;

namespace SUIFW
{
    public abstract class BaseUIForm : MonoBehaviour
    {
        /*字段*/
        private UIType _currentUIType = new UIType();
        private bool _isInit = false;
        private CanvasGroup _canvasGroup;
        protected UI_Animator _animator;
        protected Canvas _canvas;

        private Action _onHideCall;

        protected bool _isNeedRefresh;
        public bool IsNeedRefresh => _isNeedRefresh; //是否需要重新刷新
        //private Action _onShowCall;
        //是否自动全面屏适配.
        //初始化 = false时, 界面自己适配高宽比大于2的情况
        protected bool _isFullScreen = true;

        [HideInInspector]
        public bool _isOpenMainUp = false;

        //是否关闭拖拽红包
        protected bool _isHideDragRedpack = false;

        //准备关闭状态,
        [HideInInspector]
        private UIBaseState _uiBaseState;

        //特效列表
        private List<ParticleSystemRenderer> _effectList = new List<ParticleSystemRenderer>();
        //子canvas列表
        private List<Canvas> _childCanvasList = new List<Canvas>();
        //private 
        public bool IsInit
        {
            get
            {
                return _isInit;
            }
        }

        /* 属性*/
        //当前UI窗体类型
        public UIType CurrentUIType
        {
            get { return _currentUIType; }
            set { _currentUIType = value; }
        }




        #region  窗体的四种(生命周期)状态

        /// <summary>
        /// 显示状态
        /// </summary>
        public virtual void Display(bool redisplay = false)
        {
            UiBaseState = UIBaseState.Opening;
            GL_GameEvent._instance.SendEvent(EEventID.UIOpen, new EventParam<BaseUIForm>(this));
            SetCanvasGroup(true);

            if (!_isInit)
                Init();
            BeforeDisplay();

            gameObject.SetActive(true);
            if(_animator == null)
                _animator = GetComponentInChildren<UI_Animator>();

            float animtime = 0;
            if (_animator)
            {
                _onHideCall?.Invoke();
                _onHideCall = null;
                gameObject.SetActive(true);
                Refresh(redisplay);
                _animator.DoShowPlay(PoppedRefresh, this);

                // DDebug.LogError("~~~窗口报错： " + name);
                //有窗口回弹, 时间有点长
                animtime = _animator.ShowAniTimer * 0.8f;
            }
            else
            {
                Refresh(redisplay);
                PoppedRefresh();
            }

            //弹出窗口设置 遮罩
            //mainup不需要刷新遮罩
            if (_currentUIType.UIForms_Type == UIFormType.PopUp && !(this is UI_IF_MainUp))
            {
                UIMaskMgr.GetInstance().SetMaskWindow(this.gameObject, animtime, _currentUIType.UIForm_LucencyType);
            }

            //检测 mainup
            if(_isOpenMainUp)
            {
                UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_MainUp);
            }

            if(_isHideDragRedpack)
                UI_Diplomats._instance.CloseUI(SysDefine.UI_Path_DragRedpack);
        }

        /// <summary>
        /// 关闭中
        /// </summary>
	    public virtual void Hiding()
        {
            UiBaseState = UIBaseState.Closeing;
            GL_GameEvent._instance.SendEvent(EEventID.UIClose, new EventParam<BaseUIForm>(this));
            SetCanvasGroup(false);
            // 动画回调
            void Callback()
            {

            }
            float animtime = 0;
            _onHideCall = Callback;
            if (_animator == null)
                _animator = GetComponentInChildren<UI_Animator>();

            if (_animator)
            {
                animtime = _animator.HideAniTimer;
                _animator.DoHidePlay(() =>
                {
                    gameObject.SetActive(false);
                    OnHide();
                }
                , this);
            }
            else
            {
                gameObject.SetActive(false);
                OnHide(); // 逻辑关闭
            }

            if (_currentUIType.UIForms_Type == UIFormType.PopUp)
            {
                UIMaskMgr.GetInstance().CancelMaskWindow(animtime, false, this);
            }
        }

        /// <summary>
        /// 重新显示状态
        /// </summary>
	    public virtual void Redisplay()
        {
            Display(true);
            return;
            gameObject.SetActive(true);
            //设置模态窗体调用(必须是弹出窗体)
            if (_currentUIType.UIForms_Type == UIFormType.PopUp)
            {
                UIMaskMgr.GetInstance().SetMaskWindow(this.gameObject, 0, _currentUIType.UIForm_LucencyType);
            }
        }

        /// <summary>
        /// 冻结状态
        /// </summary>
	    public virtual void Freeze()
        {
            Hiding();
            // DDebug.LogError(this + "UI冻结");
            //this.gameObject.SetActive(true);
        }
        /// <summary>
        /// 解除冻结
        /// </summary>
        public virtual void ContactFreeze()
        {
            // DDebug.LogError(this + "UI重新显示");
        }
        
        /// <summary>
        /// 解除冻结
        /// </summary>
        public virtual void UiFreeze()
        {
            // DDebug.LogError(this + "UI重新显示");
        }

        public void ClearHide()
        {
            _onHideCall = null;
        }

        public virtual void SetCanvasGroup(bool enable)
        {
            _canvasGroup = gameObject.GetComponent<CanvasGroup>();
            if (!_canvasGroup)
                _canvasGroup = gameObject.AddComponent<CanvasGroup>();
            _canvasGroup.blocksRaycasts = enable;
        }

        #endregion

        #region 封装子类常用的方法

        
        /// <summary>
        /// 注册按钮事件
        /// </summary>
        /// <param name="buttonName">按钮节点名称</param>
        /// <param name="delHandle">委托：需要注册的方法</param>
	    protected void RigisterButtonObjectEvent(string buttonName, EventTriggerListener.VoidDelegate delHandle)
        {
            Transform tf = UnityHelper.FindTheChildNode(this.gameObject, buttonName);
            if (tf == null)
            {
                DDebug.LogError("~~~找不到UI 界面: " + buttonName);
                return;
            }

            RigisterButtonEvent(tf.gameObject, delHandle);
        }

        protected void RigisterButtonObjectEvent(UnityEngine.UI.Button button, EventTriggerListener.VoidDelegate delHandle)
        {
            if (button == null)
            {
                DDebug.LogError("~~~找不到UI 界面: ");
                return;
            }

            RigisterButtonEvent(button.gameObject, delHandle);
        }

        private void RigisterButtonEvent(GameObject obj, EventTriggerListener.VoidDelegate delHandle)
        {
            //给按钮注册事件方法
            if (obj != null)
            {
                EventTriggerListener.Get(obj).onClick += delHandle;
                EventTriggerListener.Get(obj).onClick += ClickSound;
                //EventTriggerListener.Get(obj).onClick += ClickClearAD;
            }
        }

        protected void RigisterObjectEventDown(string buttonName, EventTriggerListener.VoidDelegate delHandle)
        {
            Transform tf = UnityHelper.FindTheChildNode(this.gameObject, buttonName);
            if (tf == null)
            {
                DDebug.LogError("~~~找不到UI 界面: " + buttonName);
                return;
            }
            GameObject goButton = tf.gameObject;
            //给按钮注册事件方法
            if (goButton != null)
            {
                EventTriggerListener.Get(goButton).onDown += delHandle;
            }
        }

        protected void RigisterObjectEventUp(string buttonName, EventTriggerListener.VoidDelegate delHandle)
        {
            Transform tf = UnityHelper.FindTheChildNode(this.gameObject, buttonName);
            if (tf == null)
            {
                DDebug.LogError("~~~找不到UI 界面: " + buttonName);
                return;
            }
            GameObject goButton = tf.gameObject;
            //给按钮注册事件方法
            if (goButton != null)
            {
                EventTriggerListener.Get(goButton).onUp += delHandle;
            }
        }

        protected void RigisterObjectEventDrag(string buttonName, EventTriggerListener.VoidDelegateDrag delHandle)
        {
            Transform tf = UnityHelper.FindTheChildNode(this.gameObject, buttonName);
            if (tf == null)
            {
                DDebug.LogError("~~~找不到UI 界面: " + buttonName);
                return;
            }
            GameObject goButton = tf.gameObject;
            //给按钮注册事件方法
            if (goButton != null)
            {
                EventTriggerListener.Get(goButton).onDrag += delHandle;
            }
        }

        private void ClickSound(GameObject go)
        {
            GL_AudioPlayback._instance.Play(2);
        }

        // private void ClickClearAD(GameObject go)
        // {
        //     GL_CoreData._instance.ClearAdTime();
        // }

        /// <summary>
        /// 打开UI窗体
        /// </summary>
        /// <param name="uiFormName"></param>
        protected void OpenUIForm(string uiFormName, object data = null)
        {
            UIManager.GetInstance().ShowUIForms(uiFormName, data);
            //Refresh();
        }

        /// <summary>
        /// 关闭当前UI窗体
        /// </summary>
	    protected void CloseUIForm()
        {
            var strUIFromName = UnityHelper.GetUIName(this);
            UIManager.GetInstance().CloseUIForms(strUIFromName);
        }

        /// <summary>
        /// 关闭其他窗体
        /// </summary>
        protected void CloseUIForm(string str)
        {
            UIManager.GetInstance().CloseUIForms(str);
        }

        /// <summary>
        /// 显示语言
        /// </summary>
        /// <param name="id"></param>
	    public string Show(int id)
        {
            string strResult = string.Empty;

            strResult = LanguageMgr.GetInstance().ShowText(id);
            return strResult;
        }
        /// <summary>
        /// 当切换语言时，调用这个方法,刷新当前页面所有的文本
        /// </summary>
        public virtual void RefreshLanguage() { }

        /// <summary>
        /// 是否显示状态
        /// </summary>
        public bool IsShow()
        {
            return gameObject.activeInHierarchy;
        }
        
   

        public virtual void RefreshUIColor()
        {
        }

        /// <summary>
        /// 页面初始化
        /// </summary>
        /// <param name="recall"></param>

        #endregion

        #region 生命周期相关

        //刷新UI的层级
        public void RefreshCanvas()
        {
            //引导页面不参与计算
            if (this is UI_IF_Guide)
                return;

            //先计算Ui自己的sortingorder
            _canvas.overrideSorting = true;
            int order = 0;
            if (CurrentUIType.UIForms_Type == UIFormType.Normal)
                order = 0;
            else if (CurrentUIType.UIForms_Type == UIFormType.Fixed)
                order = 100;
            else if (CurrentUIType.UIForms_Type == UIFormType.PopUp)
                order = 500;
            else if (CurrentUIType.UIForms_Type == UIFormType.Topside)
                order = 1500;
            order += transform.GetSiblingIndex() * 10;
            _canvas.sortingOrder = order;

            //刷新特效
            if(_effectList != null && _effectList.Count > 0)
            {
                foreach (var child in _effectList)
                {
                    child.sortingOrder = (child.sortingOrder % 10) + order;
                }
            }

            //刷新子canvas
            if (_childCanvasList != null)
            {
                for (int i = _childCanvasList.Count - 1; i >= 0; i--)
                {
                    if (_childCanvasList[i] == null)
                        _childCanvasList.RemoveAt(i);
                    else
                    {
                        //新手引导不刷新
                        if (_childCanvasList[i].sortingOrder < 1000)
                            _childCanvasList[i].sortingOrder = (_childCanvasList[i].sortingOrder % 10) + order;
                    }

                }
            }

        }
        //显示之前
        public virtual void BeforeDisplay() { }

        public abstract void Refresh(bool recall);

        public virtual void PoppedRefresh()
        {
            UiBaseState = UIBaseState.Opened;
        }

        public abstract void onUpdate();

        public abstract void Init();

        public virtual void InitData(object data)
        {

        }

        public virtual void OnDestory()
        {

        }

        //关闭结束
        public virtual void OnHide()
        {
            UiBaseState = UIBaseState.Closeed;
            if (_isHideDragRedpack)
                UI_Diplomats._instance.ShowUI(SysDefine.UI_Path_DragRedpack);
        }

        void Awake()
        {
            if (!_isInit)
            {
                _isInit = true;
                _canvas = this.GetComponent<Canvas>();
                if (_canvas == null)
                {
                    _canvas = this.gameObject.AddComponent<Canvas>();
                }
                var gr = this.GetComponent<UnityEngine.UI.GraphicRaycaster>();
                if (gr == null)
                {
                    this.gameObject.AddComponent<UnityEngine.UI.GraphicRaycaster>();
                }

                Init();

                CalculateAllParticleAndCanvas();

                //ps.ren
                //全面屏
                OnAdaptFullScreen();
            }
            //Refresh();
            RefreshLanguage();
            RefreshUIColor();

        }

        void Update()
        {
            onUpdate();
        }
        #endregion

        public UIBaseState UiBaseState 
        { 
            get
            {
                return _uiBaseState;
            }
            set
            {
                if(value != _uiBaseState)
                {
                    _uiBaseState = value;
                    switch (value)
                    {
                        case UIBaseState.Opening:
                            Invoke(nameof(RefreshCanvas), 0.02f);
                            break;
                        case UIBaseState.Opened:
                            break;
                        case UIBaseState.Closeing:
                            break;
                        case UIBaseState.Closeed:
                            break;
                        default:
                            break;
                    }
                }
            }
        }
        protected virtual void OnAdaptFullScreen()
        {
            if (!_isFullScreen)
                return;
            var rectTransform = GetComponent<RectTransform>();
            var safeArea = Screen.safeArea;
            var anchorMin = safeArea.position;
            var anchorMax = anchorMin + safeArea.size;

            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;

            rectTransform.anchorMin = anchorMin;
            rectTransform.anchorMax = anchorMax;
            //if (!_isFullScreen)
            //    return;
            ////全面屏
            //if (Screen.height / Screen.width >= 2)
            //{
            //    Transform tra = transform.Find("root");
            //    if (tra != null)
            //    {
            //        RectTransform root = tra.GetComponent<RectTransform>();
            //        if (root != null)
            //        {
            //            root.offsetMin = new Vector2(0, 50);
            //            root.offsetMax = new Vector2(0, -84);
            //        }
            //    }
            //}
        }

        //计算需要修改的特效和canvas
        private void CalculateAllParticleAndCanvas()
        {
            _effectList = new List<ParticleSystemRenderer>(gameObject.GetComponentsInChildren<ParticleSystemRenderer>());
            _childCanvasList = new List<Canvas>(gameObject.GetComponentsInChildren<Canvas>());
            if (_childCanvasList.Contains(_canvas))
                _childCanvasList.Remove(_canvas);

            //计算隐藏的物体
            FindPC(transform);
        }

        public void FindPC(Transform transs, bool isFIndAll = false)
        {
            foreach (Transform t in transs)
            {
                if(!t.gameObject.activeInHierarchy || isFIndAll)
                {
                    var p = t.GetComponent<ParticleSystemRenderer>();
                    if (p != null && !_effectList.Contains(p))
                        _effectList.Add(p);
                    var c = t.GetComponent<Canvas>();
                    if (c != null && !_childCanvasList.Contains(c))
                        _childCanvasList.Add(c);
                }
                FindPC(t, isFIndAll);
            }
        }

        //动态生成的ui 的canvas需要加入列表
        //public void AddCanvas(Canvas canvas)
        //{
        //    _childCanvasList.Add(canvas);
        //}

        //public void AddParticleSystem(ParticleSystemRenderer ps)
        //{
        //    _effectList.Add(ps);
        //}
    }
}