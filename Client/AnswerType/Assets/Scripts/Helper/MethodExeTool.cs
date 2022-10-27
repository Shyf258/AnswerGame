using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using JetBrains.Annotations;

/// <summary>
/// 方法执行工具
/// </summary>
public static class MethodExeTool
{
    static private ToolScript _script;
    /// <summary>
    /// 工具脚本
    /// </summary>
    /// <value>The coroutine object.</value>
    static private ToolScript script
    {
        get
        {
            if (_script == null)
            {
                GameObject go = new GameObject("MethodExeTool");
                _script = go.AddComponent<ToolScript>();
                GameObject.DontDestroyOnLoad(go);
            }
            return _script;
        }
    }

    /// <summary>
    /// 启动协程
    /// </summary>
    /// <param name="enumerator">Enumerator.</param>
    static public Coroutine StartCoroutine(IEnumerator enumerator)
    {
        return script.StartCoroutine(enumerator);
    }
    /// <summary>
    /// 停止协程
    /// </summary>
    static public void StopCoroutine(Coroutine coroutine)
    {
        if (coroutine != null)
            script.StopCoroutine(coroutine);
    }

    /// <summary>
    /// 延迟调用
    /// </summary>
    static public void Invoke(InvokeHandler.Handler method, float time)
    {
        InvokeHandler temp = new InvokeHandler();
        temp.method = method;
        temp.curTime = 0;
        temp.repeatTime = time;
        temp.repeat = 1;
        script.AddHandler(temp);
    }
    /// <summary>
    /// DoTween另类延时调用
    /// </summary>
    /// <param name="action"></param>
    /// <param name="intervalTime"></param>
    static public void InvokeDT([NotNull]Action action, float intervalTime)
    {
        var seq = DOTween.Sequence();
        seq.AppendInterval(intervalTime);
        seq.AppendCallback((() => action?.Invoke()));
        seq.SetAutoKill(true);
        //设为true时可在Time.timeScale=0的情况下正常执行
        seq.SetUpdate(true);
    }

    /// <summary>
    /// 循环调用
    /// </summary>
    /// <param name="method">Method.</param>
    /// <param name="time">Time.</param>
    /// <param name="repeat">Repeat.</param>
    static public void Loop(InvokeHandler.Handler method, float time, int repeat = 1)
    {
        //DDebug.LogError("~~~Invoke: " + method);
        InvokeHandler temp = new InvokeHandler();
        temp.method = method;
        temp.curTime = time;
        temp.repeatTime = time;
        temp.repeat = repeat;
        script.AddHandler(temp);
    }
    /// <summary>
    /// 取消循环调用
    /// </summary>
    /// <returns><c>true</c> if cancel invoke the specified method; otherwise, <c>false</c>.</returns>
    /// <param name="method">Method.</param>
    static public void CancelInvoke(InvokeHandler.Handler method)
    {
        script.RemoveHandler(method);
    }

    /// <summary>
    /// 循环调用结构
    /// </summary>
    public class InvokeHandler
    {
        /// <summary>
        /// 委托
        /// </summary>
        public delegate void Handler();
        public Handler method;
        /// <summary>
        /// 间隔时间
        /// </summary>
        public float repeatTime = 1f;
        /// <summary>
        /// 调用次数 -1代表无限循环
        /// </summary>
        public int repeat = 1;
        /// <summary>
        /// 当前时间
        /// </summary>
        public float curTime = 0f;
        /// <summary>
        /// 是否完成
        /// </summary>
        /// <value><c>true</c> if this instance is complete; otherwise, <c>false</c>.</value>
        public bool IsComplete
        {
            get
            {
                return repeat == 0;
            }
        }
        /// <summary>
        /// 更新时间
        /// </summary>
        /// <param name="time">Time.</param>
        public bool AddTime(float time)
        {
            curTime += time;
            if (curTime >= repeatTime)
            {
                curTime -= repeatTime;
                execute();
                return false;
            }

            return true;
        }
        /// <summary>
        /// 执行
        /// </summary>
        void execute()
        {
            if (repeat == 0)
                return;
            if (repeat > 0)
                repeat--;

            method?.Invoke();
        }
    }
}

/// <summary>
/// 工具脚本类
/// </summary>
class ToolScript : MonoBehaviour
{
    /// <summary>
    /// 循环调用列表
    /// </summary>
    private Dictionary<MethodExeTool.InvokeHandler.Handler, MethodExeTool.InvokeHandler> handlerList = new Dictionary<MethodExeTool.InvokeHandler.Handler, MethodExeTool.InvokeHandler>();
    /// <summary>
    /// 更新时间
    /// </summary>
    void Update()
    {
        //结束行为中,  可以嵌套下一个及时逻辑,导致handlerList 变化
        //使用try catch跳过这个循环
        //try
        //{
        //    foreach (MethodExeTool.InvokeHandler temp in handlerList.Values)
        //    {
        //        temp.AddTime(Time.deltaTime);
        //    }
        //    DeleteComplete();
        //}
        //catch (Exception)
        //{
        //}
        
        //当前计时器可以执行后, 跳出循环, 防止计时器结束事件,嵌套计时器添加
        foreach (MethodExeTool.InvokeHandler temp in handlerList.Values)
        {
            //temp.AddTime(Time.deltaTime);
            if (!temp.AddTime(Time.deltaTime))
                break;
        }
        DeleteComplete();

    }
    /// <summary>
    /// 删除已完成
    /// </summary>
    void DeleteComplete()
    {
        List<MethodExeTool.InvokeHandler> list = new List<MethodExeTool.InvokeHandler>();
        foreach (MethodExeTool.InvokeHandler temp in handlerList.Values)
        {
            if (temp.IsComplete)
            {
                list.Add(temp);
            }
        }

        foreach (MethodExeTool.InvokeHandler del in list)
        {
            handlerList.Remove(del.method);
        }
    }
    /// <summary>
    /// 添加调用
    /// </summary>
    /// <param name="temp">Temp.</param>
    public void AddHandler(MethodExeTool.InvokeHandler temp)
    {
        RemoveHandler(temp.method);
        handlerList.Add(temp.method, temp);
    }
    /// <summary>
    /// 删除调用
    /// </summary>
    /// <param name="method">Method.</param>
    public void RemoveHandler(MethodExeTool.InvokeHandler.Handler method)
    {
        if (handlerList.ContainsKey(method))
        {
            handlerList.Remove(method);
        }
    }

}