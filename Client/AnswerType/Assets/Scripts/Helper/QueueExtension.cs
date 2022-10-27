#region << 文 件 说 明 >>

/*----------------------------------------------------------------
// 文件名称：QueueExtension
// 创 建 者：Yangsong
// 创建时间：2022年03月11日 星期五 10:41
// 文件版本：V1.0.0
//===============================================================
// 功能描述：
//		
//
//----------------------------------------------------------------*/

#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Helper
{
    /// <summary>
    /// 队列扩展
    /// </summary>
    public static class QueueExtension
    {
        /// <summary>
        /// 入列,自动执行
        /// </summary>
        /// <param name="queue"></param>
        /// <param name="value"></param>
        /// <param name="fun"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool TriggerEnter<T>(this Queue<T> queue,T value,Action<T> fun = null)
        {
            bool isContains = queue.Contains(value);
            if (!isContains)
                queue.Enqueue(value);
            else
                DDebug.Log($"存储队列出现重复添加情况，重复元素：{value}，请检查。");
            
            if (queue.Count == 1) TriggerNext(queue,fun);
            return isContains;
        }

        /// <summary>
        /// 下一个
        /// </summary>
        /// <param name="queue"></param>
        /// <param name="fun"></param>
        /// <typeparam name="T"></typeparam>
        public static void TriggerNext<T>(this Queue<T> queue,Action<T> fun = null)
        {
            if (queue.Count > 0)
            {
                var vaule = queue.Peek();
                if (vaule is Action action)
                {
                    action.Invoke();
                    return;
                }
                
                fun?.Invoke(vaule);
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="queue"></param>
        /// <typeparam name="T"></typeparam>
        public static void TriggerRemove<T>(this Queue<T> queue)
        {
            if (queue.Count > 0) queue.Dequeue();
        }

        /// <summary>
        /// 下一个并删除
        /// </summary>
        /// <param name="queue"></param>
        /// <param name="fun"></param>
        /// <typeparam name="T"></typeparam>
        public static void TriggerNextAndRemove<T>(this Queue<T> queue,Action<T> fun = null)
        {
            TriggerRemove(queue);
            TriggerNext(queue,fun);
        }
        
        /// <summary>
        /// 转换成列表形式
        /// </summary>
        public static List<T> ToList<T>(this Queue<T> queue)
        {
            var list = new List<T>();
            while (queue.Count > 0)
                list.Add(queue.Dequeue());

            return list;
        }
    }
}