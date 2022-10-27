#region << 文 件 说 明 >>

/*----------------------------------------------------------------
// 文件名称：UI_HintMessage
// 创 建 者：Yangsong
// 创建时间：2021年12月03日 星期五 19:49
// 文件版本：V1.0.0
//===============================================================
// 功能描述：
//		
//
//----------------------------------------------------------------*/

#endregion

using System.Collections.Generic;
using Helper;
using UnityEngine;
using UnityEngine.UI;

namespace SUIFW.Diplomats.Common
{
    /// <summary>
    /// UI全局提示信息，消息不会冲掉
    /// </summary>
    public class UI_HintMessage : MonoBehaviour
    {
        #region UIField
        
        private static readonly string MessagePath = "SUIFW/Prefab/Common/HintMessage";
        private readonly string _animationName = "HintMessage";

        static UI_HintMessage _instance;
        private static GameObject _this;
        private static Canvas[] _canvass;

        

        private void OnDisable()
        {
            this.gameObject.SetActive(false);
        }

        private Transform _parent => UIManager.GetInstance().transform;
        private static Text _text;
        private static Animator _anim;

        /// <summary>
        /// 等待推送消息队列
        /// </summary>
        private Queue<string> _dataQueue = new Queue<string>();

        /// <summary>
        /// 连续提示间隔时间
        /// </summary>
        private float _intervalTime = 1f;

        #endregion

        #region Override

        public static UI_HintMessage _
        {
            get
            {
                if (_instance == null)
                {
                    var prefab = GL_LoadAssetMgr._instance.Load(MessagePath) as GameObject;
                    _this = Instantiate(prefab);
                    _this.name = "UI_HintMessage";
                    _instance = _this.AddComponent<UI_HintMessage>();
                    _text = _this.GetComponentInChildren<Text>();
                    _anim = _this.GetComponentInChildren<Animator>();
                    _canvass = _this.GetComponentsInChildren<Canvas>();
                    DontDestroyOnLoad(_this); // 防止被销毁
                }

                return _instance;
            }
        }
        
        public void ShowMessage(string data)
        {
            _dataQueue.TriggerEnter(data, Execute);
        }

        /// <summary>
        /// 废弃方法，后续都改用不指定父类
        /// </summary>
        /// <param name="parent">父对象</param>
        /// <param name="data">要显示的字符</param>
        public void ShowMessage(Transform parent, string data)
        {
            _dataQueue.TriggerEnter(data, Execute);
        }

        private void Execute(string data)
        {
            this.gameObject.SetActive(true);
            _text.text = data;
            _this.transform.SetParent(_parent, false);
            
            _anim.Play(_animationName,0,0,_intervalTime,() => { _dataQueue.TriggerNextAndRemove(Execute);});
            
            foreach (var c in _canvass)
            {
                c.sortingOrder = 700;
            }
        }


        #endregion

        #region Event

       

        #endregion

    }
}