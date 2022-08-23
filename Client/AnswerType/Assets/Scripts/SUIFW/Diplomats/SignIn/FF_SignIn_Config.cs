using System;
using System.Collections;
using System.Collections.Generic;
using Logic.Fly;
using Logic.System.NetWork;
using SUIFW;
using SUIFW.Diplomats.Common;
using UnityEngine;

public class FF_SignIn_Config
{
    public bool _isOpen;

    /// <summary>
    /// 可以提现
    /// </summary>
    public bool _canWithDraw;
    
    public void Init()
    {
        _isOpen = Clockin();
    }
   

    //是否能领取
   

    public bool CanWithDraw
    {
        get => _canWithDraw;
        set => _canWithDraw = value;
    }

    /// <summary>
    /// 今日是否已打卡
    /// </summary>
    /// <returns></returns>
    public bool Clockin()
    {
        return GL_PlayerData._instance.SigNetCbClockinConfig.hasClock!=1;
    }

    

}
