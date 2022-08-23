//2018.09.18    关林
//消息处理类

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public delegate void EventCallBack(EventParam param);


public class GL_GameEvent : Mono_Singleton_DontDestroyOnLoad<GL_GameEvent>
{
    private EventCallBack[] _callbackList;  //回调列表
    private EventCallBack[] _singleCallbackList;    //单词回调列表

    private List<EventInfo> _pendingList;       //待处理列表
                                                //private bool _lock = false;     //锁?

    public GL_GameEvent()
    {
        int num = Enum.GetValues(typeof(EEventID)).Length;
        _callbackList = new EventCallBack[num];
        _singleCallbackList = new EventCallBack[num];
        _pendingList = new List<EventInfo>();
    }

    /// <summary>
    /// 注册事件
    /// </summary>
    public void RegisterEvent(EEventID eventID, EventCallBack func, bool isSingle = false)
    {
        int index = (int)eventID;
        if (isSingle)
        {
            if (_singleCallbackList[index] == null)
                _singleCallbackList[index] = func;
            else
                _singleCallbackList[index] += func;
        }
        else
        {
            if (_callbackList[index] == null)
                _callbackList[index] = func;
            else
                _callbackList[index] += func;
        }
    }

    /// <summary>
    /// 注销事件
    /// </summary>
    public void UnregisterEvent(EEventID eventID, EventCallBack func, bool isSingle = false)
    {
        int index = (int)eventID;
        if (isSingle)
        {
            if (_singleCallbackList[index] != null)
                _singleCallbackList[index] -= func;
        }
        else
        {
            if (_callbackList[index] != null)
                _callbackList[index] -= func;
        }
    }

    /// <summary>
    /// 发送事件
    /// </summary>
    public void SendEvent(EEventID eventID, EventParam param = null, bool isImmediately = false)
    {
        //立刻
        if (isImmediately)
            DoLogic(eventID, param);
        else
            _pendingList.Add(new EventInfo(eventID, param));
    }

    public void Update()
    {
        for (int i = _pendingList.Count - 1; i >= 0; --i)
        {
            DoLogic(_pendingList[i]._eventID, _pendingList[i]._param);
        }

        _pendingList.Clear();
    }
    //逻辑处理

    private void DoLogic(EEventID enentID, EventParam param)
    {
        int index = (int)enentID;
        EventCallBack temp = _callbackList[index];
        if (temp != null)
            temp(param);

        temp = _singleCallbackList[index];
        if (temp != null)
        {
            temp(param);
            _singleCallbackList[index] -= temp;
        }

        //try
        //{
        //    int index = (int)enentID;
        //    EventCallBack temp = _callbackList[index];
        //    if (temp != null)
        //        temp(param);

        //    temp = _singleCallbackList[index];
        //    if (temp != null)
        //    {
        //        temp(param);
        //        _singleCallbackList[index] = null;
        //    }
        //}
        //catch (System.Exception ex)
        //{
        //    DDebug.LogError(new System.Exception(string.Format("~~~Excute Event CallBack Error: {0},{1}\n{2}", enentID.ToString(), ex.Message, ex.StackTrace)));
        //}
    }


    private class EventInfo
    {
        public EEventID _eventID;
        public EventParam _param;
        public EventInfo(EEventID id, EventParam _param)
        {
            this._eventID = id;
            this._param = _param;
        }
    }
}
