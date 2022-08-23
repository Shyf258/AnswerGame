//2021.11.19 关林
//游戏场景

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GL_Scene_GameScene : GL_SceneStateBase
{
    private string _mainMenu = "GameScene";
    public override void EnterScene(Action action = null)
    {
        base.EnterScene(action);

        //加载主界面, 分两部分
        //1. 加载mainmenu
        LoadSceneState = ELoadSceneState.Stage1;
        
        //2. 加载对应的levellist
    }

    public override void ExitScene()
    {
        UnLoadScene(_mainMenu);
    }

    public override void DoUpdate(float dt)
    {
        //计算当前场景进度
        if(LoadSceneState == ELoadSceneState.Stage1)
        {
            //加载mainmenu 占一半进度
            float value = CalculateProgress(_asyncs[_mainMenu]);
            if (_loadProgress < value)
            {
                _loadProgress += UnityEngine.Random.Range(2, 7);
                if (_loadProgress >= 100 && _asyncs[_mainMenu].isDone)
                {
                    //下一阶段
                    //加载关卡列表
                    LoadSceneState = ELoadSceneState.Complete;
                }
            }
        }

        _loadProgress = Mathf.Clamp(_loadProgress, 0, 100);
        //DDebug.LogError("~~~_loadProgress :" + _loadProgress);
    }

    public override ELoadSceneState LoadSceneState 
    { 
        get => base.LoadSceneState; 
        set
        {
            if(value != _loadSceneState)
            {
                _loadSceneState = value;
                if(value == ELoadSceneState.Stage1)
                {
                    LoadScene(_mainMenu, false);
                }
                else if(value == ELoadSceneState.Complete)
                {
                    _callBack?.Invoke();
                    
                }
            }
        }
    }

    
}
