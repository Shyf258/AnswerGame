#region << 文 件 说 明 >>

/*----------------------------------------------------------------
// 文件名称：UI_AnimHelper
// 创 建 者：Yangsong
// 创建时间：2021年12月17日 星期五 17:14
// 文件版本：V1.0.0
//===============================================================
// 功能描述：
//		
//
//----------------------------------------------------------------*/

#endregion

using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace SUIFW.Helps
{
    /// <summary>
    /// ui动画帮助类
    /// </summary>
    public class UI_AnimHelper
    {
        /// <summary>
        /// 数值累加动画
        /// </summary>
        /// <param name="text">要累积的文本</param>
        /// <param name="duiration">持续时间</param>
        /// <param name="endVaule">累积后的值</param>
        /// /// <param name="startNumber">金币初始的值</param>
        /// <param name="callback">回调</param>
        public static void AddVauleAnim(EItemType type,Text text,float endVaule,float duiration, float startNumber,Action callback = null ,bool isShowText = true)
        {
             // float startVaule = float.Parse(startNumber);
             if (type == EItemType.Bogus)
             {
                 if (isShowText)
                 {
                     DOTween.To(value => text.text = value.ToString("0.00")+"元", startNumber, endVaule, duiration)
                         .SetEase(Ease.OutQuad).SetUpdate(true).onComplete += () => { callback?.Invoke(); };
                 }
                 else
                 {
                     DOTween.To(value => text.text = value.ToString("0"), startNumber, endVaule, duiration)
                         .SetEase(Ease.OutQuad).SetUpdate(true).onComplete += () => { callback?.Invoke(); };
                 }
             }
             else
             {
                 DOTween.To(value => text.text = Mathf.Floor(value).ToString(), startNumber, endVaule, duiration)
                     .SetEase(Ease.OutQuad).SetUpdate(true).onComplete += () => { callback?.Invoke(); };
             }
           
        }

    }
}