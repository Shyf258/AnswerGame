//2021.7.14 场景状态基类

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GL_SceneStateBase
{
    public enum ELoadSceneState
    {
        None,
        Stage1,
        Stage2,
        Stage3,
        Complete,
    }

    public float _loadProgress; //场景加载进度

    protected ELoadSceneState _loadSceneState;
    protected Dictionary<string, AsyncOperation> _asyncs;    //异步对象
    protected Action _callBack; //加载回调
    public virtual void EnterScene(Action action = null)
    {
        _loadProgress = 0;
        _asyncs =  new Dictionary<string, AsyncOperation>();
        _callBack = action;
    }
    public virtual void ExitScene()
    {

    }

    public virtual void DoUpdate(float dt)
    {

    }
    public virtual ELoadSceneState LoadSceneState
    {
        get => _loadSceneState;
        set => _loadSceneState = value;
    }
    #region 加载
    //加载场景
    protected virtual AsyncOperation LoadScene(string name, bool isAdd, bool checkDuplicates = false, Action<AsyncOperation> callback = null)
    {
        if (checkDuplicates)
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                if (SceneManager.GetSceneAt(i).name == name)
                {
                    DDebug.LogError("~~~重复加载场景: " + name);
                    return null;
                }

            }
        }

        var async = SceneManager.LoadSceneAsync(name, isAdd ? LoadSceneMode.Additive : LoadSceneMode.Single);
        _asyncs.Add(name, async);
        if (callback != null)
        {
            async.completed += callback;
        }
        return async;

    }

    protected float CalculateProgress(AsyncOperation async)
    {
        float result = (int)(async.progress * 100);
        result = (int)(result / 89f * 100f);
        return result;
    }

    protected virtual AsyncOperation UnLoadScene(string name)
    {
        AsyncOperation async = SceneManager.UnloadSceneAsync(name);
        _asyncs.Remove(name);

        GL_ResourcePool._instance.Clear();
        //GL_SpriteAtlasPool._instance.Clear();
        Resources.UnloadUnusedAssets();
        System.GC.Collect();
        return async;
    }

    //是否所有场景都加载完
    public bool IsLoadOver()
    {
        bool result = true;
        foreach (var scene in _asyncs.Values)
        {
            if(!scene.isDone)
            {
                result = false;
                break;
            }
        }
        return result;
        //throw new NotImplementedException();
    }
    #endregion


}
