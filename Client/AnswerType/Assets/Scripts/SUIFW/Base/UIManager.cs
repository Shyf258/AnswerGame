/***
 * 
 *    Title: "SUIFW" UI框架项目
 *           主题： UI管理器  
 *    Description: 
 *           功能： 是整个UI框架的核心，用户程序通过本脚本，来实现框架绝大多数的功能实现。
 *                  
 *    Date: 2017
 *    Version: 0.1版本
 *    Modify Recoder: 
 *    
 * 
 *    软件开发原则：
 *    1： “高内聚，低耦合”。
 *    2： 方法的“单一职责”
 *     
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SUIFW
{
    public class UIManager : MonoBehaviour
    {
        /* 字段 */
        private static UIManager _Instance = null;
        
        /// <summary>
        /// UI窗体预设路径(参数1：窗体预设名称，2：表示窗体预设路径)
        /// </summary>
        private Dictionary<string, string> _formsPathsDic;

        /// <summary>
        /// 缓存所有UI窗体
        /// </summary>
        private Dictionary<string, BaseUIForm> _allUIFormsDic;

        /// <summary>
        /// 当前显示的UI窗体
        /// </summary>
        private Dictionary<string, BaseUIForm> _currentShowUIFormsDic;

        //定义“栈”集合,存储显示当前所有[反向切换]的窗体类型
        private Stack<BaseUIForm> _currentUIFormsStack;
        //定义“栈”集合,存储显示当前所有窗体类型
        private Stack<BaseUIForm> _currentAllUIFormsStack;
        //UI根节点
        private RectTransform _canvasTrans = null;
        //全屏幕显示的节点
        private Transform _normalTrans = null;
        //固定显示的节点
        private Transform _fixedTrans = null;
        //弹出节点
        private Transform _popUpTrans = null;
        //最上层节点
        private Transform _topsideTrans = null;

        //UI管理脚本的节点
        private Transform _uiScriptsTrans = null;
        ////测试显示banner
        //private RectTransform _TestBanner = null;
        ////刘海
        //private RectTransform _ImgBangs = null;

        private Camera _uiCamera;

        /// <summary>
        /// 得到实例
        /// </summary>
        /// <returns></returns>
	    public static UIManager GetInstance()
        {
            if (_Instance == null)
            {
                _Instance = new GameObject("UIManager").AddComponent<UIManager>();
            }
            return _Instance;
        }

        public Camera UiCamera => _uiCamera;

        public Transform TraCanvasTransfrom => _canvasTrans;

        //初始化核心数据，加载“UI窗体路径”到集合中。
        public void Init()
        {
            //字段初始化
            _allUIFormsDic = new Dictionary<string, BaseUIForm>();
            _currentShowUIFormsDic = new Dictionary<string, BaseUIForm>();
            _formsPathsDic = new Dictionary<string, string>();
            _currentUIFormsStack = new Stack<BaseUIForm>();
            _currentAllUIFormsStack = new Stack<BaseUIForm>();
            //初始化加载（根UI窗体）Canvas预设
            // InitRootCanvasLoading();
            //得到UI根节点、全屏节点、固定节点、弹出节点
            _canvasTrans = GameObject.FindGameObjectWithTag(SysDefine.SYS_TAG_CANVAS).GetComponent<RectTransform>();
            _uiCamera = GameObject.FindGameObjectWithTag("TagUICamera").transform.GetComponent<Camera>();
            _normalTrans = UnityHelper.FindTheChildNode(_canvasTrans.gameObject, SysDefine.SYS_NORMAL_NODE);
            _fixedTrans = UnityHelper.FindTheChildNode(_canvasTrans.gameObject, SysDefine.SYS_FIXED_NODE);
            _popUpTrans = UnityHelper.FindTheChildNode(_canvasTrans.gameObject, SysDefine.SYS_POPUP_NODE);
            _topsideTrans = UnityHelper.FindTheChildNode(_canvasTrans.gameObject, SysDefine.SYS_TOPSIDE_NODE);

            _uiScriptsTrans = UnityHelper.FindTheChildNode(_canvasTrans.gameObject, SysDefine.SYS_SCRIPTMANAGER_NODE);
            //_TestBanner = UnityHelper.GetTheChildNodeComponetScripts<RectTransform>(_canvasTrans.gameObject, "TestBanner");
            //_ImgBangs = UnityHelper.GetTheChildNodeComponetScripts<RectTransform>(_canvasTrans.gameObject, "ImgBangs");

            //把本脚本作为“根UI窗体”的子节点。
            this.gameObject.transform.SetParent(_uiScriptsTrans, false);
            //"根UI窗体"在场景转换的时候，不允许销毁
            DontDestroyOnLoad(_canvasTrans);
            DontDestroyOnLoad(_uiCamera);
            _canvasTrans.position = new Vector3(0, 20, 0);
            //初始化“UI窗体预设”路径数据
            InitUIFormsPathData();
#if UNITY_EDITOR
            //_TestBanner.gameObject.SetActive(true);
            //if (Screen.height / Screen.width >= 2)
            //{
            //    _ImgBangs.gameObject.SetActive(true);
            //    _TestBanner.sizeDelta = new Vector2(0, 200);
            //}
            //else
            //{
            //    _ImgBangs.gameObject.SetActive(false);
            //    _TestBanner.sizeDelta = new Vector2(0, 150);
            //}
#endif
        }


        /// <summary>
        /// 显示（打开）UI窗体
        /// 功能：
        /// 1: 根据UI窗体的名称，加载到“所有UI窗体”缓存集合中
        /// 2: 根据不同的UI窗体的“显示模式”，分别作不同的加载处理
        /// </summary>
        /// <param name="uiFormName">UI窗体预设的名称</param>
        public void ShowUIForms(string uiFormName, object initData = null)
        {
            BaseUIForm baseUIForms = null;                    //UI窗体基类

            //参数的检查
            if (string.IsNullOrEmpty(uiFormName)) return;

            //根据UI窗体的名称，加载到“所有UI窗体”缓存集合中
            baseUIForms = LoadFormsToAllUIFormsCatch(uiFormName);
            if (baseUIForms == null) return;

            //初始化传参数
            if (initData != null)
            {
                baseUIForms.InitData(initData);
            }
            //是否清空“栈集合”中得数据
            if (baseUIForms.CurrentUIType.IsClearStack)
            {
                ClearStackArray();
            }


            //根据不同的UI窗体的显示模式，分别作不同的加载处理
            switch (baseUIForms.CurrentUIType.UIForms_ShowMode)
            {
                case UIFormShowMode.Normal:                 //“普通显示”窗口模式
                    //把当前窗体加载到“当前窗体”集合中。
                    LoadUIToCurrentCache(uiFormName);
                    break;
                case UIFormShowMode.ReverseChange:          //需要“反向切换”窗口模式
                    PushUIFormToStack(uiFormName);
                    break;
                case UIFormShowMode.HideOther:              //“隐藏其他”窗口模式
                    EnterUIFormsAndHideOther(uiFormName);
                    break;
                default:
                    break;
            }
            UIMaskMgr.GetInstance().RefreshCanvas();
            RefreshCanvas();
        }

        /// <summary>
        /// 关闭（返回上一个）窗体
        /// </summary>
        /// <param name="uiFormName"></param>
        public void CloseUIForms(string uiFormName)
        {
            BaseUIForm baseUiForm;                          //窗体基类


            //参数检查
            if (string.IsNullOrEmpty(uiFormName)) return;
            //“所有UI窗体”集合中，如果没有记录，则直接返回
            _allUIFormsDic.TryGetValue(uiFormName, out baseUiForm);
            if (baseUiForm == null) return;
            //根据窗体不同的显示类型，分别作不同的关闭处理
            switch (baseUiForm.CurrentUIType.UIForms_ShowMode)
            {
                case UIFormShowMode.Normal:
                    //普通窗体的关闭
                    ExitUIForms(uiFormName);
                    break;
                case UIFormShowMode.ReverseChange:
                    //反向切换窗体的关闭
                    PopUIFroms();
                    break;
                case UIFormShowMode.HideOther:
                    //隐藏其他窗体关闭
                    ExitUIFormsAndDisplayOther(uiFormName);
                    break;
                default:
                    break;
            }
            UIMaskMgr.GetInstance().RefreshCanvas();
            RefreshCanvas();

            GL_GuideManager._instance.CheckUIClose(uiFormName);

            //界面关闭时, 检测是否主页之前没有界面了
            //gm mainup main 不在检测范围
            //目前只有新手引导需要检测
            //if(GL_GuideManager._instance.IsCheckReturnMain())
            //{
            //    //UI_Diplomats._instance.MainPage
            //}
        }

        public void CloseAllUI_NormalForms()
        {
            foreach (var key in _allUIFormsDic.Keys)
            {
                if (_allUIFormsDic[key].CurrentUIType.UIForms_Type == UIFormType.Normal)
                {
                    CloseUIForms(key);
                }
            }
        }

        public void CloseAllUI()
        {
            foreach (var key in _allUIFormsDic.Keys)
            {
                if (_allUIFormsDic[key].CurrentUIType.UIForms_Type != UIFormType.Fixed)
                {
                    CloseUIForms(key);
                }
            }
        }

        public void CloseAllUIContainFix()
        {
            foreach (var key in _allUIFormsDic.Keys)
            {

                CloseUIForms(key);

            }
        }


        /// <summary>
        /// 刷新所有页面的多语言的字体
        /// </summary>
        public void RefreshLanguage()
        {
            foreach (var ui in _allUIFormsDic.Values)
            {
                ui.RefreshLanguage();
            }
        }

        /// <summary>
        /// 刷新所有页面的ui颜色
        /// </summary>
        public void RefreshUIColor()
        {
            foreach (var ui in _allUIFormsDic.Values)
            {
                ui.RefreshUIColor();
            }
        }

        public BaseUIForm GetUIFormsFormALLDis(string uiName)
        {
            BaseUIForm baseUIResult = null;                 //加载的返回UI窗体基类

            _allUIFormsDic.TryGetValue(uiName, out baseUIResult);
            return baseUIResult;
        }

        #region  显示“UI管理器”内部核心数据，测试使用

        /// <summary>
        /// 显示"所有UI窗体"集合的数量
        /// </summary>
        /// <returns></returns>
        public int ShowALLUIFormCount()
        {
            if (_allUIFormsDic != null)
            {
                return _allUIFormsDic.Count;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 显示"当前窗体"集合中数量
        /// </summary>
        /// <returns></returns>
        public int ShowCurrentUIFormsCount()
        {
            if (_currentShowUIFormsDic != null)
            {
                return _currentShowUIFormsDic.Count;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 显示“当前栈”集合中窗体数量
        /// </summary>
        /// <returns></returns>
        public int ShowCurrentStackUIFormsCount()
        {
            if (_currentUIFormsStack != null)
            {
                return _currentUIFormsStack.Count;
            }
            else
            {
                return 0;
            }
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 根据UI窗体的名称，加载到“所有UI窗体”缓存集合中
        /// 功能： 检查“所有UI窗体”集合中，是否已经加载过，否则才加载。
        /// </summary>
        /// <param name="uiFormsName">UI窗体（预设）的名称</param>
        /// <returns></returns>
	    private BaseUIForm LoadFormsToAllUIFormsCatch(string uiFormsName)
        {
            BaseUIForm baseUIResult = null;                 //加载的返回UI窗体基类

            _allUIFormsDic.TryGetValue(uiFormsName, out baseUIResult);
            if (baseUIResult == null)
            {
                //加载指定名称的“UI窗体”
                baseUIResult = LoadUIForm(uiFormsName);
            }

            return baseUIResult;
        }

        /// <summary>
        /// 加载指定名称的“UI窗体”
        /// 功能：
        ///    1：根据“UI窗体名称”，加载预设克隆体。
        ///    2：根据不同预设克隆体中带的脚本中不同的“位置信息”，加载到“根窗体”下不同的节点。
        ///    3：隐藏刚创建的UI克隆体。
        ///    4：把克隆体，加入到“所有UI窗体”（缓存）集合中。
        /// 
        /// </summary>
        /// <param name="uiFormName">UI窗体名称</param>
	    private BaseUIForm LoadUIForm(string uiFormName)
        {
            string strUIFormPaths = null;                   //UI窗体路径
            GameObject goCloneUIPrefabs = null;             //创建的UI克隆体预设
            BaseUIForm baseUiForm = null;                     //窗体基类


            //根据UI窗体名称，得到对应的加载路径
            _formsPathsDic.TryGetValue(uiFormName, out strUIFormPaths);
            //根据“UI窗体名称”，加载“预设克隆体”
            if (!string.IsNullOrEmpty(strUIFormPaths))
            {
                goCloneUIPrefabs = GL_LoadAssetMgr._instance.Load(strUIFormPaths) as GameObject;
                goCloneUIPrefabs = GameObject.Instantiate(goCloneUIPrefabs);
            }
            //设置“UI克隆体”的父节点（根据克隆体中带的脚本中不同的“位置信息”）
            if (_canvasTrans != null && goCloneUIPrefabs != null)
            {
                baseUiForm = goCloneUIPrefabs.GetComponent<BaseUIForm>();
                if (baseUiForm == null)
                {
                    Debug.Log("baseUiForm==null! ,请先确认窗体预设对象上是否加载了baseUIForm的子类脚本！ 参数 uiFormName=" + uiFormName);
                    return null;
                }
                if (!baseUiForm.IsInit)
                {
                    baseUiForm.Init();
                }
                switch (baseUiForm.CurrentUIType.UIForms_Type)
                {
                    case UIFormType.Normal:                 //普通窗体节点
                        goCloneUIPrefabs.transform.SetParent(_normalTrans, false);
                        break;
                    case UIFormType.Fixed:                  //固定窗体节点
                        goCloneUIPrefabs.transform.SetParent(_fixedTrans, false);
                        break;
                    case UIFormType.PopUp:                  //弹出窗体节点
                        goCloneUIPrefabs.transform.SetParent(_popUpTrans, false);
                        break;
                    case UIFormType.Topside:                  //最上层窗体节点
                        goCloneUIPrefabs.transform.SetParent(_topsideTrans, false);
                        break;
                    default:
                        break;
                }

                //设置隐藏
                goCloneUIPrefabs.SetActive(false);
                //把克隆体，加入到“所有UI窗体”（缓存）集合中。
                _allUIFormsDic.Add(uiFormName, baseUiForm);
                return baseUiForm;
            }
            else
            {
                Debug.Log("_TraCanvasTransfrom==null Or goCloneUIPrefabs==null!! ,Plese Check!, 参数uiFormName=" + uiFormName);
            }

            Debug.Log("出现不可以预估的错误，请检查，参数 uiFormName=" + uiFormName);// UIFormsConfigInfo中无数据
            return null;
        }//Mehtod_end

        /// <summary>
        /// 把当前窗体加载到“当前窗体”集合中
        /// </summary>
        /// <param name="uiFormName">窗体预设的名称</param>
	    private void LoadUIToCurrentCache(string uiFormName)
        {
            BaseUIForm baseUiForm;                          //UI窗体基类
            BaseUIForm baseUIFormFromAllCache;              //从“所有窗体集合”中得到的窗体

            //如果“正在显示”的集合中，存在整个UI窗体，则直接返回
            _currentShowUIFormsDic.TryGetValue(uiFormName, out baseUiForm);
            if (baseUiForm != null)
            {
                //mainup, 需要排序到其他页面上册,则特殊处理
                if(baseUiForm.IsNeedRefresh)
                    baseUiForm.Redisplay();

                return;
            }
                
            //把当前窗体，加载到“正在显示”集合中
            _allUIFormsDic.TryGetValue(uiFormName, out baseUIFormFromAllCache);
            if (baseUIFormFromAllCache != null)
            {
                AllUiPushStackLogic(baseUIFormFromAllCache);
                _currentShowUIFormsDic.Add(uiFormName, baseUIFormFromAllCache);
                baseUIFormFromAllCache.Display();           //显示当前窗体
            }
        }

        /// <summary>
        /// 所有UI逻辑进栈
        /// </summary>
        /// <param name="baseUIForm"></param>
        private void AllUiPushStackLogic(BaseUIForm baseUIForm)
        {
            if (_currentAllUIFormsStack.Count > 0)
            {
                BaseUIForm topUiForm = _currentAllUIFormsStack.Peek();
                //栈顶元素作冻结处理
                topUiForm.UiFreeze();
            }
            baseUIForm.ContactFreeze();
            _currentAllUIFormsStack.Push(baseUIForm);
        }

        /// <summary>
        /// UI窗体入栈
        /// </summary>
        /// <param name="uiFormName">窗体的名称</param>
        private void PushUIFormToStack(string uiFormName)
        {
            BaseUIForm baseUIForm;                          //UI窗体

            //判断“栈”集合中，是否有其他的窗体，有则“冻结”处理。
            if (_currentUIFormsStack.Count > 0)
            {
                BaseUIForm topUIForm = _currentUIFormsStack.Peek();
                //栈顶元素作冻结处理
                topUIForm.Freeze();
            }
            //判断“UI所有窗体”集合是否有指定的UI窗体，有则处理。
            _allUIFormsDic.TryGetValue(uiFormName, out baseUIForm);
            if (baseUIForm != null)
            {
                AllUiPushStackLogic(baseUIForm);
                //当前窗口显示状态
                baseUIForm.Display();
                //把指定的UI窗体，入栈操作。
                _currentUIFormsStack.Push(baseUIForm);
            }
            else
            {
                Debug.Log("baseUIForm==null,Please Check, 参数 uiFormName=" + uiFormName);
            }
        }

        /// <summary>
        /// 退出指定UI窗体
        /// </summary>
        /// <param name="strUIFormName"></param>
        private void ExitUIForms(string strUIFormName)
        {
            BaseUIForm baseUIForm;                          //窗体基类

            //"正在显示集合"中如果没有记录，则直接返回。
            _currentShowUIFormsDic.TryGetValue(strUIFormName, out baseUIForm);
            if (baseUIForm == null) return;
            //指定窗体，标记为“隐藏状态”，且从"正在显示集合"中移除。
            baseUIForm.Hiding();
            _currentShowUIFormsDic.Remove(strUIFormName);

            if(baseUIForm._isOpenMainUp)
            {
                BaseUIForm mainUp;
                bool isShow = false;
                //当前显示
                foreach (var ui in _currentShowUIFormsDic.Values.Reverse())
                {
                    if (ui is UI_IF_MainUp)
                    {
                        mainUp = ui;
                        continue;
                    }

                    if(ui.CurrentUIType.UIForms_Type == UIFormType.PopUp)
                    {
                        if (ui._isOpenMainUp)
                        {
                            isShow = true;
                            break;
                        }
                        else
                        {
                            //mainup后移
                            int index = UIMaskMgr.GetInstance().GetSiblingIndex();
                            ui.transform.SetSiblingIndex(index + 1);
                        }
                    }
                    else
                    {
                        if (ui._isOpenMainUp)
                        {
                            isShow = true;
                            break;
                        }
                    }
                }

                if (!isShow)
                    CloseUIForms(SysDefine.UI_Path_MainUp);
            }

            OnExitUiForm(baseUIForm);

        }

        //（“反向切换”属性）窗体的出栈逻辑
        private void PopUIFroms()
        {
            if (_currentUIFormsStack != null && _currentUIFormsStack.Count > 0)
            {
                BaseUIForm top = _currentUIFormsStack.Peek();

                //是否清空“栈集合”中得数据
                if (top.CurrentUIType.IsClearStackOnHide)
                {
                    ClearStackArrayExceptTop();
                }
            }

            if (_currentUIFormsStack.Count >= 2)
            {
                //出栈处理
                BaseUIForm topUIForms = _currentUIFormsStack.Pop();
                //做隐藏处理
                topUIForms.Hiding();

                //出栈后，下一个窗体做“重新显示”处理。
                BaseUIForm nextUIForms = _currentUIFormsStack.Peek();
                nextUIForms.Redisplay();
                OnExitUiForm(topUIForms);
            }
            else if (_currentUIFormsStack.Count == 1)
            {
                //出栈处理
                BaseUIForm topUIForms = _currentUIFormsStack.Pop();
                //做隐藏处理
                topUIForms.Hiding();
                OnExitUiForm(topUIForms);
            }
        }

        private void OnExitUiForm(BaseUIForm nextUIForms)
        {
            if (_currentAllUIFormsStack.Count > 0)
            {
                while (_currentAllUIFormsStack.Count > 0 && _currentAllUIFormsStack.Peek() != nextUIForms)
                {
                    _currentAllUIFormsStack.Pop();
                }
                if (_currentAllUIFormsStack.Count > 0 && _currentAllUIFormsStack.Peek() == nextUIForms)
                    _currentAllUIFormsStack.Pop();
            }

            if (_currentAllUIFormsStack.Count > 0)
            {
                _currentAllUIFormsStack.Peek().ContactFreeze();
            }
        }

        /// <summary>
        /// (“隐藏其他”属性)打开窗体，且隐藏其他窗体
        /// </summary>
        /// <param name="strUIName">打开的指定窗体名称</param>
        private void EnterUIFormsAndHideOther(string strUIName)
        {
            BaseUIForm baseUIForm;                          //UI窗体基类
            BaseUIForm baseUIFormFromALL;                   //从集合中得到的UI窗体基类


            //参数检查
            if (string.IsNullOrEmpty(strUIName)) return;

            _currentShowUIFormsDic.TryGetValue(strUIName, out baseUIForm);
            if (baseUIForm != null)
            {
                //dz 
                baseUIForm.Display();
                _currentShowUIFormsDic.Remove(strUIName);
            }
            //把“正在显示集合”与“栈集合”中所有窗体都隐藏。
            foreach (BaseUIForm baseUI in _currentShowUIFormsDic.Values)
            {
                baseUI.Hiding();
            }
            foreach (BaseUIForm staUI in _currentUIFormsStack)
            {
                staUI.Hiding();
            }

            //把当前窗体加入到“正在显示窗体”集合中，且做显示处理。
            _allUIFormsDic.TryGetValue(strUIName, out baseUIFormFromALL);
            if (baseUIFormFromALL != null)
            {
                _currentShowUIFormsDic.Add(strUIName, baseUIFormFromALL);
                //窗体显示
                baseUIFormFromALL.Display();
            }
        }

        /// <summary>
        /// (“隐藏其他”属性)关闭窗体，且显示其他窗体
        /// </summary>
        /// <param name="strUIName">打开的指定窗体名称</param>
        private void ExitUIFormsAndDisplayOther(string strUIName)
        {
            BaseUIForm baseUIForm;                          //UI窗体基类


            //参数检查
            if (string.IsNullOrEmpty(strUIName)) return;

            _currentShowUIFormsDic.TryGetValue(strUIName, out baseUIForm);
            if (baseUIForm == null) return;

            //当前窗体隐藏状态，且“正在显示”集合中，移除本窗体
            //baseUIForm.Hiding();
            //_DicCurrentShowUIForms.Remove(strUIName);
            ExitUIForms(strUIName);

            //把“正在显示集合”与“栈集合”中所有窗体都定义重新显示状态。
            foreach (BaseUIForm baseUI in _currentShowUIFormsDic.Values)
            {
                baseUI.Redisplay();
            }
            foreach (BaseUIForm staUI in _currentUIFormsStack)
            {
                staUI.Redisplay();
            }
        }

        /// <summary>
        /// 是否清空“栈集合”中得数据
        /// </summary>
        /// <returns></returns>
        private bool ClearStackArray()
        {
            if (_currentUIFormsStack != null && _currentUIFormsStack.Count >= 1)
            {
                //清空栈集合
                _currentUIFormsStack.Clear();
                return true;
            }

            return false;
        }

        /// <summary>
        /// 是否清空“栈集合”中得数据
        /// </summary>
        /// <returns></returns>
        private bool ClearStackArrayExceptTop()
        {
            if (_currentUIFormsStack != null && _currentUIFormsStack.Count > 1)
            {
                var ui = _currentUIFormsStack.Pop();

                //清空栈集合
                _currentUIFormsStack.Clear();
                _currentUIFormsStack.Push(ui);
                return true;
            }

            return false;
        }

        /// <summary>
        /// 初始化“UI窗体预设”路径数据
        /// </summary>
	    private void InitUIFormsPathData()
        {
            ConfigManagerByJson configMgr = new ConfigManagerByJson(SysDefine.SYS_PATH_UIFORMS_CONFIG_INFO);
            if (configMgr != null)
            {
                _formsPathsDic = configMgr.AppSetting;
            }
        }

        #endregion


        public BaseUIForm GetUI(string uiFormName)
        {
            //参数的检查
            if (string.IsNullOrEmpty(uiFormName)) return null;

            //
            foreach (var n in _allUIFormsDic.Keys)
            {
                if (n == uiFormName)
                {
                    return _allUIFormsDic[n];
                }
            }
            return null;
        }

        public BaseUIForm ReturnPopRootLastActiveUI(BaseUIForm own)
        {
            BaseUIForm res = null;
            for (int i = 0; i < _popUpTrans.childCount; i++)
            {
                var child = _popUpTrans.GetChild(i).GetComponent<BaseUIForm>();
                if (child != null && child != own)
                {
                    if (child.UiBaseState == UIBaseState.Opened
                        && child.CurrentUIType.UIForm_LucencyType != UIFormLucenyType.Pentrate
                        && child.CurrentUIType.UIForm_LucencyType != UIFormLucenyType.Lucency)
                    {
                        res = child;
                        break;
                    }
                }
            }
            return res;
        }

        public BaseUIForm GetTopUiNoExceptSelf(BaseUIForm self)
        {
            var max = 0;

            BaseUIForm ret = null;

            foreach (var ui in _currentShowUIFormsDic)
            {
                if (ui.Value != null && ui.Value.CurrentUIType.UIForms_Type == UIFormType.PopUp)
                {
                    var canvas = ui.Value.GetComponent<Canvas>();
                    if (max < canvas.sortingOrder)
                    {
                        max = canvas.sortingOrder;
                        ret = ui.Value;
                    }
                }
            }

            return ret;
        }
        public BaseUIForm GetTopPopUpUi(BaseUIForm self)
        {
            var max = 0;

            BaseUIForm ret = null;

            foreach (var ui in _allUIFormsDic)
            {
                if (ui.Value != null && ui.Value.IsShow() && ui.Value != self && ui.Value.CurrentUIType.UIForms_Type == UIFormType.PopUp)
                {
                    var canvas = ui.Value.GetComponent<Canvas>();
                    if (max < canvas.sortingOrder)
                    {
                        max = canvas.sortingOrder;
                        ret = ui.Value;
                    }
                }
            }

            return ret;
        }

        public bool IsTopUi(BaseUIForm self)
        {
            var max = -2;

            BaseUIForm ret = null;

            foreach (var ui in _allUIFormsDic)
            {
                if (ui.Value is UI_IF_GM) continue;
                if (ui.Value != null && ui.Value.IsShow())
                {
                    var canvas = ui.Value.GetComponent<Canvas>();
                    if (max < canvas.sortingOrder)
                    {
                        max = canvas.sortingOrder;
                        ret = ui.Value;
                    }
                }
            }

            if (ret && ret == self)
            {
                return true;
            }
            return false;
        }


        //是否有显示的弹窗
        public bool IsShowPopup(bool isIgnoreMainUp = true)
        {
            bool result = false;
            foreach (var ui in _currentShowUIFormsDic.Values)
            {
                if(ui.CurrentUIType.UIForms_Type == UIFormType.PopUp)
                {
                    if (isIgnoreMainUp && ui is UI_IF_MainUp)
                        continue;

                    if(ui.IsShow())
                    {
                        result = true;
                        break;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 界面是否在显示中
        /// </summary>
        public bool IsShow(string name)
        {
            bool result = false;
            foreach (var ui in _currentShowUIFormsDic.Keys)
            {
                if (ui == name)
                {
                    result = true;
                    break;
                }
            }

            return result;
        }

        private void RefreshCanvas()
        {
            foreach (var ui in _currentShowUIFormsDic.Values)
            {
                if (!ui.gameObject.activeInHierarchy)
                    continue;

                if(ui.CurrentUIType.UIForms_Type == UIFormType.PopUp)
                {
                    ui.RefreshCanvas();
                }
            }
        }

        public UI_IF_MainUp GetMainUp()
        {
            return GetUI(SysDefine.UI_Path_MainUp) as UI_IF_MainUp;
        }
        
        public UI_IF_Main GetMain()
        {
            return GetUI(SysDefine.UI_Path_Main) as UI_IF_Main;
        }

    }//class_end
}