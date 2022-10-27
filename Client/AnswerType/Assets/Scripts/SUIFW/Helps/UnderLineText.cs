#region << 文 件 说 明 >>

/*----------------------------------------------------------------
// 文件名称：UnderLineText
// 创 建 者：Yangsong
// 创建时间：2021年12月17日 星期五 16:31
// 文件版本：V1.0.0
//===============================================================
// 功能描述：
//		
//
//----------------------------------------------------------------*/

#endregion

using UnityEngine;
using UnityEngine.UI;

namespace SUIFW.Helps
{
    public enum UnderLineType
    {
        //画线位置
        Bottom = 0,
        Center
    }
    
    /// <summary>
    /// 文本下划线
    /// </summary>
    public class UnderLineText : MonoBehaviour
    {
        public Text linkText;
        public UnderLineType underLineType;
        public bool autoLink = true;
        private string underLineText = "_";

        private void Awake()
        {
            if (underLineType == UnderLineType.Bottom)
            {
                underLineText = "_";
            }
            else
            {
                underLineText = "-";
            }
        }

        private void Start()
        {
            if (autoLink)
                CreateLink(linkText, null);
        }

        private void CreateLink(Text text, UnityEngine.Events.UnityAction onClickBtn = null)
        {
            if (text == null)
                return;

            //克隆Text，获得相同的属性
            Text underline = Instantiate(text) as Text;
            underline.name = "Underline";
            underline.transform.SetParent(text.transform);
            underline.transform.localScale = Vector3.one;
            RectTransform rt = underline.rectTransform;
            //设置下划线坐标和位置
            rt.anchoredPosition3D = Vector3.zero;
            rt.offsetMax = Vector2.zero;
            rt.offsetMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.anchorMin = Vector2.zero;
            underline.text = underLineText;
            float perlineWidth = underline.preferredWidth;      //单个下划线宽度
            float width = text.preferredWidth;
            int lineCount = (int)Mathf.Round(width / perlineWidth);
            for (int i = 1; i < lineCount; i++)
            {
                underline.text += underLineText;
            }
            if (onClickBtn != null)
            {
                var btn = text.gameObject.AddComponent<Button>();
                btn.onClick.AddListener(onClickBtn);
            }
            var underLine = underline.GetComponent<UnderLineText>();
            if (underLine)
            {
                underLine.autoLink = false;
            }
        }
    }
}