//2020.02.07    关林
//关卡玩法

using System;
using System.Collections;
using System.Collections.Generic;
using DataModule;
using Logic.Fly;
using Logic.System.NetWork;
using Newtonsoft.Json;
using SUIFW;
using UnityEngine;
using UnityEngine.Serialization;
using Object = System.Object;
using Random = UnityEngine.Random;

public class GL_GameMode
{
    //关卡状态
    public enum ELevelState
    {
        None,
        WaitStar,               //等待UI通知开始
        BeforeGame,             //游戏主UI
        Playing,                //游戏中
        Reviveing,              //复活中
        Fail,                   //失败
        SettleAnimation,        //结算动画
        SettleWait,             //结算等待
        Settle1,                //结算1  
        Settle2,                //结算2  结算跳关
    }

    private ELevelState _levelState;      //关卡状态
    private ELevelState _lastLevelState;    //上一个状态
    [HideInInspector]


    private int _timer = 60;       //关卡计时器
    public int Timer => _timer;

    /// <summary>
    /// 当前奖励
    /// </summary>
    protected Rewards _curRewards;

    protected Timer _countDown;
    public Timer CountDown => _countDown;


    #region 初始化
    public virtual void Init()
    {

    }

    

    #endregion

    #region 更新
    public virtual void DoUpdate(float dt)
    {
    }

    protected virtual void DoFail() { }
    public virtual void DoSettleWait() { }
    public virtual void DoSettle() { }
    public virtual void DoCheck() { }
    protected virtual void DoPlaying() 
    {
        
    }
    #endregion

    #region 接口
    public virtual void Tip() { }

    public virtual void Clear()
    {
        
    }

    #endregion


    public ELevelState LevelState
    {
        get
        {
            return _levelState;
        }
        set
        {
            //DDebug.LogError("LevelState~~~~~~~~~~~~" + value.ToString());
            if (value != _levelState)
            {
                _lastLevelState = _levelState;
                _levelState = value;
                switch (value)
                {
                    case ELevelState.None:
                        break;
                    case ELevelState.WaitStar:
                        break;
                    case ELevelState.BeforeGame:
                        break;
                    case ELevelState.Playing:
                        DoPlaying();
                        break;
                    case ELevelState.Reviveing:
                        break;
                    case ELevelState.Fail:
                        DoFail();
                        break;
                    case ELevelState.SettleWait:
                        DoSettleWait();
                        break;
                    case ELevelState.Settle1:
                        break;
                    case ELevelState.Settle2:
                        DoSettle();
                        break;
                    default:
                        break;
                }
                
            }
        }
    }

    
}