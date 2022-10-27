/***
 * 
 *    Title: "SUIFW" UI框架项目
 *           主题： UI遮罩管理器  
 *    Description: 
 *           功能： 负责“弹出窗体”模态显示实现
 *                  
 *    Date: 2017
 *    Version: 0.1版本
 *    Modify Recoder: 
 *    
 *   
 */
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.UI;

namespace SUIFW
{
    public class UIMaskMgr : MonoBehaviour
    {
        /*  字段 */
        //本脚本私有单例
        private static UIMaskMgr _Instance = null;
        //UI根节点对象
        private GameObject _goCanvasRoot = null;
        //UI脚本节点对象
        private Transform _traUIScriptsNode = null;
        //顶层面板
        private GameObject _goTopPanel;
        //遮罩面板
        private GameObject _goMaskPanel;
        private Canvas _goMaskCanvas;
        //UI摄像机
        //UI摄像机原始的“层深”
        private float _originalUICameralDepth;

        //private Color color1 = new Color(SysDefine.Sys_UiMask_Impenetrable_color_RGB, SysDefine.Sys_UiMask_Impenetrable_color_RGB, SysDefine.Sys_UiMask_Impenetrable_color_RGB, SysDefine.Sys_UiMask_Lucency_color_RGB_A);
        //private Color color2 = new Color(SysDefine.Sys_UiMask_Trans_Lucency_color_RGB, SysDefine.Sys_UiMask_Trans_Lucency_color_RGB, SysDefine.Sys_UiMask_Trans_Lucency_color_RGB, SysDefine.Sys_UiMask_Trans_Lucency_color_RGB_A);
        //private Color color3 = new Color(SysDefine.Sys_UiMask_Impenetrable_color_RGB, SysDefine.Sys_UiMask_Impenetrable_color_RGB, SysDefine.Sys_UiMask_Impenetrable_color_RGB, SysDefine.Sys_UiMask_Impenetrable_color_RGB_A);

        private string _ownUIName;  //遮罩所属UI
        private UIBaseState _maskState; //遮罩状态
        //得到实例
        public static UIMaskMgr GetInstance()
        {
            if (_Instance == null)
            {
                _Instance = new GameObject("UIMaskMgr").AddComponent<UIMaskMgr>();
            }
            return _Instance;
        }

        public void Init()
        {
            //得到UI根节点对象、脚本节点对象
            _goCanvasRoot = GameObject.FindGameObjectWithTag(SysDefine.SYS_TAG_CANVAS);
            _traUIScriptsNode = UnityHelper.FindTheChildNode(_goCanvasRoot, SysDefine.SYS_SCRIPTMANAGER_NODE);
            //把本脚本实例，作为“脚本节点对象”的子节点。
            UnityHelper.AddChildNodeToParentNode(_traUIScriptsNode, this.gameObject.transform);
            //得到“顶层面板”、“遮罩面板”
            _goTopPanel = _goCanvasRoot;
            _goMaskPanel = UnityHelper.FindTheChildNode(_goCanvasRoot, "_UIMaskPanel").gameObject;
            _goMaskCanvas = _goMaskPanel.GetComponent<Canvas>();

            //得到UI摄像机原始的“层深”
            _originalUICameralDepth = UIUtils.GetUGUICam().depth;
        }

        /// <summary>
        /// 设置遮罩状态
        /// </summary>
        /// <param name="goDisplayUIForms">需要显示的UI窗体</param>
        /// <param name="lucenyType">显示透明度属性</param>
	    public void SetMaskWindow(GameObject goDisplayUIForms, float time, UIFormLucenyType lucenyType = UIFormLucenyType.Lucency)
        {
            _ownUIName = goDisplayUIForms.name;
            //顶层窗体下移
            _goTopPanel.transform.SetAsLastSibling();
            //启用遮罩窗体以及设置透明度
            switch (lucenyType)
            {
                //完全透明，不能穿透
                case UIFormLucenyType.Lucency:
                    _goMaskPanel.SetActive(true);
                    StopAllCoroutines();
                    StartCoroutine(OpenMask(time, Color.clear));
                    break;
                //深透明，不能穿透
                case UIFormLucenyType.Dark:
                    _goMaskPanel.SetActive(true);
                    StopAllCoroutines();
                    StartCoroutine(OpenMask(time, new Color(0, 0, 0, SysDefine.UiMask_Dark_Color_A)));
                    break;
                //浅透明，不能穿透
                case UIFormLucenyType.Light:
                    _goMaskPanel.SetActive(true);
                    StopAllCoroutines();
                    StartCoroutine(OpenMask(time, new Color(0, 0, 0, SysDefine.UiMask_Light_Color_A)));
                    break;
                //可以穿透
                case UIFormLucenyType.Pentrate:
                    if (_goMaskPanel.activeInHierarchy)
                    {
                        _maskState = UIBaseState.Closeed;
                        _goMaskPanel.SetActive(false);
                    }
                    break;

                default:
                    break;
            }



            //遮罩窗体下移
            _goMaskPanel.transform.SetAsLastSibling();
            //显示窗体的下移
            goDisplayUIForms.transform.SetAsLastSibling();
            //增加当前UI摄像机的层深（保证当前摄像机为最前显示）

            UIUtils.GetUGUICam().depth = UIUtils.GetUGUICam().depth + 100;    //增加层深


        }

        //协程 开启遮罩
        public IEnumerator OpenMask(float time, Color targetColor)
        {
            _maskState = UIBaseState.Opening;
            float t = 0;
            while (t <= time)
            {
                t += Time.deltaTime;
                _goMaskPanel.GetComponent<Image>().color = Color.Lerp(Color.clear, targetColor, t / time);
                yield return null;
            }
            _goMaskPanel.SetActive(true);
            _maskState = UIBaseState.Opened;
        }

        /// <summary>
        /// 取消遮罩状态
        /// </summary>
	    public void CancelMaskWindow(float time, bool b, BaseUIForm own)
        {
            //如果有 其他动画正在打开遮罩, 则关闭遮罩无效
            if(_maskState == UIBaseState.Opening && _ownUIName != own.gameObject.name)
            {
                //DDebug.LogError("~~~其他界面正在打开遮罩");
                return;
            }

            //顶层窗体上移
            _goTopPanel.transform.SetAsFirstSibling();

            //如果存在其他窗体 ，不隐藏mask
            var lastUI = UIManager.GetInstance().ReturnPopRootLastActiveUI(own);
            if (lastUI != null)
            {
                _goMaskPanel.transform.SetAsLastSibling();
                _goMaskPanel.GetComponent<Image>().color = new Color(0, 0, 0, SysDefine.UiMask_Dark_Color_A);
                lastUI.transform.SetAsLastSibling();
                own.transform.SetAsLastSibling();
            }
            //禁用遮罩窗体
            else
            {
                StopAllCoroutines();
                StartCoroutine(CloseMask(time, _goMaskPanel.GetComponent<Image>().color, b));
            }

            //恢复当前UI摄像机的层深 
            UIUtils.GetUGUICam().depth = _originalUICameralDepth;  //恢复层深
        }

        public IEnumerator CloseMask(float time, Color curColor, bool b)
        {
            float t = 0;
            while (t <= time)
            {
                t += Time.deltaTime;
                _goMaskPanel.GetComponent<Image>().color = Color.Lerp(curColor, Color.clear, t / time);
                yield return null;
            }
            //隐藏
            _goMaskPanel.SetActive(b);
        }

        public int GetSiblingIndex()
        {
            return _goMaskPanel.transform.GetSiblingIndex();
        }

        //刷新遮罩层级
        public void RefreshCanvas()
        {
            //transform.SetAsLastSibling
            //500代表 PopUp的层级
            _goMaskCanvas.sortingOrder = 500 + _goMaskPanel.transform.GetSiblingIndex() * 10;
        }
    }
}